// =========================================================
// Controllers/OrderController.cs
// Sepet ve sipariş işlemlerinin merkezi.
// Bu Controller'daki TÜM action'lar oturum kontrolüne tabidir.
// Giriş yapmayan kullanıcı bu sayfaların hiçbirine erişemez.
//
// Sepet Mantığı:
// - Sepet, kullanıcının Session'ında JSON olarak saklanır.
// - Her "Sepete Ekle" işleminde JSON deserialize → güncelle → serialize edilir.
// - Kullanıcı oturumu kapatana ya da sipariş tamamlanana kadar korunur.
// =========================================================

using LiveLifeCoffee.Models;
using LiveLifeCoffee.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LiveLifeCoffee.Controllers
{
    /// <summary>
    /// Sepet yönetimi ve sipariş oluşturma işlemlerine ait HTTP isteklerini işler.
    /// </summary>
    public class OrderController : Controller
    {
        // Session'da sepet verisini saklamak için kullanılan anahtar sabiti
        // Sabit kullanmak: yazım hatalarını önler, değiştirmek kolaylaşır
        private const string CartSessionKey = "ShoppingCart";

        // Servis bağımlılıkları; DI ile enjekte edilir
        private readonly MenuService _menuService;
        private readonly OrderService _orderService;
        private readonly UserService _userService;

        // Constructor: ASP.NET Core gerekli servisleri otomatik sağlar
        public OrderController(MenuService menuService, OrderService orderService, UserService userService)
        {
            _menuService = menuService;
            _orderService = orderService;
            _userService = userService;
        }

        // -------------------------------------------------------
        // PRIVATE YARDIMCI METOT: Session'dan sepet verisini okur.
        // Session'da veri yoksa boş liste döndürür.
        // JSON serialize/deserialize tüm sepet işlemlerinde bu metot kullanılır.
        // -------------------------------------------------------
        private List<CartItem> GetCartFromSession()
        {
            // Session'dan "ShoppingCart" anahtarlı veriyi string olarak al
            string? cartJson = HttpContext.Session.GetString(CartSessionKey);

            // JSON null veya boşsa yeni boş liste döndür
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }

            // JSON string'i CartItem listesine deserialize et
            // ?? new List<CartItem>(): Deserialize null dönerse boş liste kullan
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson)
                   ?? new List<CartItem>();
        }

        // -------------------------------------------------------
        // PRIVATE YARDIMCI METOT: Sepet verisini Session'a yazar.
        // Her sepet değişikliğinden sonra bu metot çağrılmalıdır.
        // -------------------------------------------------------
        private void SaveCartToSession(List<CartItem> cart)
        {
            // Sepet listesini JSON string'e serialize et
            string cartJson = JsonSerializer.Serialize(cart);

            // Session'a yaz
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }

        // -------------------------------------------------------
        // PRIVATE YARDIMCI METOT: Oturum kontrolü.
        // Session'da UserId var mı kontrol eder.
        // Yoksa false döner ve Controller action'ı yönlendirme yapar.
        // -------------------------------------------------------
        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId").HasValue;
        }

        // -------------------------------------------------------
        // GET: /Order/Cart
        // Kullanıcının mevcut sepetini gösterir.
        // GİRİŞ ZORUNLU: Giriş yapılmamışsa Login'e yönlendir.
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult Cart()
        {
            // Oturum kontrolü
            if (!IsUserLoggedIn())
            {
                // Giriş sonrası geri dönmek için mevcut URL'i TempData'ya kaydet
                TempData["ReturnUrl"] = Url.Action("Cart", "Order");
                return RedirectToAction("Login", "Account");
            }

            // Session'dan sepeti al
            var cart = GetCartFromSession();

            // Sepeti View'a geç
            return View(cart);
        }

        // -------------------------------------------------------
        // POST: /Order/AddToCart
        // Menüden bir ürünü sepete ekler ya da miktarını artırır.
        // GİRİŞ ZORUNLU.
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]  // CSRF koruması
        public IActionResult AddToCart(int menuItemId, int quantity = 1)
        {
            // Oturum kontrolü
            if (!IsUserLoggedIn())
            {
                // Kullanıcı giriş yaptıktan sonra sepet sayfasına dönmesi için yönlendirme
                TempData["ReturnUrl"] = Url.Action("Cart", "Order");
                return RedirectToAction("Login", "Account");
            }

            // Miktar geçerli mi kontrol et (en az 1 olmalı)
            if (quantity < 1)
            {
                // Hatalı istek; kötü niyetli veri manipülasyonuna karşı savunma
                return BadRequest("Geçersiz miktar.");
            }

            // Menüde ürün var mı?
            var menuItem = _menuService.GetById(menuItemId);
            if (menuItem == null)
            {
                // Ürün bulunamadı; 404 döndür
                return NotFound("Ürün bulunamadı.");
            }

            // Ürün stokta var mı?
            if (!menuItem.IsAvailable)
            {
                // Stokta yok; kullanıcıyı menüye geri yönlendir
                TempData["ErrorMessage"] = $"'{menuItem.Name}' şu an stokta bulunmamaktadır.";
                return RedirectToAction("Index", "Menu");
            }

            // Session'dan mevcut sepeti al
            var cart = GetCartFromSession();

            // Ürün sepette zaten var mı?
            var existingItem = cart.FirstOrDefault(c => c.MenuItemId == menuItemId);

            if (existingItem != null)
            {
                // Varsa miktarını artır (yeni ekleme yerine güncelleme)
                existingItem.Quantity += quantity;
            }
            else
            {
                // Yoksa yeni kalem oluştur ve sepete ekle
                cart.Add(new CartItem
                {
                    MenuItemId = menuItem.Id,
                    Name = menuItem.Name,
                    UnitPrice = menuItem.Price,  // O anki fiyatı kilitle
                    Quantity = quantity
                });
            }

            // Güncellenmiş sepeti Session'a kaydet
            SaveCartToSession(cart);

            // Başarı mesajı
            TempData["SuccessMessage"] = $"'{menuItem.Name}' sepete eklendi!";

            // Kullanıcıyı sepet sayfasına yönlendir
            return RedirectToAction("Cart");
        }

        // -------------------------------------------------------
        // POST: /Order/RemoveFromCart
        // Belirtilen ürünü sepetten tamamen kaldırır.
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int menuItemId)
        {
            // Oturum kontrolü
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // Sepeti al
            var cart = GetCartFromSession();

            // Belirtilen ürünü bul ve listeden kaldır
            var itemToRemove = cart.FirstOrDefault(c => c.MenuItemId == menuItemId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
            }

            // Güncellenmiş sepeti kaydet
            SaveCartToSession(cart);

            // Sepet sayfasına geri dön
            return RedirectToAction("Cart");
        }

        // -------------------------------------------------------
        // POST: /Order/UpdateQuantity
        // Sepetteki bir ürünün miktarını günceller.
        // Miktar 0 veya negatif gelirse ürün sepetten kaldırılır.
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int menuItemId, int quantity)
        {
            // Oturum kontrolü
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // Sepeti al
            var cart = GetCartFromSession();

            // Güncellenecek ürünü bul
            var item = cart.FirstOrDefault(c => c.MenuItemId == menuItemId);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    // Miktar geçersizse ürünü sepetten kaldır
                    cart.Remove(item);
                }
                else
                {
                    // Geçerli miktarsa güncelle
                    item.Quantity = quantity;
                }
            }

            // Sepeti kaydet
            SaveCartToSession(cart);

            return RedirectToAction("Cart");
        }

        // -------------------------------------------------------
        // POST: /Order/Checkout
        // Sipariş tamamlama işlemi.
        // Sepet doğrulandıktan sonra OrderService'e kaydedilir.
        // Sepet temizlenir ve onay sayfasına yönlendirilir.
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout()
        {
            // Oturum kontrolü
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // Session'dan sepeti al
            var cart = GetCartFromSession();

            // Sepet boşsa siparişi reddet
            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "Sepetiniz boş. Sipariş veremezsiniz.";
                return RedirectToAction("Cart");
            }

            // Session'dan kullanıcı bilgilerini al
            int userId = HttpContext.Session.GetInt32("UserId")!.Value;
            string userFullName = HttpContext.Session.GetString("UserFullName") ?? "Bilinmeyen";

            // OrderService üzerinden siparişi kaydet
            Order createdOrder = _orderService.CreateOrder(userId, userFullName, cart);

            // Sipariş tamamlandı; sepeti temizle
            // Session'dan sadece sepet anahtarını kaldır (kullanıcı oturumu açık kalır)
            HttpContext.Session.Remove(CartSessionKey);

            // Sipariş onay sayfasına yönlendir; sipariş ID'si URL'e eklenir
            return RedirectToAction("Confirmation", new { orderId = createdOrder.Id });
        }

        // -------------------------------------------------------
        // GET: /Order/Confirmation/{orderId}
        // Sipariş onay sayfasını gösterir.
        // Siparişi yalnızca o siparişi veren kullanıcı görebilir.
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult Confirmation(int orderId)
        {
            // Oturum kontrolü
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // Siparişi OrderService'den al
            var order = _orderService.GetById(orderId);

            // Sipariş bulunamadıysa 404
            if (order == null)
            {
                return NotFound("Sipariş bulunamadı.");
            }

            // GÜVENLİK: Başkasının siparişini görüntülemeyi engelle
            int currentUserId = HttpContext.Session.GetInt32("UserId")!.Value;
            if (order.UserId != currentUserId)
            {
                // Yetkisiz erişim; ana sayfaya yönlendir
                return RedirectToAction("Index", "Home");
            }

            // Siparişi View'a geç
            return View(order);
        }
    }
}
