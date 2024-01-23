using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PlatformService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Створення та запуск хоста
            CreateHostBuilder(args).Build().Run();
        }
        // Метод для створення IHostBuilder
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Вказуємо тип Startup, який буде використовуватися
                    webBuilder.UseStartup<Startup>();
                });
    }
}
