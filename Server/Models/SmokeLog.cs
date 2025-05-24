using System;
namespace Server.Models
{
    public class SmokeLog
    {
        public int Id { get; set; }
        public double SmokeLevel { get; set; } // Mức độ khói
        public DateTime Timestamp { get; set; } // Thời gian ghi nhận
        public string Status { get; set; } // Trạng thái cảnh báo
        public bool Alert { get; set; } = false; // Cảnh báo
        public string AlertMessage { get; set; } = null; // Thông điệp cảnh báo
    }
}
