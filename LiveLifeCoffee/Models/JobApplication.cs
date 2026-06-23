// =========================================================
// Models/JobApplication.cs
// İŞ BAŞVURUSU MODELİ
//
// Web sitesindeki "İş Başvurusu" formundan gelen başvuruları
// veritabanında kalıcı olarak saklamak için kullanılır.
// Admin panelinde tablo halinde görüntülenir.
// =========================================================

namespace LiveLifeCoffee.Models
{
    /// <summary>
    /// Kullanıcının iş başvurusu formundan gönderdiği veriyi temsil eder.
    /// </summary>
    public class JobApplication
    {
        // Başvurunun benzersiz kimlik numarası (veritabanı tarafından otomatik atanır)
        public int Id { get; set; }

        // Başvuru sahibinin tam adı (zorunlu alan)
        public string FullName { get; set; } = string.Empty;

        // Başvuru sahibinin e-posta adresi (iletişim için)
        public string Email { get; set; } = string.Empty;

        // Başvuru sahibinin telefon numarası (isteğe bağlı)
        public string Phone { get; set; } = string.Empty;

        // Başvurulan pozisyon (Barista, Kasiyer, Garson vb.)
        public string Position { get; set; } = string.Empty;

        // Başvuru sahibinin motivasyon mektubu veya kendini tanıtma metni
        public string CoverLetter { get; set; } = string.Empty;

        // Başvurunun sisteme ulaştığı tarih ve saat (otomatik ayarlanır)
        public DateTime AppliedAt { get; set; } = DateTime.Now;

        // Admin başvuruyu görüntüledi mi? (okunmamış rozeti için kullanılır)
        public bool IsRead { get; set; } = false;
    }
}
