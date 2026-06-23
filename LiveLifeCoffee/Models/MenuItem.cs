// =========================================================
// Models/MenuItem.cs
// Kafenin menüsündeki bir ürünü (kahve, içecek, yiyecek vb.)
// temsil eden model sınıfı.
// =========================================================

namespace LiveLifeCoffee.Models
{
    /// <summary>
    /// Menüdeki bir ürünü (kahve, içecek, atıştırmalık vb.) temsil eder.
    /// </summary>
    public class MenuItem
    {
        // Ürünün benzersiz kimlik numarası
        public int Id { get; set; }

        // Ürünün adı (örnek: "Filtre Kahve", "Latte")
        public string Name { get; set; } = string.Empty;

        // Ürünün kısa açıklaması (müşteriye gösterilir)
        public string Description { get; set; } = string.Empty;

        // Ürünün fiyatı (TL cinsinden)
        public decimal Price { get; set; }

        // Ürünün kategorisi (örnek: "Sıcak İçecekler", "Soğuk İçecekler", "Atıştırmalıklar")
        public string Category { get; set; } = string.Empty;

        // Ürünün stokta olup olmadığını belirtir (false ise sipariş verilemez)
        public bool IsAvailable { get; set; } = true;
    }
}
