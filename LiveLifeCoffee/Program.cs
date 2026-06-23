// =========================================================
// Program.cs
// Uygulamanın başlangıç noktası.
// Burada:
//   - Servisler Dependency Injection (DI) container'a kayıt edilir
//   - Session yapılandırılır
//   - HTTP pipeline (middleware) kurulur
//   - Uygulama başlatılır
// =========================================================

using LiveLifeCoffee.Services;

using LiveLifeCoffee.Data;
using Microsoft.EntityFrameworkCore;

// WebApplication builder nesnesi oluştur
// Bu nesne üzerinden servisler ve yapılandırma eklenir
var builder = WebApplication.CreateBuilder(args);

// =========================================================
// SERVİSLER (Dependency Injection Container'a Kayıt)
// =========================================================

// MVC Controller + View desteğini ekle
// Bu satır olmadan Controller ve View'lar çalışmaz
builder.Services.AddControllersWithViews();

// Entity Framework Core - SQLite Veritabanı Bağlamını Ekle
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=livelife.db"));

// Scoped: Her HTTP isteğinde yeni bir örnek oluşturulur (Veritabanı işlemleri için standarttır)
// Eskiden bellek (RAM) kullandığımız için Singleton'dı. Şimdi veritabanına geçtiğimiz için Scoped yapıyoruz.

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ContactMessageService>();
builder.Services.AddScoped<JobApplicationService>(); // İş başvurusu servisi (YENİ)

// Session için gerekli altyapıyı ekle
// Session, kullanıcıya özgü geçici veriyi sunucuda tutar
builder.Services.AddSession(options =>
{
    // Session çerezinin kullanıcı tarafından JavaScript ile okunamaz hale gelmesi
    // XSS (Cross-Site Scripting) saldırılarına karşı koruma
    options.Cookie.HttpOnly = true;

    // Session çerezinin zorunlu olarak işaretlenmesi (GDPR uyumu için false tutulabilir)
    options.Cookie.IsEssential = true;

    // Session'ın kullanılmadan ne kadar süre geçerliliğini koruyacağı (20 dakika)
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});

// =========================================================
// UYGULAMA YAPIMI (app = WebApplication)
// =========================================================

var app = builder.Build();

// Geliştirme ortamında değilsek hata yönetim sayfasını kullan
if (!app.Environment.IsDevelopment())
{
    // Kullanıcıya özel hata sayfası göster (/Home/Error)
    app.UseExceptionHandler("/Home/Error");

    // HTTP Strict Transport Security (HSTS) başlığı ekler
    app.UseHsts();
}

// HTTP isteklerini HTTPS'e yönlendir
app.UseHttpsRedirection();

// wwwroot klasöründeki statik dosyaların (CSS, JS, resimler) sunulmasını sağla
app.UseStaticFiles();

// URL yönlendirme sistemini etkinleştir
app.UseRouting();

// Session middleware'ini pipeline'a ekle
// UseRouting'den SONRA, MapControllerRoute'dan ÖNCE olmalı
app.UseSession();

// Yetkilendirme middleware (şu an temel kullanım, ileride genişletilebilir)
app.UseAuthorization();

// Varsayılan MVC rota tanımı
// URL formatı: /{controller}/{action}/{id?}
// Varsayılan: HomeController.Index()
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Uygulamayı başlat ve HTTP isteklerini dinlemeye başla
app.Run();
