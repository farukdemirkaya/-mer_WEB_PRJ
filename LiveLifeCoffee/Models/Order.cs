// =========================================================
// Models/Order.cs
// Tamamlanmış bir siparişi temsil eden model.
// Kullanıcı sepetini onayladığında yeni bir Order nesnesi
// oluşturulur ve OrderService üzerinden saklanır.
// =========================================================

namespace LiveLifeCoffee.Models
{
    /// <summary>
    /// Kullanıcının tamamladığı bir siparişi temsil eder.
    /// </summary>
    public class Order
    {
        // Siparişin benzersiz kimlik numarası (otomatik atanır)
        public int Id { get; set; }

        // Siparişi veren kullanıcının kimlik numarası (User.Id ile eşleşir)
        public int UserId { get; set; }

        // Siparişi veren kullanıcının adı (görüntüleme amaçlı kopyalanır)
        public string UserFullName { get; set; } = string.Empty;

        // Siparişe ait ürün kalemleri listesi
        // Bu liste, kullanıcının o anki sepetinin bir kopyasıdır
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        // Siparişin toplam tutarı (TL)
        // Tüm kalemlerin TotalPrice değerlerinin toplamı
        public decimal GrandTotal { get; set; }

        // Siparişin verildiği tarih ve saat
        public DateTime OrderedAt { get; set; } = DateTime.Now;

        // Siparişin mevcut durumu
        // "Hazırlanıyor", "Teslim Edildi", "İptal Edildi" gibi değerler alabilir
        public string Status { get; set; } = "Hazırlanıyor";
    }
}
