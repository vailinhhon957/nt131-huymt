using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Data;
using Server.Models;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/smoke")]
    public class SmokeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<MessHub> _hubContext;

        public SmokeController(AppDbContext context, IHubContext<MessHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public class SmokeResponse
        {
            public string Message { get; set; }
            public string Status { get; set; }
            public double SmokeLevel { get; set; }
            public DateTime Timestamp { get; set; }
            public bool Alert { get; set; } = false;
            public string AlertMessage { get; set; } = null;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveSmoke([FromBody] SmokeLog data)
        {
            if (data == null || data.SmokeLevel < 0)
                return BadRequest("Dữ liệu không hợp lệ");

            data.Timestamp = DateTime.Now;
            _context.SmokeLogs.Add(data);
            await _context.SaveChangesAsync();

            bool isAlert = data.SmokeLevel > 150;
            var status = isAlert ? "CẢNH BÁO KHÓI CAO" : "Bình thường";

            if (isAlert)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveAlert", new
                {
                    Smoke = true,
                    SmokeLevel = data.SmokeLevel,
                    Message = "Phát hiện khói! Vui lòng kiểm tra ngay!"
                });
                Debug.WriteLine("Đã gửi cảnh báo khói qua SignalR");
            }

            return Ok(new SmokeResponse
            {
                Message = "Mức khói đã được lưu",
                Status = status,
                SmokeLevel = data.SmokeLevel,
                Timestamp = data.Timestamp,
                Alert = isAlert,
                AlertMessage = isAlert ? $"Mức khói vượt ngưỡng: {data.SmokeLevel}. Vui lòng kiểm tra ngay!" : null
            });
        }

        [HttpGet]
        public IActionResult GetSmokeLogs()
        {
            var allLogs = _context.SmokeLogs
                .OrderByDescending(s => s.Timestamp)
                .ToList();

            return Ok(allLogs);
        }

        [HttpGet("latest")]
        public IActionResult GetLatestSmoke()
        {
            try
            {
                var latest = _context.SmokeLogs
                    .OrderByDescending(s => s.Timestamp)
                    .FirstOrDefault();

                if (latest == null)
                {
                    return Ok(new
                    {
                        smokeLevel = 0,
                        timestamp = DateTime.Now,
                        status = "NO DATA"
                    });
                }

                return Ok(new
                {
                    smokeLevel = latest.SmokeLevel,
                    timestamp = latest.Timestamp
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    smokeLevel = 0,
                    timestamp = DateTime.Now,
                    status = "ERROR",
                    message = ex.Message
                });
            }
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetSmokeStatus()
        {
            try
            {
                var latest = _context.SmokeLogs
                    .OrderByDescending(s => s.Timestamp)
                    .FirstOrDefault();

                if (latest == null)
                {
                    return Ok(new
                    {
                        status = "NO DATA",
                        smokeLevel = 0,
                        timestamp = DateTime.Now
                    });
                }

                string status = latest.SmokeLevel > 150 ? "FIRE ALERT" : "NORMAL";

                if (latest.SmokeLevel > 150)
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveAlert", new
                    {
                        Smoke = true,
                        SmokeLevel = latest.SmokeLevel,
                        Message = "Phát hiện khói! Vui lòng kiểm tra ngay!"
                    });
                    Console.WriteLine("Đã gửi cảnh báo khói qua SignalR");
                }

                return Ok(new
                {
                    status,
                    smokeLevel = latest.SmokeLevel,
                    timestamp = latest.Timestamp
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "ERROR",
                    smokeLevel = 0,
                    timestamp = DateTime.Now,
                    message = ex.Message
                });
            }
        }
    }
}
