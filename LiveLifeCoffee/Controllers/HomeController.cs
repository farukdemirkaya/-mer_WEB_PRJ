// =========================================================
// Controllers/HomeController.cs
// Ana Sayfa, İletişim ve İş Başvurusu sayfalarını yönetir.
// Bu sayfalar giriş gerektirmez; herkese açıktır.
//
// KAPSAM:
//   - Ana Sayfa (Index)
//   - İletişim Formu (Contact)
//   - İş Başvurusu Formu (JobApplication) ← YENİ
// =========================================================

using LiveLifeCoffee.Models;    // ViewModel ve Model sınıfları için
using LiveLifeCoffee.Services;  // Servis sınıfları için
using Microsoft.AspNetCore.Mvc; // Controller altyapısı için

namespace LiveLifeCoffee.Controllers
{
    /// <summary>
    /// Ana sayfa, iletişim ve iş başvurusu sayfalarına ait HTTP isteklerini işler.
    /// </summary>
    public class HomeController : Controller
    {
        // İletişim mesajı servisi: DI ile enjekte edilir
        private readonly ContactMessageService _contactMessageService;

        // İş başvurusu servisi: DI ile enjekte edilir (YENİ)
        private readonly JobApplicationService _jobApplicationService;

        // Constructor: ASP.NET Core her iki servisi de otomatik sağlar
        public HomeController(
            ContactMessageService contactMessageService,
            JobApplicationService jobApplicationService) // YENİ parametre
        {
            _contactMessageService  = contactMessageService;  // İletişim servisi atanıyor
            _jobApplicationService  = jobApplicationService;  // Başvuru servisi atanıyor
        }

        // -------------------------------------------------------
        // GET: /  veya  /Home/Index
        // Ana sayfayı döndürür. Herkes erişebilir.
        // -------------------------------------------------------
        public IActionResult Index()
        {
            // Ana sayfa View'ını render et ve tarayıcıya gönder
            return View();
        }

        // -------------------------------------------------------
        // GET: /Home/Contact
        // Boş iletişim formunu gösterir.
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult Contact()
        {
            // Yeni ve boş bir ContactViewModel oluşturup View'a gönder
            var model = new ContactViewModel();
            return View(model);
        }

        // -------------------------------------------------------
        // POST: /Home/Contact
        // İletişim formu gönderildiğinde çalışır.
        // [ValidateAntiForgeryToken]: CSRF saldırılarına karşı koruma.
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactViewModel model)
        {
            // ModelState.IsValid: DataAnnotations kurallarının hepsinin geçilip geçilmediğini kontrol eder
            if (!ModelState.IsValid)
            {
                // Doğrulama hatası varsa formu hata mesajlarıyla birlikte tekrar göster
                return View(model);
            }

            // Mesajı servis aracılığıyla veritabanına kaydet
            _contactMessageService.Save(
                model.FullName,  // Gönderenin adı
                model.Email,     // Gönderenin e-postası
                model.Subject,   // Mesaj konusu
                model.Message    // Mesajın içeriği
            );

            // Form başarıyla işlendi; teşekkür mesajı göstermek için bayrağı set et
            model.IsSuccess = true;

            // Aynı View'ı başarı bayrağıyla geri döndür (yönlendirme yok; çift gönderim önlenmez ama basit kalıyoruz)
            return View(model);
        }

        // -------------------------------------------------------
        // GET: /Home/JobApplication
        // Boş iş başvurusu formunu gösterir. Herkes erişebilir.
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult JobApplication()
        {
            // Yeni ve boş bir JobApplicationViewModel oluşturup View'a gönder
            var model = new JobApplicationViewModel();
            return View(model);
        }

        // -------------------------------------------------------
        // POST: /Home/JobApplication
        // İş başvurusu formu gönderildiğinde çalışır.
        // Sunucu taraflı doğrulama sonrası başvuruyu veritabanına kaydeder.
        // [ValidateAntiForgeryToken]: CSRF koruması.
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult JobApplication(JobApplicationViewModel model)
        {
            // Doğrulama kuralları (DataAnnotations) karşılandı mı kontrol et
            if (!ModelState.IsValid)
            {
                // Hata varsa formu hatalarla birlikte tekrar göster
                return View(model);
            }

            // Başvuruyu servis aracılığıyla veritabanına kaydet
            _jobApplicationService.Add(
                model.FullName,     // Başvuru sahibinin adı
                model.Email,        // E-posta adresi
                model.Phone,        // Telefon numarası
                model.Position,     // Başvurulan pozisyon
                model.CoverLetter   // Motivasyon mektubu
            );

            // Başarı bayrağını set et; View teşekkür mesajı gösterecek
            model.IsSuccess = true;

            // Aynı View'ı başarı bayrağıyla döndür
            return View(model);
        }

        // -------------------------------------------------------
        // GET: /Home/Privacy
        // Gizlilik sayfası (yedek sayfa, gerektiğinde kullanılır).
        // -------------------------------------------------------
        public IActionResult Privacy()
        {
            // Basit gizlilik bilgisi sayfası
            return View();
        }

        // -------------------------------------------------------
        // GET: /Home/Error
        // Hata durumunda kullanıcıya bilgi sayfası gösterir.
        // -------------------------------------------------------
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Hata sayfasını döndür (ErrorViewModel opsiyonel)
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
