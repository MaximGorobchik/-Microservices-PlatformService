using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        //����� PrepPopulation ������� SeedData � ������ ������ �� ������ ��������� AppDbContext �� ����� ����������
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using( var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
            }
        }
        //����� SeedData ��������, �� ������� �������� ������� � ����� ����������.��� ��������, �� ���� ����� ��� ���������.
        //���� �, �� ���� �������� ��� �� ������ �� � ���� �����
        private static void SeedData(AppDbContext context, bool isProd)
        {
            // ������ SeedData � ������������� ��������� ������ �� �������� DbContext �� ������ ����������
            if (isProd)
            {
                Console.WriteLine("--> Attempting to apply migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"--> Could not run migrations: {ex.Message}");
                }
                
            }
            // ��������, �� ���� ����� ��� ���������
            if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data...");
                // ��������� ���������� �����, ���� ���� ����� �������
                context.Platforms.AddRange(
                    new Platform() {Name="Dot Net", Publisher="Microsoft", Cost="Free"},
                    new Platform() {Name="SQL Server Express", Publisher="Microsoft", Cost="Free"},
                    new Platform() {Name="Kubernetes", Publisher="Cloud Native Computing Foundation", Cost="Free"}
                );
                // ���������� ��� �� ���� �����
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
    }
}