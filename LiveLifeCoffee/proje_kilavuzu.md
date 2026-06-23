# ☕ LiveLife Coffee - Proje Geliştirici Rehberi

Bu belge, **LiveLife Coffee** projesinin nasıl çalıştığını, hangi dosyanın ne işe yaradığını ve gelecekte yapmak isteyebileceğiniz değişiklikleri (yeni özellik ekleme, tasarım değiştirme vb.) nasıl yapacağınızı anlatan kapsamlı bir rehberdir.

---

## 📁 1. Proje Klasör Yapısı ve Mimarisi

Proje **ASP.NET Core MVC (Model-View-Controller)** mimarisiyle ve veri depolama için **Entity Framework Core (SQLite)** kullanılarak geliştirilmiştir. 

- **M (Model):** Veri yapılarını ve kurallarını temsil eder.
- **V (View):** Kullanıcının ekranda gördüğü HTML arayüzleridir.
- **C (Controller):** Kullanıcıdan gelen istekleri alır, işler (veriyi kaydeder/okur) ve ilgili View'a gönderir.

### 🗂️ Klasörlerin Görevleri

#### `Models/` (Veri ve Form Modelleri)
Veritabanına kaydedilecek nesneleri veya formlardan alınacak verilerin kurallarını tanımlar.
* `User.cs` : Müşteri bilgileri (Ad, E-posta, Şifre özeti, Kayıt Tarihi).
* `MenuItem.cs` : Kahve ve tatlıların özellikleri (Ad, Fiyat, Kategori, Stok durumu).
* `Order.cs` : Tamamlanmış siparişler (Tarih, Tutar, Alan Kişi, Durum).
* `CartItem.cs` : Sepete eklenen ve siparişe bağlanan ürünler (Sipariş ile ilişkili tutulur).
* `ContactMessage.cs` : İletişim formundan gelen mesajlar (Okunma durumu, Tarih).
* `*ViewModel.cs` uzantılı dosyalar: Ekranda gösterilen formların (Giriş yap, Kayıt ol, Ürün ekle) yapısını ve zorunlu alan kurallarını tutar.

#### `Data/` (Veritabanı Katmanı)
* `AppDbContext.cs` : Entity Framework Core'un kalbidir. Veritabanı tablolarını (`Users`, `MenuItems`, `Orders` vb.) tanımlar ve sistem ilk çalıştığında örnek kahvelerin veritabanına yazılmasını (Seed Data) sağlar.

#### `Services/` (Arka Plan İş Mantığı)
Veritabanı (`AppDbContext`) ile Controller'lar arasındaki köprüdür. MVC mimarisinin "kalın controller" (fat controller) olmasını engeller.
* `UserService.cs` : Kayıt olma, şifre doğrulama ve kullanıcı getirme işlemlerini veritabanı üzerinden yapar.
* `MenuService.cs` : Menüdeki ürünleri getirir, adminin yeni ürün eklemesini veya silmesini doğrudan SQLite veritabanına yansıtır.
* `OrderService.cs` : Tamamlanmış siparişleri kaydeder (`Include` kullanarak sepet öğeleriyle beraber) ve adminin görmesini sağlar.
* `ContactMessageService.cs` : İletişim mesajlarını SQLite'a kaydeder ve admin okuduğunda veritabanında okundu olarak işaretler.

#### `Controllers/` (Yönlendiriciler / Beyin)
Kullanıcının tıkladığı her link buraya gelir.
* `HomeController.cs` : Ana sayfa (`/`) ve İletişim (`/Home/Contact`) sayfalarını yönetir.
* `AccountController.cs` : Giriş yapma ve Kayıt olma işlemlerini yönetir. Admin giriş yaptığında onu fark edip panele yönlendiren kod buradadır.
* `MenuController.cs` : Müşterilere menüyü listeler.
* `OrderController.cs` : Sepete ekleme, sepeti görüntüleme ve siparişi tamamlama adımlarını yönetir.
* `AdminController.cs` : Admin panelindeki dashboard, siparişleri görme, mesaj okuma ve ürün ekleme/çıkarma sayfalarını yönetir. **Sadece admin olanlar** buraya erişebilir.

#### `Views/` (Kullanıcı Arayüzü / HTML)
Her Controller'ın kendi adında bir klasörü vardır.
* `Shared/_Layout.cshtml` : Sitenin iskeletidir. **Üst menü (Navbar)** ve **Alt kısım (Footer)** buradadır. Tüm sayfalar bu iskeletin içine yerleşir.
* `Home/`, `Account/`, `Menu/`, `Order/`, `Admin/` : İlgili bölümlerin sayfalarıdır. (Örn: `Views/Home/Index.cshtml` ana sayfanın tasarımıdır).

#### `wwwroot/` (Statik Dosyalar)
* `css/site.css` : Sitenin tüm tasarım kodları (renkler, buton şekilleri, tema ayarları) buradadır.

#### `Program.cs` (Ayar Dosyası)
Projenin kalbidir. Veritabanı bağlantısı (`AddDbContext`), Servislerin her istekte yeniden oluşturulması (`AddScoped`), Session (oturum) sürelerinin ayarlanması burada yapılır.

