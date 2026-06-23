// =========================================================
// Controllers/AdminController.cs
// Admin Paneli — Tüm yönetim işlemleri burada toplanır.
//
// KORUMA MANTIĞI:
//   Her action en başında RequireAdmin() metodunu çağırır.
//   Session'da "IsAdmin" = "true" yoksa → Admin Login'e yönlendir.
//   Bu yaklaşım sayesinde hiçbir admin action'ı izinsiz çalışmaz.
//
// KAPSAM:
//   - Admin giriş / çıkış
//   - Dashboard (özet istatistikler)
//   - Tüm siparişleri görüntüleme
//   - İletişim mesajlarını görüntüleme + okundu işaretleme
//   - Menü ürünü listeleme / ekleme / düzenleme / silme
// =========================================================

using LiveLifeCoffee.Models;
using LiveLifeCoffee.Services;
using Microsoft.AspNetCore.Mvc;

namespace LiveLifeCoffee.Controllers
{
    /// <summary>
    /// Admin paneline ait tüm HTTP isteklerini işler.
    /// </summary>
    public class AdminController : Controller
    {
        // -------------------------------------------------------
        // ADMIN KİMLİK BİLGİLERİ
        // Gerçek projede bu bilgiler appsettings.json veya
        // bir veritabanından şifreli olarak okunur.
        // Şimdilik basitlik için sabit olarak tanımlıyoruz.
        // -------------------------------------------------------
        // Giriş artık /Account/Login üzerinden yapılır.
        // AccountController'daki AdminEmail ve AdminPassword sabitleriyle EŞ OLMALIDIR.
        // /Admin/Login sayfası yedek olarak çalışmaya devam eder.
        private const string AdminUsername = "admin@livelife.com"; // Admin e-posta (giriş sayfasında kullanılan)
        private const string AdminPassword = "admin1234";           // Admin parolası

        // Session'da admin durumunu saklamak için anahtar sabiti
        private const string AdminSessionKey = "IsAdmin";

        // Servis bağımlılıkları; DI ile enjekte edilir
        private readonly MenuService _menuService;                          // Menü ürünleri servisi
        private readonly OrderService _orderService;                        // Siparişler servisi
        private readonly ContactMessageService _contactMessageService;      // İletişim mesajları servisi
        private readonly JobApplicationService _jobApplicationService;      // İş başvuruları servisi (YENİ)

        // Constructor: ASP.NET Core gerekli servisleri otomatik sağlar
        public AdminController(
            MenuService menuService,
            OrderService orderService,
            ContactMessageService contactMessageService,
            JobApplicationService jobApplicationService) // YENİ parametre
        {
            _menuService            = menuService;            // Menü servisini ata
            _orderService           = orderService;           // Sipariş servisini ata
            _contactMessageService  = contactMessageService;  // Mesaj servisini ata
            _jobApplicationService  = jobApplicationService;  // Başvuru servisini ata (YENİ)
        }

        // -------------------------------------------------------
        // PRIVATE YARDIMCI METOT: Admin oturum kontrolü.
        // Session'da "IsAdmin" = "true" değeri var mı kontrol eder.
        // Yoksa false döner; action Login'e yönlendirir.
        // -------------------------------------------------------
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString(AdminSessionKey) == "true";
        }

        // -------------------------------------------------------
        // PRIVATE YARDIMCI METOT: Admin değilse Login'e yönlendir.
        // Her korunan action'ın başında çağrılır.
        // null döndürürse devam edilir; IActionResult döndürürse yönlendirme yapılır.
        // -------------------------------------------------------
        private IActionResult? RequireAdmin()
        {
            // Admin oturumu yoksa giriş sayfasına yönlendir
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            // Admin oturumu varsa null döndür (devam et)
            return null;
        }

        // =======================================================
        // ADMIN GİRİŞ / ÇIKIŞ
        // =======================================================

        // -------------------------------------------------------
        // GET: /Admin/Login
        // Admin giriş formunu gösterir.
        // Zaten giriş yapmış admin Dashboard'a yönlendirilir.
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult Login()
        {
            // Admin zaten giriş yapmışsa Dashboard'a git
            if (IsAdmin())
            {
                return RedirectToAction("Dashboard");
            }

            // ViewBag.Error: hatalı giriş denemesinde hata mesajı taşır
            return View();
        }

