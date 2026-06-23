// =========================================================
// Models/User.cs
// Kullanıcı (müşteri) veri modelini tanımlar.
// Uygulamada kayıt olan her müşteri bu sınıfın bir örneğidir.
// =========================================================

namespace LiveLifeCoffee.Models
{
    /// <summary>
    /// Sisteme kayıtlı bir kullanıcıyı temsil eden model sınıfı.
    /// </summary>
    public class User
    {
        // Kullanıcının benzersiz kimlik numarası (otomatik atanır)
        public int Id { get; set; }

        // Kullanıcının tam adı (Ad Soyad)
        public string FullName { get; set; } = string.Empty;

        // Kullanıcının e-posta adresi (giriş için kullanılır)
        public string Email { get; set; } = string.Empty;

        // Kullanıcının hashlenmiş parolası (SHA-256 ile saklanır, ham parola hiç tutulmaz)
        public string PasswordHash { get; set; } = string.Empty;

        // Kullanıcının sisteme kaydolduğu tarih ve saat
        public DateTime RegisteredAt { get; set; } = DateTime.Now;
    }
}
