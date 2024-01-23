using System.Collections.Generic;
using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepo
    {
        // Метод для збереження змін до бази даних
        bool SaveChanges();
        // Метод для отримання всіх платформ
        IEnumerable<Platform> GetAllPlatforms();
        // Метод для отримання платформи за ідентифікатором
        Platform GetPlatformById(int id);
        // Метод для додавання нової платформи
        void CreatePlatform(Platform plat);
    }
}