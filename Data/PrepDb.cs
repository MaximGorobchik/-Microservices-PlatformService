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
        //Метод PrepPopulation викликає SeedData з області служби та отримує екземпляр AppDbContext та режим продакшену
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using( var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
            }
        }
        //Метод SeedData перевіряє, чи потрібно виконати міграції у режимі продакшену.Далі перевіряє, чи база даних вже заповнена.
        //Якщо ні, то додає початкові дані та зберігає їх у базу даних
        private static void SeedData(AppDbContext context, bool isProd)
        {
            // Виклик SeedData з використанням сервісного області та поданням DbContext та режиму продакшену
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
            // Перевірка, чи база даних вже заповнена
            if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data...");
                // Додавання початкових даних, якщо база даних порожня
                context.Platforms.AddRange(
                    new Platform() {Name="Dot Net", Publisher="Microsoft", Cost="Free"},
                    new Platform() {Name="SQL Server Express", Publisher="Microsoft", Cost="Free"},
                    new Platform() {Name="Kubernetes", Publisher="Cloud Native Computing Foundation", Cost="Free"}
                );
                // Збереження змін до бази даних
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
    }
}