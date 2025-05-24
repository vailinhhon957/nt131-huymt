namespace Server.Models
{
    public class TemperatureLog
    {
        public int Id { get; set; }
        public double Temperature { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Status { get; set; } = "OK";
        public bool Alert { get; set; } = false;
        public string AlertMessage { get; set; } = null; // Thông điệp cảnh báo
    }
}
