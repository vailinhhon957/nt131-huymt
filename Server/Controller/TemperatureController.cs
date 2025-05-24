using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Controllers;
using Server.Data;
using Server.Models;
using Server.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/temperature")]
    public class TemperatureController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<MessHub> _hubContext;
        public TemperatureController(AppDbContext context,IHubContext<MessHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        public class TemperatureResponse
        {
            public string Message { get; set; }
            public string Status { get; set; }
            public double Temperature { get; set; }
            public DateTime Timestamp { get; set; }
            public bool Alert { get; set; } = false; // Cảnh báo
            public string AlertMessage { get; set; } = null; // Thông điệp cảnh báo
        }
        [HttpPost]
        public async Task<IActionResult> ReceiveTemperature([FromBody] TemperatureLog data)
        {
            if (data == null || data.Temperature <= 0)
                return BadRequest("Dữ liệu không hợp lệ");

            _context.TemperatureLogs.Add(data);
            await _context.SaveChangesAsync();
            string status = data.Temperature > 70 ? "CẢNH BÁO NHIỆT ĐỘ CAO" : "OK";
            if (data.Temperature > 25)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveAlert", new
                {
                    Temperature = data.Temperature,
                    Message = "Cảnh báo: Nhiệt độ vượt ngưỡng!"
                });
            }
            bool isAlert = data.Temperature > 25;
            return Ok(new TemperatureResponse
            {
                Message = "Nhiệt độ đã được lưu",
                Status = status,
                Temperature = data.Temperature,
                Timestamp = data.Timestamp,
                Alert = isAlert,
                AlertMessage = isAlert ? $"Nhiệt độ vượt ngưỡng: {data.Temperature}°C. Vui lòng kiểm tra ngay!" : null
            });
        }

        [HttpGet]
        public IActionResult GetTemperatures()
        {
            var allLogs = _context.TemperatureLogs
                .OrderByDescending(t => t.Timestamp)
                .ToList();
            return Ok(allLogs);
        }

        [HttpGet("latest")]
        public IActionResult GetLatestTemperature()
        {
            var latest = _context.TemperatureLogs
                .OrderByDescending(t => t.Timestamp)
                .FirstOrDefault();

            if (latest == null)
                return NotFound("Chưa có dữ liệu");
          
            return Ok(new { temperature = latest.Temperature, timestamp = latest.Timestamp });
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var latest = _context.TemperatureLogs
                .OrderByDescending(t => t.Timestamp)
                .FirstOrDefault();

            if (latest == null)
                return NotFound("Chưa có dữ liệu");

            string status = latest.Temperature > 60 ? "FIRE ALERT" : "NORMAL";


            return Ok(new
            {
                Status = status,
                temperature = latest.Temperature,
                timestamp = latest.Timestamp
            });
        }
        
    }
}
