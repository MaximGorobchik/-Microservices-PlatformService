using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class AppDbContext : DbContext
    {
        // �����������, ���� ������ ��������� ������������ DbContext
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
            
        }
        // ����������, ��� ����������� DbSet ��� ������ � �������� �������� � ��� �����
        public DbSet<Platform> Platforms { get; set; }
    }
}