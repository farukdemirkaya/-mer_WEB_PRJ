// =========================================================
// Controllers/MenuController.cs
// Menü sayfasını yönetir.
// Menü herkese açıktır; giriş gerekmez.
// Ancak "Sepete Ekle" butonu → giriş kontrolü OrderController'da yapılır.
// =========================================================

using LiveLifeCoffee.Services;
using Microsoft.AspNetCore.Mvc;

namespace LiveLifeCoffee.Controllers
{
    /// <summary>
    /// Menü listesine ait HTTP isteklerini işler.
    /// </summary>
    public class MenuController : Controller
    {
        // MenuService bağımlılığı; DI ile enjekte edilir
        private readonly MenuService _menuService;

        // Constructor: ASP.NET Core, MenuService'i otomatik olarak sağlar
        public MenuController(MenuService menuService)
        {
            _menuService = menuService;
        }

        // -------------------------------------------------------
        // GET: /Menu  veya  /Menu/Index
        // Tüm menü ürünlerini kategorilere göre gruplandırarak gösterir.
        // -------------------------------------------------------
        public IActionResult Index()
        {
            // Tüm ürünleri servisten al
            var allItems = _menuService.GetAll();

            // Ürünleri kategoriye göre gruplandır
            // GroupBy: aynı Category değerine sahip öğeleri bir araya toplar
            // Dictionary<string, List<MenuItem>> yapısı oluşturur
            var groupedItems = allItems
                .GroupBy(item => item.Category)
                .ToDictionary(
                    group => group.Key,      // Anahtar: kategori adı (string)
                    group => group.ToList()  // Değer: o kategorideki ürün listesi
                );

            // Gruplanmış veriyi View'a geç (ViewBag üzerinden)
            // ViewBag: Controller'dan View'a veri taşımak için dinamik nesne
            ViewBag.GroupedItems = groupedItems;

            // Menü View'ını render et
            return View();
        }
    }
}
