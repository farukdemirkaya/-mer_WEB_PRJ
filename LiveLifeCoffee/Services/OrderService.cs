// =========================================================
// Services/OrderService.cs
// Siparişleri veritabanı üzerinden yönetir.
// =========================================================

using LiveLifeCoffee.Models;
using LiveLifeCoffee.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace LiveLifeCoffee.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        // Yeni bir siparişi veritabanına kaydeder
        public Order CreateOrder(int userId, string userFullName, List<CartItem> cartItems)
        {
            var order = new Order
            {
                UserId = userId,
                UserFullName = userFullName,
                Items = cartItems,
                GrandTotal = cartItems.Sum(i => i.TotalPrice),
                OrderedAt = DateTime.Now,
                Status = "Hazırlanıyor"
            };

            _context.Orders.Add(order);
            _context.SaveChanges();
            return order;
        }

        // Belirli bir kullanıcının (UserId) geçmiş siparişlerini getirir
        // Include ile alt kalemleri de (CartItems) sorguya dahil ediyoruz
        public List<Order> GetUserOrders(int userId)
        {
            return _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderedAt)
                .ToList();
        }

        // Sistemdeki tüm siparişleri getirir (Admin paneli için)
        public List<Order> GetAll()
        {
            return _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.OrderedAt)
                .ToList();
        }

        // Tek bir siparişin detaylarını getirir (Admin veya Kullanıcı için)
        public Order? GetById(int orderId)
        {
            return _context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == orderId);
        }

        // Siparişin durumunu günceller ("Hazırlanıyor", "Teslim Edildi" vb.)
        public void UpdateOrderStatus(int orderId, string newStatus)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = newStatus;
                _context.SaveChanges();
            }
        }
    }
}
