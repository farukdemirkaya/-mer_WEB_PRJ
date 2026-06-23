// =========================================================
// Controllers/AccountController.cs
// Kullanıcı kayıt, giriş ve çıkış işlemlerini yönetir.
// Session üzerinden kullanıcı kimliği ve adı saklanır.
//
// GİRİŞ MANTIĞI (tek noktadan):
//   POST /Account/Login'e gelen kimlik bilgileri önce admin
//   sabitleriile karşılaştırılır. Eşleşirse admin session açılır
//   ve Admin Dashboard'a yönlendirilir. Eşleşmezse normal
//   kullanıcı doğrulaması yapılır.
//   Bu sayede tek bir giriş sayfası hem müşteriyi hem admini
//   doğru yere yönlendirir.
// =========================================================

using LiveLifeCoffee.Models;
using LiveLifeCoffee.Services;
using Microsoft.AspNetCore.Mvc;

namespace LiveLifeCoffee.Controllers
{
    /// <summary>
    /// Kayıt, giriş ve çıkış işlemlerine ait HTTP isteklerini işler.
    /// </summary>
    public class AccountController : Controller
    {
        // ---------------------------------------------------------
        // ADMİN KİMLİK BİLGİLERİ
        // Normal kullanıcı giriş sayfasından admin girişine izin vermek için
        // burada da tanımlanır. Gerçek projede merkezi bir config'den okunur.
        // ---------------------------------------------------------
        private const string AdminEmail    = "admin@livelife.com"; // Admin e-posta adresi
        private const string AdminPassword = "admin1234";           // Admin parolası

        // Session'da admin bayrağını saklamak için anahtar
        private const string AdminSessionKey = "IsAdmin";

        // UserService bağımlılığı; DI ile enjekte edilir
        private readonly UserService _userService;

        // Constructor: ASP.NET Core, UserService'i otomatik olarak sağlar
        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        // -------------------------------------------------------
        // GET: /Account/Register
        // Kayıt formunu boş olarak gösterir.
        // Zaten giriş yapmış kullanıcılar ana sayfaya yönlendirilir.
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult Register()
        {
            // Session'da kullanıcı ID'si varsa zaten giriş yapmış
            if (HttpContext.Session.GetInt32("UserId").HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new RegisterViewModel());
        }

        // -------------------------------------------------------
        // POST: /Account/Register
        // Form gönderildiğinde çalışır; doğrulama ve kayıt işlemi yapılır.
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Admin e-postasıyla kayıt yapılmasını engelle
            if (model.Email.Equals(AdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("Email", "Bu e-posta adresi kullanılamaz.");
                return View(model);
            }

            bool success = _userService.Register(model.FullName, model.Email, model.Password);

            if (!success)
            {
                ModelState.AddModelError("Email", "Bu e-posta adresi zaten kayıtlı.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Kayıt başarılı! Şimdi giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        // -------------------------------------------------------
        // GET: /Account/Login
        // Tek giriş sayfası: hem müşteri hem admin buradan giriş yapar.
        // Zaten giriş yapmış kullanıcılar yönlendirilir.
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult Login()
        {
            // Admin oturumu açıksa Dashboard'a git
            if (HttpContext.Session.GetString(AdminSessionKey) == "true")
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            // Normal kullanıcı oturumu açıksa Ana Sayfa'ya git
            if (HttpContext.Session.GetInt32("UserId").HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        // -------------------------------------------------------
        // POST: /Account/Login
        // Giriş formu gönderildiğinde çalışır.
        //
        // AKIŞ:
        //   1. Formdan gelen e-posta + parola admin sabitleriyle karşılaştırılır.
        //      → Eşleşirse: Session'a IsAdmin = "true" yaz, Admin Dashboard'a git.
        //   2. Eşleşmezse UserService ile normal kullanıcı doğrulaması yapılır.
        //      → Bulunursa: Session'a UserId + UserFullName yaz, Ana Sayfa'ya git.
        //   3. İkisi de eşleşmezse: hata mesajı göster.
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            // Sunucu taraflı doğrulama (boş alan, geçersiz e-posta formatı vb.)
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // ---------------------------------------------------------
            // ADIM 1: Admin kimlik bilgisi kontrolü
            // E-posta VE parola sabitlerle birebir eşleşiyor mu?
            // StringComparison.OrdinalIgnoreCase → büyük/küçük harf fark etmez
            // ---------------------------------------------------------
            bool isAdminEmail    = model.Email.Equals(AdminEmail, StringComparison.OrdinalIgnoreCase);
            bool isAdminPassword = model.Password == AdminPassword; // Parola büyük/küçük duyarlı

            if (isAdminEmail && isAdminPassword)
            {
                // Admin kimliği doğrulandı
                // Session'a admin bayrağını yaz; normal kullanıcı session'u açılmaz
                HttpContext.Session.SetString(AdminSessionKey, "true");

                // Admin Dashboard'a yönlendir
                return RedirectToAction("Dashboard", "Admin");
            }

            // ---------------------------------------------------------
            // ADIM 2: Normal kullanıcı doğrulaması
            // UserService SHA-256 hash karşılaştırması yapar
            // ---------------------------------------------------------
            var user = _userService.Authenticate(model.Email, model.Password);

            if (user == null)
            {
                // Admin de değil, normal kullanıcı da değil → hata göster
                model.ErrorMessage = "E-posta veya parola hatalı.";
                return View(model);
            }

            // Normal kullanıcı girişi başarılı
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserFullName", user.FullName);

            // Giriş öncesi ziyaret edilmek istenen sayfa varsa oraya dön
            string? returnUrl = TempData["ReturnUrl"] as string;
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        // -------------------------------------------------------
        // GET: /Account/Logout
        // Session'ı tamamen temizler (hem kullanıcı hem admin oturumunu kapatır).
        // -------------------------------------------------------
        public IActionResult Logout()
        {
            // Tüm session verilerini temizle
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}
