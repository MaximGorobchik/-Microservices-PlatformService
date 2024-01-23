using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        // Конструктор, який отримує HttpClient та IConfiguration
        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        // Метод для відправлення платформи до CommandService
        public async Task SendPlatformToCommand(PlatformReadDto plat)
        {
            // Створення HTTP-запиту з об'єктом платформи у форматі JSON
            var httpContent = new StringContent(
                JsonSerializer.Serialize(plat),
                Encoding.UTF8,
                "application/json"
            );

            // Відправлення POST-запиту до CommandService
            var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}", httpContent);


            // Перевірка успішності відправлення і виведення відповідного повідомлення
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Sync POST to CommandService was OK!");
            }
            else
            {
                Console.WriteLine("--> Sync POST to CommandService was  NOT OK!");
            }
        }
    }
}