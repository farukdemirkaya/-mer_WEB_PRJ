// =========================================================
// Services/ContactMessageService.cs
// İletişim formu mesajlarını veritabanı üzerinden yönetir.
// =========================================================

using LiveLifeCoffee.Models;
using LiveLifeCoffee.Data;
using System.Collections.Generic;
using System.Linq;

namespace LiveLifeCoffee.Services
{
    public class ContactMessageService
    {
        private readonly AppDbContext _context;

        public ContactMessageService(AppDbContext context)
        {
            _context = context;
        }

        // Yeni iletişim mesajını veritabanına ekler
        public void Save(string fullName, string email, string subject, string message)
        {
            var msg = new ContactMessage
            {
                FullName = fullName,
                Email = email,
                Subject = subject,
                Message = message,
                ReceivedAt = DateTime.Now,
                IsRead = false
            };
            _context.ContactMessages.Add(msg);
            _context.SaveChanges();
        }

        // Tüm iletişim mesajlarını en yeniden eskiye doğru getirir (Admin için)
        public List<ContactMessage> GetAll()
        {
            return _context.ContactMessages
                .OrderByDescending(m => m.ReceivedAt)
                .ToList();
        }

        // Tek bir mesajı getirir
        public ContactMessage? GetById(int id)
        {
            return _context.ContactMessages.FirstOrDefault(m => m.Id == id);
        }

        // Mesajı okundu olarak işaretler
        public void MarkAsRead(int id)
        {
            var msg = _context.ContactMessages.FirstOrDefault(m => m.Id == id);
            if (msg != null && !msg.IsRead)
            {
                msg.IsRead = true;
                _context.SaveChanges();
            }
        }

        // Okunmamış (yeni) mesaj sayısını döndürür
        public int GetUnreadCount()
        {
            return _context.ContactMessages.Count(m => !m.IsRead);
        }
    }
}
