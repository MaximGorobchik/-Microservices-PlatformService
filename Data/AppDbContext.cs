using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class AppDbContext : DbContext
    {
        // Конструктор, який приймає параметри конфігурації DbContext
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
            
        }
        // Властивість, яка представляє DbSet для роботи з таблицею платформ в базі даних
        public DbSet<Platform> Platforms { get; set; }
    }
}