---

## 🛠️ 2. Nasıl Değişiklik Yapılır? (Sık Sorulan Sorular)

### 🎨 Renkleri veya Tasarımı Nasıl Değiştiririm?
1. `wwwroot/css/site.css` dosyasını açın.
2. En üstteki `:root` bloğunu bulun. Bütün renkler burada tanımlıdır.
   Örneğin ana yeşil rengi değiştirmek için `--color-primary: #15803d;` değerini istediğiniz bir renk koduyla değiştirebilirsiniz. Değiştirdiğiniz anda sitedeki butonlar, başlıklar ve menüler otomatik değişir.

### 🌐 Üst Menüye (Navbar) Yeni Bir Link Nasıl Eklerim?
1. `Views/Shared/_Layout.cshtml` dosyasını açın.
2. `<ul class="navbar-menu">` bölümünü bulun.
3. Araya yeni bir `<li>` elemanı ekleyin. 
   Örnek: `<li><a href="/Home/Hakkimizda">Hakkımızda</a></li>`

### ☕ Yeni Bir Sayfa Nasıl Eklerim? (Örn: Hakkımızda Sayfası)
1. **Controller'a Metot Ekle:** `Controllers/HomeController.cs` içine gidin ve şu kodu ekleyin:
   ```csharp
   public IActionResult Hakkimizda() {
       return View();
   }
   ```
2. **View Klasörü Oluştur:** `Views/Home/` klasörü içine `Hakkimizda.cshtml` adında yeni bir dosya oluşturun.
3. İçine HTML kodlarınızı yazın:
   ```html
   @{ ViewData["Title"] = "Hakkımızda"; }
   <h1>Biz Kimiz?</h1>
   <p>LiveLife Coffee 2026 yılında kuruldu...</p>
   ```

### 🔒 Admin Parolasını Nasıl Değiştiririm?
1. `Controllers/AccountController.cs` dosyasını açın.
2. Üst kısımlarda yer alan şu kodları bulun ve değiştirin:
   ```csharp
   private const string AdminEmail    = "admin@livelife.com";
   private const string AdminPassword = "admin1234";
   ```

### 🗄️ Veritabanını Nasıl Yönetirim veya Değiştiririm?
Şu an sistem verileri yerel bir **SQLite** veritabanı dosyasında (`livelife.db`) kalıcı olarak tutmaktadır.
1. Yeni bir tablo veya sütun eklemek isterseniz, `Models/` altındaki ilgili sınıfa yeni özelliğinizi (Property) ekleyin.
2. Ardından terminalde/konsolda `dotnet ef migrations add YeniOzellikEklendi` yazarak bir göç (migration) oluşturun.
3. `dotnet ef database update` yazarak veritabanınızı güncelleyin.
*Not: İleride SQL Server, PostgreSQL veya MySQL'e geçmek isterseniz `Program.cs` içindeki `UseSqlite()` kısmını `UseSqlServer()` vb. olarak değiştirip ilgili NuGet paketini yüklemeniz yeterlidir. Mimari tamamen buna uyumludur.*

### 🛒 Kargo veya Ödeme Adımı Eklemek İstersem?
1. `Models/Order.cs` içerisine `public string Address { get; set; }` gibi yeni alanlar ekleyin ve veritabanını güncelleyin (Migration).
2. `Views/Order/Cart.cshtml` sayfasında "Siparişi Tamamla" butonunun yanına bir adres veya kart giriş formu çizin.
3. `Controllers/OrderController.cs` içindeki `Checkout` metodunu güncelleyip bu bilgileri teslim alın ve veritabanına yazılması için OrderService'e iletin.

---

## 🚀 3. Karşılaşılabilecek Hatalar ve Çözümleri

- **Hata:** Giriş yaptıktan sonra "Sepete Ekle" tuşu çalışmıyor veya admin paneline atıyor.
  **Çözüm:** Session süresi dolmuş olabilir. Tarayıcıyı yenileyin. Admin hesabıyla sepete ürün eklenemez, admin sadece yönetir. Sepet testleri için müşteri hesabı açmalısınız.
  
- **Hata:** Yaptığım CSS (tasarım) değişiklikleri sitede görünmüyor.
  **Çözüm:** Tarayıcınız eski CSS dosyasını önbelleğe (cache) almış olabilir. Sitedeyken `CTRL + F5` tuşlarına basarak sayfayı zorla yenileyin.

- **Hata:** Yeni ürün ekledim ama resmi yok?
  **Çözüm:** Şu anki sistemde resim yükleme özelliği yoktur. Eğer resim eklenmek istenirse `Models/MenuItem.cs` içine `public string ImageUrl { get; set; }` eklenip ürün listeleme HTML'lerine `<img>` etiketi konulmalıdır. Bunun için sunucuya dosya yükleme (File Upload) kodları yazılmalıdır.

---
*Bu dosya projenizin ana dizinine `proje_kilavuzu.md` olarak kaydedilmiştir. İstediğiniz zaman açıp okuyabilirsiniz.*
