using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

namespace PlatformService
{
    public class Startup
    {
        //Конструктор отримує IConfiguration та IWebHostEnvironment, які будуть використовуватися для доступу до конфігурації додатка та інформації про середовище.
        public IConfiguration Configuration { get; }

        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }
        //Метод ConfigureServices для налаштування сервісів
        public void ConfigureServices(IServiceCollection services)
        {
            // В залежності від середовища використовується SQL Server або InMemory база даних
            if (_env.IsProduction())
            {
                Console.WriteLine("--> Using SQLServer Db");
                services.AddDbContext<AppDbContext>(opt =>
                    opt.UseSqlServer(Configuration.GetConnectionString("PlatformsConn")));
            }
            else
            {
                Console.WriteLine("--> Using InMem Db");
                services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
            }

            // Додавання сервісу репозиторію та інших сервісів
            services.AddScoped<IPlatformRepo, PlatformRepo>();
            services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
            services.AddSingleton<IMessageBusClient, MessageBusClient>();

            // Додавання Grpc та інших необхідних сервісів
            services.AddGrpc();
            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            // Налаштування Swagger для документації API
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
            });
            // Виведення інформації про CommandService Endpoint
            Console.WriteLine($"--> CommandService Endpoint {Configuration["CommandService"]}");
        }

        //Метод Configure для конфігурації middleware та обробки HTTP-запитів
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Конфігурація middleware для різних середовищ
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
            }
            // Конфігурація routing та авторизації
            app.UseRouting();
            app.UseAuthorization();
            // Конфігурація endpoints для контролерів та Grpc сервісу
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<GrpcPlatformService>();
                // Реалізація маршруту для доступу до файла "platforms.proto"
                endpoints.MapGet("/protos/platforms.proto", async context =>
                {
                    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
                });
            });
            // Підготовка бази даних (використовується метод з класу PrepDb)
            PrepDb.PrepPopulation(app, env.IsProduction());
        }
    }
}
