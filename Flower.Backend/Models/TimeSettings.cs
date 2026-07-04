namespace Flower.Backend.Models
{
    public class TimeSettings
    {
        public string TimeZone { get; set; } = "Asia/Ho_Chi_Minh";
        public int LeadTimeHours { get; set; } = 2;
        public int MaxOrdersPerSlot { get; set; } = 10;
        public int PreShippingMinutes { get; set; } = 30;
    }
}
