// =========================================================
// Models/MenuItemViewModel.cs
// Admin panelinde menü ürünü ekleme ve düzenleme formu için ViewModel.
// DataAnnotations ile sunucu taraflı doğrulama kurallarını içerir.
// =========================================================

using System.ComponentModel.DataAnnotations;

namespace LiveLifeCoffee.Models
{
    /// <summary>
    /// Menü ürünü ekleme / düzenleme formunu taşıyan ViewModel.
    /// </summary>
    public class MenuItemViewModel
    {
        // Düzenleme işleminde ürünün mevcut ID'si; yeni ürün için 0
        public int Id { get; set; }

        // Ürün adı zorunludur
        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ürün adı 2-100 karakter arasında olmalıdır.")]
        [Display(Name = "Ürün Adı")]
        public string Name { get; set; } = string.Empty;

        // Açıklama zorunludur
        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Açıklama 5-500 karakter arasında olmalıdır.")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        // Fiyat zorunludur ve sıfırdan büyük olmalıdır
        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0.01, 9999.99, ErrorMessage = "Fiyat 0,01 ile 9999,99 arasında olmalıdır.")]
        [Display(Name = "Fiyat (₺)")]
        public decimal Price { get; set; }

        // Kategori zorunludur
        [Required(ErrorMessage = "Kategori seçiniz.")]
        [Display(Name = "Kategori")]
        public string Category { get; set; } = string.Empty;

        // Ürün stokta var mı? (checkbox)
        [Display(Name = "Stokta Mevcut")]
        public bool IsAvailable { get; set; } = true;
    }
}
