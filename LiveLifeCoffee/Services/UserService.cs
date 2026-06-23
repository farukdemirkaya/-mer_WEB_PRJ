// =========================================================
// Services/UserService.cs
// Kullanıcı hesap işlemlerini veritabanı üzerinden yönetir.
// =========================================================

using System.Security.Cryptography;
using System.Text;
using LiveLifeCoffee.Models;
using LiveLifeCoffee.Data;

namespace LiveLifeCoffee.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // -------------------------------------------------------
        // Yeni bir müşteri kaydı oluşturur (Veritabanına yazar)
        // -------------------------------------------------------
        public bool Register(string fullName, string email, string password)
        {
            // E-posta daha önce kayıtlı mı?
            bool exists = _context.Users.Any(u => u.Email.ToLower() == email.ToLower());
            if (exists) return false;

            // Parolayı SHA-256 ile hash'le
            string passwordHash = HashPassword(password);

            // Veritabanına kaydedilecek nesneyi oluştur
            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = passwordHash,
                RegisteredAt = DateTime.Now
            };

            // EF Core üzerinden veritabanına ekle
            _context.Users.Add(user);
            _context.SaveChanges();

            return true;
        }

        // -------------------------------------------------------
        // Kullanıcı girişi için doğrulama yapar (Veritabanından okur)
        // -------------------------------------------------------
        public User? Authenticate(string email, string password)
        {
            // E-postaya göre kullanıcıyı bul
            var user = _context.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

            if (user == null) return null; // Kullanıcı yok

            // Parolanın hash'ini hesapla ve veritabanındaki ile karşılaştır
            string passwordHash = HashPassword(password);

            if (user.PasswordHash == passwordHash)
            {
                return user; // Giriş başarılı
            }

            return null; // Parola hatalı
        }

        // -------------------------------------------------------
        // Güvenlik: Düz metin parolayı SHA-256 algoritmasıyla şifreler
        // -------------------------------------------------------
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
