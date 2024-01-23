using System;
using System.Collections.Generic;
using System.Linq;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private readonly AppDbContext _context;
        // Конструктор, який отримує контекст бази даних
        public PlatformRepo(AppDbContext context)
        {
            _context = context;
        }



        // Метод для додавання нової платформи
        public void CreatePlatform(Platform plat)
        {
            if(plat == null)
            {
                throw new ArgumentNullException(nameof(plat));
            }

            _context.Platforms.Add(plat);
        }
        // Метод для отримання всіх платформ
        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }
        // Метод для отримання платформи за ідентифікатором
        public Platform GetPlatformById(int id)
        {
            return _context.Platforms.FirstOrDefault(p => p.Id == id);
        }
        // Метод для збереження змін до бази даних
        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}