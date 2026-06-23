// =========================================================
// Models/ContactMessage.cs
// İletişim formundan gelen mesajları kalıcı olarak saklamak için
// kullanılan domain modeli.
// Daha önce ContactViewModel yalnızca form verisini taşıyordu;
// bu model ise veritabanı/bellek kaydı için kullanılır.
// =========================================================
// ÖMMERRR
namespace LiveLifeCoffee.Models
{
    /// <summary>
    /// Kullanıcının iletişim formundan gönderdiği mesajı temsil eder.
    /// </summary>
    public class ContactMessage
    {
        // Mesajın benzersiz kimlik numarası (otomatik atanır)
        public int Id { get; set; }

        // Mesajı gönderen kişinin tam adı
        public string FullName { get; set; } = string.Empty;

        // Gönderenin e-posta adresi (geri dönüş için)
        public string Email { get; set; } = string.Empty;

        // Mesajın konusu
        public string Subject { get; set; } = string.Empty;

        // Mesajın içeriği
        public string Message { get; set; } = string.Empty;

        // Mesajın sisteme ulaştığı tarih ve saat
        public DateTime ReceivedAt { get; set; } = DateTime.Now;

        // Admin tarafından okundu mu? (Dashboard'da "Yeni" rozeti için)
        public bool IsRead { get; set; } = false;
    }
}
