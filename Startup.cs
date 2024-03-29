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
        //����������� ������ IConfiguration �� IWebHostEnvironment, �� ������ ����������������� ��� ������� �� ������������ ������� �� ���������� ��� ����������.
        public IConfiguration Configuration { get; }

        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }
        //����� ConfigureServices ��� ������������ ������
        public void ConfigureServices(IServiceCollection services)
        {
            // � ��������� �� ���������� ��������������� SQL Server ��� InMemory ���� �����
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

            // ��������� ������ ���������� �� ����� ������
            services.AddScoped<IPlatformRepo, PlatformRepo>();
            services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
            services.AddSingleton<IMessageBusClient, MessageBusClient>();

            // ��������� Grpc �� ����� ���������� ������
            services.AddGrpc();
            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            // ������������ Swagger ��� ������������ API
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
            });
            // ��������� ���������� ��� CommandService Endpoint
            Console.WriteLine($"--> CommandService Endpoint {Configuration["CommandService"]}");
        }

        //����� Configure ��� ������������ middleware �� ������� HTTP-������
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // ������������ middleware ��� ����� ���������
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
            }
            // ������������ routing �� �����������
            app.UseRouting();
            app.UseAuthorization();
            // ������������ endpoints ��� ���������� �� Grpc ������
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<GrpcPlatformService>();
                // ��������� �������� ��� ������� �� ����� "platforms.proto"
                endpoints.MapGet("/protos/platforms.proto", async context =>
                {
                    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
                });
            });
            // ϳ�������� ���� ����� (��������������� ����� � ����� PrepDb)
            PrepDb.PrepPopulation(app, env.IsProduction());
        }
    }
}
