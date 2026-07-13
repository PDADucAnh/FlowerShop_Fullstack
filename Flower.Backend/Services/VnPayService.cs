using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace Flower.Backend.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly ISystemSettingService _settingService;

        public VnPayService(IConfiguration configuration)
            : this(configuration, null!)
        {
        }

        public VnPayService(IConfiguration configuration, ISystemSettingService settingService)
        {
            _configuration = configuration;
            _settingService = settingService;
        }

        private VNPaySettings GetVNPaySettings()
        {
            if (_settingService != null)
            {
                var vnpay = _settingService.GetSetting<VNPaySettings>("VNPay").GetAwaiter().GetResult();
                if (vnpay != null && !string.IsNullOrEmpty(vnpay.TmnCode))
                {
                    return vnpay;
                }
            }

            var configSection = _configuration.GetSection("Vnpay");
            return new VNPaySettings
            {
                TmnCode = configSection["TmnCode"] ?? "",
                HashSecret = configSection["HashSecret"] ?? "",
                ReturnUrl = configSection["PaymentBackReturnUrl"] ?? "",
                IsSandbox = true,
                EnablePayment = true
            };
        }

        public string CreatePaymentUrl(VnPaymentRequestModel model, HttpContext context)
        {
            var vnpay = GetVNPaySettings();
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"] ?? "SE Asia Standard Time");
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();

            var pay = new VnPayLibrary();
            var urlCallBack = vnpay.ReturnUrl;

            pay.AddRequestData("vnp_Version", "2.1.0");
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", vnpay.TmnCode);
            pay.AddRequestData("vnp_Amount", ((int)(model.Amount * 100)).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_IpAddr", VnPayLibrary.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", "vn");
            pay.AddRequestData("vnp_OrderInfo", $"Thanh toán đơn hàng {model.OrderId}");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack ?? "");
            pay.AddRequestData("vnp_TxnRef", $"{model.OrderId}_{model.PaymentId}");
            pay.AddRequestData("vnp_ExpireDate", timeNow.AddMinutes(15).ToString("yyyyMMddHHmmss"));

            var baseUrl = vnpay.IsSandbox 
                ? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html" 
                : "https://pay.vnpayment.vn/vpcpay.html";

            return pay.CreateRequestUrl(baseUrl, vnpay.HashSecret);
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnpay = GetVNPaySettings();
            var pay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    pay.AddResponseData(key, value);
                }
            }

            var txnRef = pay.GetResponseData("vnp_TxnRef");
            var orderId = txnRef.Contains("_") ? txnRef.Split('_')[0] : txnRef;
            var vnPayTranId = pay.GetResponseData("vnp_TransactionNo");
            var vnpResponseCode = pay.GetResponseData("vnp_ResponseCode");
            var vnpSecureHash = collections.FirstOrDefault(k => k.Key == "vnp_SecureHash").Value;
            var orderInfo = pay.GetResponseData("vnp_OrderInfo");
            var hashSecret = vnpay.HashSecret;

            var checkSignature = pay.ValidateSignature(vnpSecureHash, hashSecret);

            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false,
                    VnPayResponseCode = vnpResponseCode
                };
            }

            var rawAmount = pay.GetResponseData("vnp_Amount");
            decimal.TryParse(rawAmount, out var amountInVnd);
            amountInVnd /= 100;

            return new VnPaymentResponseModel
            {
                Success = vnpResponseCode == "00",
                PaymentMethod = "VnPay",
                OrderDescription = orderInfo,
                OrderId = orderId,
                PaymentId = vnPayTranId,
                TransactionId = vnPayTranId,
                Token = vnpSecureHash,
                VnPayResponseCode = vnpResponseCode,
                Amount = amountInVnd
            };
        }
    }
}
