using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Flower.Backend.Models;
using Flower.Backend.Services.Interfaces;
using Flower.Data.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flower.Backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        private SmtpClient CreateSmtpClient()
        {
            var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                EnableSsl = _settings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            if (!string.IsNullOrEmpty(_settings.Username) && !string.IsNullOrEmpty(_settings.Password))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
            }
            else
            {
                client.UseDefaultCredentials = true;
            }

            return client;
        }

        public async Task SendOrderConfirmationAsync(Order order, string customerEmail, string customerName)
        {
            try
            {
                var body = BuildOrderConfirmationBody(order, customerName);
                var senderEmail = !string.IsNullOrEmpty(_settings.SenderEmail)
                    ? _settings.SenderEmail
                    : _settings.Username ?? "noreply@flowershop.com";
                using var message = new MailMessage
                {
                    From = new MailAddress(senderEmail, _settings.SenderName),
                    Subject = $"Xác nhận đơn hàng #{order.Id} - FlowerShop",
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(customerEmail, customerName));

                using var client = CreateSmtpClient();

                await client.SendMailAsync(message);
                _logger.LogInformation("Order confirmation email sent for order {OrderId} to {Email}", order.Id, customerEmail);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send order confirmation email for order {OrderId}", order.Id);
            }
        }

        private static string BuildOrderConfirmationBody(Order order, string customerName)
        {
            var parsedNotes = ParseNotes(order.Notes);

            var addressParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(order.DeliveryAddress)) addressParts.Add(order.DeliveryAddress.Trim());
            if (!string.IsNullOrWhiteSpace(order.DeliveryDistrict)) addressParts.Add(order.DeliveryDistrict.Trim());
            addressParts.Add("TP. Hồ Chí Minh");
            var fullAddress = string.Join(", ", addressParts);

            var timeSlot = !string.IsNullOrWhiteSpace(order.DeliveryTimeSlot) ? order.DeliveryTimeSlot.Trim() : "N/A";
            var dateStr = order.DeliveryDate?.ToString("dd/MM/yyyy") ?? "N/A";
            var deliveryTime = $"{timeSlot} ngày {dateStr}";

            var orderPlacedTime = order.OrderDate.ToString("dd/MM/yyyy HH:mm");

            // Confirmation time
            var orderConfirmedTime = "Chờ xác nhận";
            if (order.Status == OrderStatus.Confirmed || order.Status == OrderStatus.Preparing || order.Status == OrderStatus.Shipping || order.Status == OrderStatus.Completed || order.VerifiedAt.HasValue || order.PaymentPaidAt.HasValue)
            {
                var dt = order.VerifiedAt ?? order.PaymentPaidAt ?? order.OrderDate.AddMinutes(5);
                orderConfirmedTime = dt.ToString("dd/MM/yyyy HH:mm");
            }

            // Shipping time
            var orderShippingTime = "Chờ giao hàng";
            if (order.Status == OrderStatus.Shipping)
            {
                orderShippingTime = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm");
            }
            else if (order.Status == OrderStatus.Completed)
            {
                var confirmTime = order.VerifiedAt ?? order.PaymentPaidAt ?? order.OrderDate.AddMinutes(5);
                orderShippingTime = confirmTime.AddHours(2).ToString("dd/MM/yyyy HH:mm");
            }

            // Completed time
            var orderCompletedTime = "Chờ hoàn thành";
            if (order.Status == OrderStatus.Completed)
            {
                orderCompletedTime = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm");
            }

            var buyer = WebUtility.HtmlEncode(parsedNotes.Buyer);
            var recipient = WebUtility.HtmlEncode(parsedNotes.Recipient);
            var greeting = WebUtility.HtmlEncode(parsedNotes.Greeting);
            var extraNotes = WebUtility.HtmlEncode(parsedNotes.ExtraNotes);
            var encodedAddress = WebUtility.HtmlEncode(fullAddress);
            var encodedTime = WebUtility.HtmlEncode(deliveryTime);
            var encodedCustomerName = WebUtility.HtmlEncode(customerName);

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'><style>");
            sb.AppendLine("body { font-family: 'Georgia', serif; background: #f5f2ed; color: #1a1a1a; padding: 40px 20px; }");
            sb.AppendLine(".container { max-width: 600px; margin: 0 auto; background: #fff; border: 1px solid #d4cfc7; }");
            sb.AppendLine(".header { background: #ab2c5d; color: #fff; padding: 30px; text-align: center; }");
            sb.AppendLine(".header h1 { margin: 0; font-size: 20px; letter-spacing: 2px; text-transform: uppercase; }");
            sb.AppendLine(".content { padding: 30px; }");
            sb.AppendLine(".order-id { font-size: 24px; font-weight: bold; color: #ab2c5d; margin: 10px 0; }");
            sb.AppendLine("table { width: 100%; border-collapse: collapse; margin: 20px 0; }");
            sb.AppendLine("th { background: #f5f2ed; padding: 10px; text-align: left; font-size: 11px; text-transform: uppercase; letter-spacing: 1px; }");
            sb.AppendLine("td { padding: 10px; border-bottom: 1px solid #e8e4dd; font-size: 14px; }");
            sb.AppendLine(".total { font-size: 18px; font-weight: bold; color: #ab2c5d; text-align: right; padding-top: 15px; }");
            sb.AppendLine(".footer { padding: 20px 30px; background: #f5f2ed; font-size: 12px; color: #666; text-align: center; }");
            sb.AppendLine(".section-title { font-size: 16px; font-weight: bold; color: #ab2c5d; border-bottom: 1px solid #ab2c5d; padding-bottom: 5px; margin-top: 25px; margin-bottom: 10px; text-transform: uppercase; letter-spacing: 1px; }");
            sb.AppendLine(".info-grid { width: 100%; margin-bottom: 20px; }");
            sb.AppendLine(".info-grid td { padding: 6px 0; border: none; font-size: 14px; }");
            sb.AppendLine(".info-label { font-weight: bold; color: #555; width: 35%; }");
            sb.AppendLine(".info-value { color: #1a1a1a; }");
            sb.AppendLine("</style></head><body>");
            sb.AppendLine("<div class='container'>");
            sb.AppendLine("<div class='header'><h1>Xác nhận đơn hàng</h1></div>");
            sb.AppendLine("<div class='content'>");
            sb.AppendLine($"<p>Kính gửi {encodedCustomerName},</p>");
            sb.AppendLine("<p>Cảm ơn bạn đã đặt hàng tại FlowerShop. Đơn hàng của bạn đã được ghi nhận và đang được xử lý.</p>");
            sb.AppendLine($"<div class='order-id'>Mã đơn hàng: #{order.Id}</div>");

            // Sections
            sb.AppendLine("<div class='section-title'>Thông tin khách hàng & Người nhận</div>");
            sb.AppendLine("<table class='info-grid'>");
            sb.AppendLine($"<tr><td class='info-label'>Người mua:</td><td class='info-value'>{buyer}</td></tr>");
            sb.AppendLine($"<tr><td class='info-label'>Người nhận:</td><td class='info-value'>{recipient}</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<div class='section-title'>Thông tin giao hàng</div>");
            sb.AppendLine("<table class='info-grid'>");
            sb.AppendLine($"<tr><td class='info-label'>Địa chỉ:</td><td class='info-value'>{encodedAddress}</td></tr>");
            sb.AppendLine($"<tr><td class='info-label'>Thời gian giao:</td><td class='info-value'>{encodedTime}</td></tr>");
            sb.AppendLine($"<tr><td class='info-label'>Thời gian đặt hàng:</td><td class='info-value'>{orderPlacedTime}</td></tr>");
            sb.AppendLine($"<tr><td class='info-label'>Thời gian xác nhận:</td><td class='info-value'>{orderConfirmedTime}</td></tr>");
            sb.AppendLine($"<tr><td class='info-label'>Thời gian đang giao:</td><td class='info-value'>{orderShippingTime}</td></tr>");
            sb.AppendLine($"<tr><td class='info-label'>Thời gian đã giao:</td><td class='info-value'>{orderCompletedTime}</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<div class='section-title'>Lời chúc thiệp</div>");
            sb.AppendLine($"<p style='margin: 10px 0; font-size: 14px;'>{greeting}</p>");

            sb.AppendLine("<div class='section-title'>Ghi chú</div>");
            sb.AppendLine($"<p style='margin: 10px 0; font-size: 14px;'>{extraNotes}</p>");

            sb.AppendLine("<div class='section-title'>Chi tiết đơn hàng</div>");
            sb.AppendLine("<table><thead><tr><th>Sản phẩm</th><th>Số lượng</th><th>Đơn giá</th><th>Thành tiền</th></tr></thead><tbody>");

            if (order.OrderDetails != null)
            {
                foreach (var detail in order.OrderDetails)
                {
                    var productName = detail.Product?.Name ?? $"Sản phẩm #{detail.ProductId}";
                    var lineTotal = detail.Quantity * detail.UnitPrice;
                    sb.AppendLine($"<tr><td>{WebUtility.HtmlEncode(productName)}</td><td>{detail.Quantity}</td><td>{detail.UnitPrice:N0}₫</td><td>{lineTotal:N0}₫</td></tr>");
                }
            }

            sb.AppendLine("</tbody></table>");
            var total = order.OrderDetails?.Sum(d => d.Quantity * d.UnitPrice) ?? 0;
            sb.AppendLine($"<div class='total'>Tổng cộng: {total:N0}₫</div>");
            var methodStr = order.PaymentMethod == PaymentMethod.COD ? "Thanh toán khi nhận hàng (COD)" : "Thanh toán trực tuyến (VNPAY)";
            sb.AppendLine($"<p><strong>Phương thức thanh toán:</strong> {methodStr}</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class='footer'>");
            sb.AppendLine("<p>FlowerShop — Trân trọng cảm ơn quý khách!</p>");
            sb.AppendLine("<p>Mọi thắc mắc xin vui lòng liên hệ: support@flowershop.com</p>");
            sb.AppendLine("</div></div></body></html>");
            return sb.ToString();
        }

        public async Task SendOrderConfirmedEmailAsync(Order order, string customerEmail, string customerName)
        {
            try
            {
                var statusText = $"Đơn hàng #{order.Id} đã được xác nhận và thợ cắm hoa đang tiến hành thiết kế hoa. Cảm ơn bạn!";
                var body = BuildOrderEmailBody(order, customerName, "Đơn hàng đã xác nhận", statusText);
                var senderEmail = !string.IsNullOrEmpty(_settings.SenderEmail) && !_settings.SenderEmail.Contains("noreply")
                    ? _settings.SenderEmail
                    : _settings.Username ?? "noreply@flowershop.com";
                using var message = new MailMessage
                {
                    From = new MailAddress(senderEmail, _settings.SenderName),
                    Subject = $"Đơn hàng #{order.Id} đã được xác nhận - FlowerShop",
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(customerEmail, customerName));

                using var client = CreateSmtpClient();

                await client.SendMailAsync(message);
                _logger.LogInformation("Order confirmed email sent for order {OrderId} to {Email}", order.Id, customerEmail);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send order confirmed email for order {OrderId}", order.Id);
            }
        }

        public async Task SendOrderShippingEmailAsync(Order order, string customerEmail, string customerName)
        {
            try
            {
                var statusText = $"Đơn hàng #{order.Id} của bạn đang được vận chuyển và sẽ sớm giao tới tay bạn. FlowerShop trân trọng cảm ơn!";
                var body = BuildOrderEmailBody(order, customerName, "Đơn hàng đang được giao", statusText);
                var senderEmail = !string.IsNullOrEmpty(_settings.SenderEmail) && !_settings.SenderEmail.Contains("noreply")
                    ? _settings.SenderEmail
                    : _settings.Username ?? "noreply@flowershop.com";
                using var message = new MailMessage
                {
                    From = new MailAddress(senderEmail, _settings.SenderName),
                    Subject = $"Đơn hàng #{order.Id} đang được giao - FlowerShop",
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(customerEmail, customerName));

                using var client = CreateSmtpClient();

                await client.SendMailAsync(message);
                _logger.LogInformation("Order shipping email sent for order {OrderId} to {Email}", order.Id, customerEmail);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send order shipping email for order {OrderId}", order.Id);
            }
        }

        public async Task SendOrderCompletedEmailAsync(Order order, string customerEmail, string customerName)
        {
            try
            {
                var statusText = $"Đơn hàng #{order.Id} của bạn đã được giao thành công. FlowerShop xin trân trọng cảm ơn quý khách!";
                var body = BuildOrderEmailBody(order, customerName, "Giao hàng thành công", statusText);
                var senderEmail = !string.IsNullOrEmpty(_settings.SenderEmail) && !_settings.SenderEmail.Contains("noreply")
                    ? _settings.SenderEmail
                    : _settings.Username ?? "noreply@flowershop.com";
                using var message = new MailMessage
                {
                    From = new MailAddress(senderEmail, _settings.SenderName),
                    Subject = $"Giao hàng thành công #{order.Id} - FlowerShop",
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(customerEmail, customerName));

                using var client = CreateSmtpClient();

                await client.SendMailAsync(message);
                _logger.LogInformation("Order completed email sent for order {OrderId} to {Email}", order.Id, customerEmail);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send order completed email for order {OrderId}", order.Id);
            }
        }

        private static string BuildOrderEmailBody(Order order, string customerName, string title, string statusText)
        {
            var parsedNotes = ParseNotes(order.Notes);

            var addressParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(order.DeliveryAddress)) addressParts.Add(order.DeliveryAddress.Trim());
            if (!string.IsNullOrWhiteSpace(order.DeliveryDistrict)) addressParts.Add(order.DeliveryDistrict.Trim());
            addressParts.Add("TP. Hồ Chí Minh");
            var fullAddress = string.Join(", ", addressParts);

            var timeSlot = !string.IsNullOrWhiteSpace(order.DeliveryTimeSlot) ? order.DeliveryTimeSlot.Trim() : "N/A";
            var dateStr = order.DeliveryDate?.ToString("dd/MM/yyyy") ?? "N/A";
            var deliveryTime = $"{timeSlot} ngày {dateStr}";

            var orderPlacedTime = order.OrderDate.ToString("dd/MM/yyyy HH:mm");

            // Confirmation time
            var orderConfirmedTime = "Chờ xác nhận";
            if (order.Status == OrderStatus.Confirmed || order.Status == OrderStatus.Preparing || order.Status == OrderStatus.Shipping || order.Status == OrderStatus.Completed || order.VerifiedAt.HasValue || order.PaymentPaidAt.HasValue)
            {
                var dt = order.VerifiedAt ?? order.PaymentPaidAt ?? order.OrderDate.AddMinutes(5);
                orderConfirmedTime = dt.ToString("dd/MM/yyyy HH:mm");
            }

            // Shipping time
            var orderShippingTime = "Chờ giao hàng";
            if (order.Status == OrderStatus.Shipping)
            {
                orderShippingTime = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm");
            }
            else if (order.Status == OrderStatus.Completed)
            {
                var confirmTime = order.VerifiedAt ?? order.PaymentPaidAt ?? order.OrderDate.AddMinutes(5);
                orderShippingTime = confirmTime.AddHours(2).ToString("dd/MM/yyyy HH:mm");
            }

            // Completed time
            var orderCompletedTime = "Chờ hoàn thành";
            if (order.Status == OrderStatus.Completed)
            {
                orderCompletedTime = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm");
            }

            var buyer = WebUtility.HtmlEncode(parsedNotes.Buyer);
            var recipient = WebUtility.HtmlEncode(parsedNotes.Recipient);
            var greeting = WebUtility.HtmlEncode(parsedNotes.Greeting);
            var extraNotes = WebUtility.HtmlEncode(parsedNotes.ExtraNotes);
            var encodedAddress = WebUtility.HtmlEncode(fullAddress);
            var encodedTime = WebUtility.HtmlEncode(deliveryTime);
            var encodedCustomerName = WebUtility.HtmlEncode(customerName);
            var encodedStatusText = WebUtility.HtmlEncode(statusText);
            var encodedTitle = WebUtility.HtmlEncode(title);

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'><style>");
            sb.AppendLine("body { font-family: 'Georgia', serif; background: #f5f2ed; color: #1a1a1a; padding: 40px 20px; }");
            sb.AppendLine(".container { max-width: 600px; margin: 0 auto; background: #fff; border: 1px solid #d4cfc7; }");
            sb.AppendLine(".header { background: #ab2c5d; color: #fff; padding: 30px; text-align: center; }");
            sb.AppendLine(".header h1 { margin: 0; font-size: 20px; letter-spacing: 2px; text-transform: uppercase; }");
            sb.AppendLine(".content { padding: 30px; }");
            sb.AppendLine(".order-id { font-size: 24px; font-weight: bold; color: #ab2c5d; margin: 10px 0; }");
            sb.AppendLine("table { width: 100%; border-collapse: collapse; margin: 20px 0; }");
            sb.AppendLine("th { background: #f5f2ed; padding: 10px; text-align: left; font-size: 11px; text-transform: uppercase; letter-spacing: 1px; }");
            sb.AppendLine("td { padding: 10px; border-bottom: 1px solid #e8e4dd; font-size: 14px; }");
            sb.AppendLine(".total { font-size: 18px; font-weight: bold; color: #ab2c5d; text-align: right; padding-top: 15px; }");
            sb.AppendLine(".footer { padding: 20px 30px; background: #f5f2ed; font-size: 12px; color: #666; text-align: center; }");
            sb.AppendLine(".section-title { font-size: 16px; font-weight: bold; color: #ab2c5d; border-bottom: 1px solid #ab2c5d; padding-bottom: 5px; margin-top: 25px; margin-bottom: 10px; text-transform: uppercase; letter-spacing: 1px; }");
            sb.AppendLine(".info-grid { width: 100%; margin-bottom: 20px; }");
            sb.AppendLine(".info-grid td { padding: 6px 0; border: none; font-size: 14px; }");
            sb.AppendLine(".info-label { font-weight: bold; color: #555; width: 35%; }");
            sb.AppendLine(".info-value { color: #1a1a1a; }");
            sb.AppendLine("</style></head><body>");
            sb.AppendLine("<div class='container'>");
            sb.AppendLine($"<div class='header'><h1>{encodedTitle}</h1></div>");
            sb.AppendLine("<div class='content'>");
            sb.AppendLine($"<p>Kính gửi {encodedCustomerName},</p>");
            sb.AppendLine($"<p>{encodedStatusText}</p>");
            sb.AppendLine($"<div class='order-id'>Mã đơn hàng: #{order.Id}</div>");

            // Sections
            sb.AppendLine("<div class='section-title'>Thông tin khách hàng & Người nhận</div>");
            sb.AppendLine("<table class='info-grid'>");
            sb.AppendLine($"<tr><td class='info-label'>Người mua:</td><td class='info-value'>{buyer}</td></tr>");
            sb.AppendLine($"<tr><td class='info-label'>Người nhận:</td><td class='info-value'>{recipient}</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<div class='section-title'>Thông tin giao hàng</div>");
            sb.AppendLine("<table class='info-grid'>");
            sb.AppendLine($"<tr><td class='info-label'>Địa chỉ:</td><td class='info-value'>{encodedAddress}</td></tr>");
            sb.AppendLine($"<tr><td class='info-label'>Thời gian giao:</td><td class='info-value'>{encodedTime}</td></tr>");
            sb.AppendLine($"<tr><td class='info-label'>Thời gian đặt hàng:</td><td class='info-value'>{orderPlacedTime}</td></tr>");
            sb.AppendLine($"<tr><td class='info-label'>Thời gian xác nhận:</td><td class='info-value'>{orderConfirmedTime}</td></tr>");
            sb.AppendLine($"<tr><td class='info-label'>Thời gian đang giao:</td><td class='info-value'>{orderShippingTime}</td></tr>");
            sb.AppendLine($"<tr><td class='info-label'>Thời gian đã giao:</td><td class='info-value'>{orderCompletedTime}</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<div class='section-title'>Lời chúc thiệp</div>");
            sb.AppendLine($"<p style='margin: 10px 0; font-size: 14px;'>{greeting}</p>");

            sb.AppendLine("<div class='section-title'>Ghi chú</div>");
            sb.AppendLine($"<p style='margin: 10px 0; font-size: 14px;'>{extraNotes}</p>");

            sb.AppendLine("<div class='section-title'>Chi tiết đơn hàng</div>");
            sb.AppendLine("<table><thead><tr><th>Sản phẩm</th><th>Số lượng</th><th>Đơn giá</th><th>Thành tiền</th></tr></thead><tbody>");

            if (order.OrderDetails != null)
            {
                foreach (var detail in order.OrderDetails)
                {
                    var productName = detail.Product?.Name ?? $"Sản phẩm #{detail.ProductId}";
                    var lineTotal = detail.Quantity * detail.UnitPrice;
                    sb.AppendLine($"<tr><td>{WebUtility.HtmlEncode(productName)}</td><td>{detail.Quantity}</td><td>{detail.UnitPrice:N0}₫</td><td>{lineTotal:N0}₫</td></tr>");
                }
            }

            sb.AppendLine("</tbody></table>");
            var total = order.OrderDetails?.Sum(d => d.Quantity * d.UnitPrice) ?? 0;
            sb.AppendLine($"<div class='total'>Tổng cộng: {total:N0}₫</div>");
            var methodStr = order.PaymentMethod == PaymentMethod.COD ? "Thanh toán khi nhận hàng (COD)" : "Thanh toán trực tuyến (VNPAY)";
            sb.AppendLine($"<p><strong>Phương thức thanh toán:</strong> {methodStr}</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class='footer'>");
            sb.AppendLine("<p>FlowerShop — Trân trọng cảm ơn quý khách!</p>");
            sb.AppendLine("<p>Mọi thắc mắc xin vui lòng liên hệ: pdahoctap@gmail.com</p>");
            sb.AppendLine("</div></div></body></html>");
            return sb.ToString();
        }

        private class ParsedNotes
        {
            public string Buyer { get; set; } = "N/A";
            public string Recipient { get; set; } = "N/A";
            public string Greeting { get; set; } = "Không có";
            public string ExtraNotes { get; set; } = "Không có";
        }

        private static ParsedNotes ParseNotes(string? notes)
        {
            var result = new ParsedNotes();
            if (string.IsNullOrEmpty(notes)) return result;

            var parts = notes.Split('|');
            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                if (trimmed.StartsWith("Người mua:", StringComparison.OrdinalIgnoreCase))
                {
                    result.Buyer = trimmed.Replace("Người mua:", "", StringComparison.OrdinalIgnoreCase).Trim();
                }
                else if (trimmed.StartsWith("Người nhận:", StringComparison.OrdinalIgnoreCase))
                {
                    result.Recipient = trimmed.Replace("Người nhận:", "", StringComparison.OrdinalIgnoreCase).Trim();
                }
                else if (trimmed.StartsWith("Lời chúc:", StringComparison.OrdinalIgnoreCase))
                {
                    result.Greeting = trimmed.Replace("Lời chúc:", "", StringComparison.OrdinalIgnoreCase).Trim();
                }
                else if (trimmed.StartsWith("Ghi chú thêm:", StringComparison.OrdinalIgnoreCase))
                {
                    result.ExtraNotes = trimmed.Replace("Ghi chú thêm:", "", StringComparison.OrdinalIgnoreCase).Trim();
                }
            }
            return result;
        }

        public async Task SendResetPasswordEmailAsync(string email, string name, string resetLink, string? rawToken = null)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'><style>");
                sb.AppendLine("body { font-family: 'Georgia', serif; background: #f5f2ed; color: #1a1a1a; padding: 40px 20px; }");
                sb.AppendLine(".container { max-width: 500px; margin: 0 auto; background: #fff; border: 1px solid #d4cfc7; }");
                sb.AppendLine(".header { background: #ab2c5d; color: #fff; padding: 30px; text-align: center; }");
                sb.AppendLine(".header h1 { margin: 0; font-size: 20px; letter-spacing: 2px; text-transform: uppercase; }");
                sb.AppendLine(".content { padding: 30px; text-align: center; font-size: 15px; line-height: 1.6; }");
                sb.AppendLine(".btn { display: inline-block; background: #ab2c5d; color: #fff; padding: 12px 24px; text-decoration: none; font-weight: bold; margin: 20px 0; letter-spacing: 1px; }");
                sb.AppendLine(".footer { padding: 20px; background: #f5f2ed; text-align: center; font-size: 11px; color: #666; }");
                sb.AppendLine("</style></head><body>");
                sb.AppendLine("<div class='container'>");
                sb.AppendLine("<div class='header'><h1>FlowerShop</h1></div>");
                sb.AppendLine("<div class='content'>");
                sb.AppendLine($"<p>Xin chào <strong>{WebUtility.HtmlEncode(name)}</strong>,</p>");
                sb.AppendLine("<p>Bạn đã yêu cầu đặt lại mật khẩu cho tài khoản của mình tại FlowerShop.</p>");

                if (!string.IsNullOrEmpty(rawToken))
                {
                    sb.AppendLine("<p>Vui lòng sử dụng mã dưới đây để đặt lại mật khẩu (mã có giá trị 15 phút):</p>");
                    sb.AppendLine($"<div style='background:#f5f2ed;padding:16px;font-family:monospace;font-size:18px;letter-spacing:4px;font-weight:bold;margin:16px 0'>{WebUtility.HtmlEncode(rawToken)}</div>");
                    sb.AppendLine($"<p>Hoặc bấm vào nút bên dưới để mở trang đặt lại mật khẩu:</p>");
                }
                else
                {
                    sb.AppendLine("<p>Vui lòng bấm vào nút bên dưới để tiến hành thiết lập mật khẩu mới (liên kết có giá trị trong vòng 15 phút):</p>");
                }

                sb.AppendLine($"<a class='btn' href='{resetLink}'>ĐẶT LẠI MẬT KHẨU</a>");
                sb.AppendLine("<p style='color: #888; font-size: 12px;'>Nếu bạn không yêu cầu hành động này, vui lòng bỏ qua email.</p>");
                sb.AppendLine("</div>");
                sb.AppendLine("<div class='footer'><p>© 2026 FlowerShop. All rights reserved.</p></div>");
                sb.AppendLine("</div></body></html>");

                var body = sb.ToString();
                var senderEmail = !string.IsNullOrEmpty(_settings.SenderEmail) && !_settings.SenderEmail.Contains("noreply")
                    ? _settings.SenderEmail
                    : _settings.Username ?? "noreply@flowershop.com";

                using var message = new MailMessage
                {
                    From = new MailAddress(senderEmail, _settings.SenderName),
                    Subject = "Đặt lại mật khẩu của bạn - FlowerShop",
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(email, name));

                using var client = CreateSmtpClient();
                await client.SendMailAsync(message);
                _logger.LogInformation("Password reset email sent to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
                throw;
            }
        }

        public async Task SendOtpEmailAsync(string email, string name, string otp)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'><style>");
                sb.AppendLine("body { font-family: 'Georgia', serif; background: #f5f2ed; color: #1a1a1a; padding: 40px 20px; }");
                sb.AppendLine(".container { max-width: 500px; margin: 0 auto; background: #fff; border: 1px solid #d4cfc7; text-align: center; }");
                sb.AppendLine(".header { background: #ab2c5d; color: #fff; padding: 30px; }");
                sb.AppendLine(".header h1 { margin: 0; font-size: 20px; letter-spacing: 2px; text-transform: uppercase; }");
                sb.AppendLine(".otp { font-size: 36px; font-weight: bold; color: #ab2c5d; letter-spacing: 8px; margin: 30px 0; }");
                sb.AppendLine(".content { padding: 30px; font-size: 15px; line-height: 1.6; }");
                sb.AppendLine(".footer { padding: 20px; background: #f5f2ed; font-size: 11px; color: #666; }");
                sb.AppendLine("</style></head><body>");
                sb.AppendLine("<div class='container'>");
                sb.AppendLine("<div class='header'><h1>FlowerShop</h1></div>");
                sb.AppendLine("<div class='content'>");
                sb.AppendLine($"<p>Xin chào <strong>{WebUtility.HtmlEncode(name)}</strong>,</p>");
                sb.AppendLine("<p>Mã xác thực đơn hàng của bạn là:</p>");
                sb.AppendLine($"<div class='otp'>{WebUtility.HtmlEncode(otp)}</div>");
                sb.AppendLine("<p>Vui lòng nhập mã này để xác nhận đơn hàng. Mã có hiệu lực trong 10 phút.</p>");
                sb.AppendLine("<p style='color: #888; font-size: 12px;'>Nếu bạn không thực hiện đặt hàng, vui lòng bỏ qua email này.</p>");
                sb.AppendLine("</div>");
                sb.AppendLine("<div class='footer'><p>© 2026 FlowerShop. All rights reserved.</p></div>");
                sb.AppendLine("</div></body></html>");

                var body = sb.ToString();
                var senderEmail = !string.IsNullOrEmpty(_settings.SenderEmail) && !_settings.SenderEmail.Contains("noreply")
                    ? _settings.SenderEmail
                    : _settings.Username ?? "noreply@flowershop.com";

                using var message = new MailMessage
                {
                    From = new MailAddress(senderEmail, _settings.SenderName),
                    Subject = "Mã xác thực đơn hàng - FlowerShop",
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(email, name));

                using var client = CreateSmtpClient();
                await client.SendMailAsync(message);
                _logger.LogInformation("OTP email sent to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email to {Email}", email);
                throw;
            }
        }
    }
}