        // -------------------------------------------------------
        // POST: /Admin/Login
        // Kullanıcı adı ve parola kontrolü yapar.
        // Başarılıysa Session'a IsAdmin = true yazar.
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]  // CSRF koruması
        public IActionResult Login(string username, string password)
        {
            // Kullanıcı adı VE parola sabit değerlerle eşleşiyor mu?
            // StringComparison.Ordinal: büyük/küçük harf duyarlı karşılaştırma
            bool isValid = string.Equals(username, AdminUsername, StringComparison.Ordinal)
                        && string.Equals(password, AdminPassword, StringComparison.Ordinal);

            if (!isValid)
            {
                // Hatalı giriş; hata mesajı ayarla ve formu tekrar göster
                ViewBag.Error = "Kullanıcı adı veya parola hatalı.";
                return View();
            }

            // Giriş başarılı; Session'a admin bayrağını yaz
            HttpContext.Session.SetString(AdminSessionKey, "true");

            // Admin Dashboard'a yönlendir
            return RedirectToAction("Dashboard");
        }

        // -------------------------------------------------------
        // GET: /Admin/Logout
        // Admin oturumunu sonlandırır; normal kullanıcı oturumuna dokunmaz.
        // -------------------------------------------------------
        public IActionResult Logout()
        {
            // Yalnızca admin session anahtarını kaldır
            // (Normal kullanıcı oturumu açıksa o devam eder)
            HttpContext.Session.Remove(AdminSessionKey);

            // Admin giriş sayfasına yönlendir
            return RedirectToAction("Login");
        }

        // =======================================================
        // DASHBOARD
        // =======================================================

        // -------------------------------------------------------
        // GET: /Admin/Dashboard
        // Özet istatistikleri gösterir: sipariş sayısı, mesaj sayısı,
        // menü ürün sayısı, toplam ciro.
        // -------------------------------------------------------
        public IActionResult Dashboard()
        {
            // Admin koruması; admin değilse yönlendir
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            // Tüm siparişleri al
            var allOrders = _orderService.GetAll();

            // Kontrol Paneli'nde gösterilecek istatistikler ViewBag üzerinden taşınır
            ViewBag.TotalOrders = allOrders.Count; // Toplam sipariş sayısı

            // Toplam ciro: tüm siparişlerin GrandTotal toplamı
            ViewBag.TotalRevenue = allOrders.Sum(o => o.GrandTotal);

            // Toplam menü ürün sayısı
            ViewBag.TotalMenuItems = _menuService.GetAll().Count;

            // Okunmamış mesaj sayısı (bildirim rozeti için)
            ViewBag.UnreadMessages = _contactMessageService.GetUnreadCount();

            // Okunmamış iş başvurusu sayısı (YENİ — bildirim rozeti için)
            ViewBag.UnreadApplications = _jobApplicationService.GetUnreadCount();

            // Son 5 sipariş (en yeni önce)
            ViewBag.RecentOrders = allOrders
                .OrderByDescending(o => o.OrderedAt) // En yeni önce sırala
                .Take(5)                             // Yalnızca son 5 sipariş
                .ToList();                           // Listeye çevir

            return View(); // Dashboard View'ını render et
        }

        // =======================================================
        // SİPARİŞLER
        // =======================================================

        // -------------------------------------------------------
        // GET: /Admin/Orders
        // Tüm siparişleri listeler (en yeni önce).
        // -------------------------------------------------------
        public IActionResult Orders()
        {
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            // Tüm siparişleri al; en yeni önce sırala
            var orders = _orderService.GetAll()
                .OrderByDescending(o => o.OrderedAt)
                .ToList();

            return View(orders);
        }

        // -------------------------------------------------------
        // GET: /Admin/OrderDetail/{id}
        // Belirli bir siparişin detaylarını gösterir.
        // -------------------------------------------------------
        public IActionResult OrderDetail(int id)
        {
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            // Siparişi bul
            var order = _orderService.GetById(id);

            if (order == null)
            {
                // Sipariş yoksa 404
                return NotFound("Sipariş bulunamadı.");
            }

            return View(order);
        }

        // =======================================================
        // İLETİŞİM MESAJLARI
        // =======================================================

        // -------------------------------------------------------
        // GET: /Admin/Messages
        // Tüm iletişim mesajlarını listeler.
        // -------------------------------------------------------
        public IActionResult Messages()
        {
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            // Tüm mesajları al (en yeni önce)
            var messages = _contactMessageService.GetAll();

            return View(messages);
        }

        // -------------------------------------------------------
        // GET: /Admin/MessageDetail/{id}
        // Tek bir mesajı detaylı gösterir ve "okundu" işaretler.
        // -------------------------------------------------------
        public IActionResult MessageDetail(int id)
        {
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            // Mesajı bul
            var message = _contactMessageService.GetById(id);

            if (message == null)
            {
                return NotFound("Mesaj bulunamadı.");
            }

            // Admin mesajı açtığında otomatik "okundu" işaretle
            _contactMessageService.MarkAsRead(id);

            return View(message);
        }

        // =======================================================
        // MENÜ YÖNETİMİ
        // =======================================================

        // -------------------------------------------------------
        // GET: /Admin/Menu
        // Tüm menü ürünlerini listeler.
        // -------------------------------------------------------
        public IActionResult Menu()
        {
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            // Tüm ürünleri al
            var items = _menuService.GetAll();

            return View(items);
        }

        // -------------------------------------------------------
        // GET: /Admin/AddMenuItem
        // Yeni ürün ekleme formunu gösterir.
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult AddMenuItem()
        {
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            // Mevcut kategorileri dropdown için hazırla
            ViewBag.Categories = _menuService.GetCategories();

            // Boş ViewModel ile formu göster
            return View(new MenuItemViewModel());
        }

        // -------------------------------------------------------
        // POST: /Admin/AddMenuItem
        // Form gönderildiğinde yeni ürünü sisteme ekler.
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMenuItem(MenuItemViewModel model)
        {
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            // Sunucu taraflı doğrulama kontrolü
            if (!ModelState.IsValid)
            {
                // Hata varsa dropdown'u yeniden yükle ve formu tekrar göster
                ViewBag.Categories = _menuService.GetCategories();
                return View(model);
            }

            // MenuService üzerinden yeni ürün ekle
            _menuService.AddItem(
                model.Name,
                model.Description,
                model.Price,
                model.Category,
                model.IsAvailable
            );

            // Başarı mesajı ve menü listesine dön
            TempData["SuccessMessage"] = $"'{model.Name}' menüye başarıyla eklendi.";
            return RedirectToAction("Menu");
        }

        // -------------------------------------------------------
        // GET: /Admin/EditMenuItem/{id}
        // Mevcut bir ürünü düzenleme formunu dolu olarak gösterir.
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult EditMenuItem(int id)
        {
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            // Düzenlenecek ürünü bul
            var item = _menuService.GetById(id);

            if (item == null)
            {
                return NotFound("Ürün bulunamadı.");
            }

            // MenuItem → MenuItemViewModel dönüşümü
            // Domain modeli doğrudan formda kullanmak yerine ViewModel tercih edilir
            var model = new MenuItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Category = item.Category,
                IsAvailable = item.IsAvailable
            };

            // Dropdown için kategorileri hazırla
            ViewBag.Categories = _menuService.GetCategories();

            return View(model);
        }

        // -------------------------------------------------------
        // POST: /Admin/EditMenuItem
        // Formu kaydeder ve ürünü günceller.
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMenuItem(MenuItemViewModel model)
        {
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            // Sunucu taraflı doğrulama
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _menuService.GetCategories();
                return View(model);
            }

            // MenuService üzerinden güncelle
            bool updated = _menuService.UpdateItem(
                model.Id,
                model.Name,
                model.Description,
                model.Price,
                model.Category,
                model.IsAvailable
            );

            if (!updated)
            {
                // Ürün artık yoksa (silinmiş olabilir)
                TempData["ErrorMessage"] = "Ürün güncellenemedi. Ürün bulunamadı.";
                return RedirectToAction("Menu");
            }

            TempData["SuccessMessage"] = $"'{model.Name}' başarıyla güncellendi.";
            return RedirectToAction("Menu");
        }

        // -------------------------------------------------------
        // POST: /Admin/DeleteMenuItem
        // Belirtilen ürünü menüden siler.
        // GET değil POST: silme işlemi yan etki yaratır; GET ile yapılmamalıdır.
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMenuItem(int id)
        {
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            // Silinecek ürünün adını önceden al (başarı mesajı için)
            var item = _menuService.GetById(id);
            string itemName = item?.Name ?? "Ürün";

            // MenuService üzerinden sil
            bool deleted = _menuService.DeleteItem(id);

            if (deleted)
            {
                TempData["SuccessMessage"] = $"'{itemName}' menüden silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Ürün silinemedi. Ürün bulunamadı.";
            }

            return RedirectToAction("Menu"); // Menü listesine geri dön
        }

        // =======================================================
        // İŞ BAŞVURULARI (YENİ BÖLÜM)
        // =======================================================

        // -------------------------------------------------------
        // GET: /Admin/Applications
        // Tüm iş başvurularını tablo halinde listeler.
        // En yeni başvuru en üstte görünür.
        // -------------------------------------------------------
        public IActionResult Applications()
        {
            // Admin koruması: oturum yoksa giriş sayfasına yönlendir
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            // Tüm başvuruları servisten al (en yeni önce sıralı)
            var applications = _jobApplicationService.GetAll();

            // View'a listeyi model olarak gönder
            return View(applications);
        }

        // -------------------------------------------------------
        // GET: /Admin/ApplicationDetail/{id}
        // Belirli bir başvuruyu detaylı gösterir ve "okundu" işaretler.
        // -------------------------------------------------------
        public IActionResult ApplicationDetail(int id)
        {
            // Admin koruması: oturum yoksa giriş sayfasına yönlendir
            var redirect = RequireAdmin();
            if (redirect != null) return redirect;

            // Belirtilen ID'ye sahip başvuruyu bul
            var application = _jobApplicationService.GetById(id);

            // Başvuru bulunamazsa 404 hatası döndür
            if (application == null)
            {
                return NotFound("Başvuru bulunamadı."); // Kayıt yok
            }

            // Admin başvuruyu açtığında otomatik olarak "okundu" işaretle
            _jobApplicationService.MarkAsRead(id);

            // Başvuru detay View'ını model ile render et
            return View(application);
        }
    }
}
