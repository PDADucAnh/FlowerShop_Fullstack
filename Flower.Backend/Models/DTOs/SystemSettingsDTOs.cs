using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class StoreInfoSettings
    {
        [Required(ErrorMessage = "Tên cửa hàng không được để trống")]
        public string StoreName { get; set; } = "FlowerShop";
        public string Logo { get; set; } = "/images/logo.png";
        [Required(ErrorMessage = "Hotline không được để trống")]
        public string Hotline { get; set; } = "0123456789";
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = "contact@flowershop.com";
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string Address { get; set; } = "123 Đường Hoa, Quận 1, TP. HCM";
        public string Facebook { get; set; } = "https://facebook.com/flowershop";
        public string Zalo { get; set; } = "0123456789";
        public string OpenHours { get; set; } = "08:00 - 21:00";
    }

    public class SmtpSettings
    {
        [Required(ErrorMessage = "SMTP Host không được để trống")]
        public string Host { get; set; } = "smtp.gmail.com";
        [Range(1, 65535, ErrorMessage = "SMTP Port phải từ 1 đến 65535")]
        public int Port { get; set; } = 587;
        [Required(ErrorMessage = "Username không được để trống")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password không được để trống")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Tên người gửi không được để trống")]
        public string SenderName { get; set; } = "FlowerShop";
        [Required(ErrorMessage = "Email người gửi không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string SenderEmail { get; set; } = "noreply@flowershop.com";
    }

    public class VNPaySettings
    {
        [Required(ErrorMessage = "TmnCode không được để trống")]
        public string TmnCode { get; set; } = string.Empty;
        [Required(ErrorMessage = "Hash Secret không được để trống")]
        public string HashSecret { get; set; } = string.Empty;
        [Required(ErrorMessage = "ReturnUrl không được để trống")]
        public string ReturnUrl { get; set; } = string.Empty;
        public bool IsSandbox { get; set; } = true;
        public bool EnablePayment { get; set; } = true;
    }

    public class ShippingSettings
    {
        [Range(0, 1000000, ErrorMessage = "Phí giao hàng phải từ 0 trở lên")]
        public decimal DefaultFee { get; set; } = 30000;
        [Range(0, 10000000, ErrorMessage = "Ngưỡng miễn phí ship phải từ 0 trở lên")]
        public decimal FreeShipFrom { get; set; } = 500000;
        [Range(0, 1000, ErrorMessage = "Khoảng cách giao tối đa phải từ 0 trở lên")]
        public double MaxDistance { get; set; } = 20; // km
        public string DeliveryTime { get; set; } = "2-4 giờ";
    }

    public class OrderSettings
    {
        [Range(1, 1440, ErrorMessage = "Thời gian tự động hủy phải từ 1 đến 1440 phút")]
        public int AutoCancelMinutes { get; set; } = 30;
        public bool EnableCOD { get; set; } = true;
        public bool EnableOnlinePayment { get; set; } = true;
    }

    public class AllSystemSettingsViewModel
    {
        public StoreInfoSettings Store { get; set; } = new();
        public SmtpSettings Smtp { get; set; } = new();
        public VNPaySettings VNPay { get; set; } = new();
        public ShippingSettings Shipping { get; set; } = new();
        public OrderSettings Order { get; set; } = new();
    }
}
