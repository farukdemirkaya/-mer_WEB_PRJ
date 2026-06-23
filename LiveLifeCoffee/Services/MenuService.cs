// =========================================================
// Services/MenuService.cs
// Menüdeki kahveleri ve atıştırmalıkları veritabanı üzerinden yönetir.
// =========================================================

using LiveLifeCoffee.Models;
using LiveLifeCoffee.Data;
using System.Collections.Generic;
using System.Linq;

namespace LiveLifeCoffee.Services
{
    public class MenuService
    {
        private readonly AppDbContext _context;

        public MenuService(AppDbContext context)
        {
            _context = context;
        }

        // Tüm aktif (satışa açık) menü ürünlerini listeler
        public List<MenuItem> GetAllActive()
        {
            return _context.MenuItems
                .Where(m => m.IsAvailable)
                .ToList();
        }

        // İster aktif ister pasif olsun, veritabanındaki tüm ürünleri getirir (Admin için)
        public List<MenuItem> GetAll()
        {
            return _context.MenuItems.ToList();
        }

        // Menüdeki mevcut kategorileri getirir (Benzersiz)
        public List<string> GetCategories()
        {
            return _context.MenuItems
                .Select(m => m.Category)
                .Distinct()
                .ToList();
        }

        // ID'ye göre tek bir ürünü getirir
        public MenuItem? GetById(int id)
        {
            return _context.MenuItems.FirstOrDefault(m => m.Id == id);
        }

        // Yeni ürün ekler (Admin kullanır)
        public void AddItem(string name, string description, decimal price, string category, bool isAvailable)
        {
            var item = new MenuItem
            {
                Name = name,
                Description = description,
                Price = price,
                Category = category,
                IsAvailable = isAvailable
            };
            _context.MenuItems.Add(item);
            _context.SaveChanges();
        }

        // Mevcut ürünü günceller (Admin kullanır)
        public bool UpdateItem(int id, string name, string description, decimal price, string category, bool isAvailable)
        {
            var existing = _context.MenuItems.FirstOrDefault(m => m.Id == id);
            if (existing != null)
            {
                existing.Name = name;
                existing.Description = description;
                existing.Price = price;
                existing.Category = category;
                existing.IsAvailable = isAvailable;
                
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        // Ürünü siler (Admin kullanır)
        public bool DeleteItem(int id)
        {
            var existing = _context.MenuItems.FirstOrDefault(m => m.Id == id);
            if (existing != null)
            {
                _context.MenuItems.Remove(existing);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
