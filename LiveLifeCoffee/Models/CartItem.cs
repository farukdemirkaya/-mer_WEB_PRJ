// =========================================================
// Models/CartItem.cs
// Kullanıcının sepetindeki tek bir kalemi temsil eder.
// Sepet, Session içinde CartItem listesi olarak saklanır.
// =========================================================

namespace LiveLifeCoffee.Models
{
    /// <summary>
    /// Kullanıcının alışveriş sepetindeki bir ürün kalemini temsil eder.
    /// </summary>
    public class CartItem
    {
        // Veritabanında (EF Core) bu satırın benzersiz kimliği
        public int Id { get; set; }

        // Hangi siparişe ait olduğunu belirten dış anahtar (Foreign Key)
        public int OrderId { get; set; }

        // Sepete eklenen ürünün menüdeki benzersiz kimlik numarası
        public int MenuItemId { get; set; }

        // Ürünün adı (görüntüleme amaçlı, MenuItem'dan kopyalanır)
        public string Name { get; set; } = string.Empty;

        // Sepete eklendiği andaki birim fiyat (TL)
        // Sonradan fiyat değişse bile sipariş fiyatı sabit kalır
        public decimal UnitPrice { get; set; }

        // Bu üründen kaç adet sipariş edildiği
        public int Quantity { get; set; }

        // Hesaplanmış toplam tutar: UnitPrice * Quantity
        // get-only property; ayrıca saklanmaz, her hesaplamada anlık üretilir
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}
