using Microsoft.EntityFrameworkCore;
using Server.Models; // Đảm bảo đúng namespace của bạn

namespace Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TemperatureLog> TemperatureLogs { get; set; }
        public DbSet<SmokeLog> SmokeLogs { get; set; } // Thêm DbSet cho SmokeLog
    }
}
