// =========================================================
// Services/JobApplicationService.cs
// İŞ BAŞVURUSU SERVİSİ
//
// Veritabanındaki JobApplications tablosuyla ilgili tüm
// iş mantığı işlemlerini (CRUD) bu servis yürütür.
// Controller'lar doğrudan veritabanına erişmek yerine bu
// servisi kullanır (Separation of Concerns prensibi).
//
// Bağımlılık: AppDbContext (EF Core üzerinden SQLite'a bağlı)
// Yaşam döngüsü: Scoped (her HTTP isteğinde yeni örnek)
// =========================================================

using LiveLifeCoffee.Data;   // AppDbContext için
using LiveLifeCoffee.Models; // JobApplication modeli için

namespace LiveLifeCoffee.Services
{
    /// <summary>
    /// İş başvurularının eklenmesi, listelenmesi ve detay görüntülenmesi işlemlerini yönetir.
    /// </summary>
    public class JobApplicationService
    {
        // EF Core veritabanı bağlamı: DI (Dependency Injection) ile enjekte edilir
        private readonly AppDbContext _db;

        // Constructor: ASP.NET Core, AppDbContext'i otomatik olarak sağlar
        public JobApplicationService(AppDbContext db)
        {
            _db = db; // Veritabanı bağlamını saklıyoruz
        }

        // -------------------------------------------------------
        // YENİ BAŞVURU EKLE
        // Form verilerinden JobApplication nesnesi oluşturur
        // ve veritabanına kaydeder.
        // -------------------------------------------------------
        public void Add(string fullName, string email, string phone, string position, string coverLetter)
        {
            // Yeni başvuru nesnesi oluştur (tarih otomatik atanır)
            var application = new JobApplication
            {
                FullName    = fullName,    // Başvuru sahibinin adı
                Email       = email,       // E-posta adresi
                Phone       = phone,       // Telefon numarası
                Position    = position,    // Başvurulan pozisyon
                CoverLetter = coverLetter, // Motivasyon mektubu
                AppliedAt   = DateTime.Now,// Başvuru zamanı
                IsRead      = false        // Henüz okunmadı
            };

            // EF Core: nesneyi "eklenecek" durumuna al
            _db.JobApplications.Add(application);

            // Değişiklikleri veritabanına kaydet
            _db.SaveChanges();
        }

        // -------------------------------------------------------
        // TÜM BAŞVURULARI GETİR
        // En yeni başvuru en üstte gelecek şekilde sıralar.
        // Admin panelinde tablo halinde gösterilir.
        // -------------------------------------------------------
        public List<JobApplication> GetAll()
        {
            // Tüm başvuruları en yeni tarihe göre azalan sırada getir
            return _db.JobApplications
                .OrderByDescending(a => a.AppliedAt) // En yeni önce
                .ToList();                            // Liste olarak döndür
        }

        // -------------------------------------------------------
        // ID'YE GÖRE TEK BAŞVURU GETİR
        // Admin başvuru detay sayfası için kullanılır.
        // Bulunamazsa null döndürür.
        // -------------------------------------------------------
        public JobApplication? GetById(int id)
        {
            // FirstOrDefault: kayıt yoksa null döner, exception atmaz
            return _db.JobApplications.FirstOrDefault(a => a.Id == id);
        }

        // -------------------------------------------------------
        // BAŞVURUYU OKUNDU OLARAK İŞARETLE
        // Admin başvuruyu açtığında otomatik çağrılır.
        // IsRead = true yapılır; böylece "Yeni" rozeti kaybolur.
        // -------------------------------------------------------
        public void MarkAsRead(int id)
        {
            // Başvuruyu bul
            var application = _db.JobApplications.FirstOrDefault(a => a.Id == id);

            // Başvuru bulundu ve henüz okunmadıysa güncelle
            if (application != null && !application.IsRead)
            {
                application.IsRead = true; // Okundu olarak işaretle
                _db.SaveChanges();         // Veritabanına kaydet
            }
        }

        // -------------------------------------------------------
        // OKUNMAMIŞ BAŞVURU SAYISINI GETİR
        // Admin Dashboard'da bildirim rozeti için kullanılır.
        // -------------------------------------------------------
        public int GetUnreadCount()
        {
            // IsRead = false olan kayıtları say
            return _db.JobApplications.Count(a => !a.IsRead);
        }
    }
}
