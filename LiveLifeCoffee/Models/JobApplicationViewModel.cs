// =========================================================
// Models/JobApplicationViewModel.cs
// İŞ BAŞVURUSU FORM VİEW MODELİ
//
// View katmanında kullanılan form doğrulama modelidir.
// JobApplication domain modelinden ayrı tutulur; bu sayede
// form validasyonu ve domain mantığı birbirinden bağımsız kalır.
// =========================================================

using System.ComponentModel.DataAnnotations; // Doğrulama attribute'ları için

namespace LiveLifeCoffee.Models
{
    /// <summary>
    /// İş başvurusu formunun veri bağlama ve doğrulama modelidir.
    /// </summary>
    public class JobApplicationViewModel
    {
        // Ad Soyad: zorunlu, maksimum 100 karakter
        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
        [MaxLength(100, ErrorMessage = "Ad Soyad 100 karakteri geçemez.")]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = string.Empty;

        // E-posta: zorunlu, geçerli e-posta formatı
        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        [MaxLength(200)]
        [Display(Name = "E-posta Adresi")]
        public string Email { get; set; } = string.Empty;

        // Telefon: isteğe bağlı, en fazla 20 karakter
        [MaxLength(20, ErrorMessage = "Telefon numarası 20 karakteri geçemez.")]
        [Display(Name = "Telefon Numarası")]
        public string Phone { get; set; } = string.Empty;

        // Başvurulan pozisyon: zorunlu, listeden seçilir
        [Required(ErrorMessage = "Lütfen başvurmak istediğiniz pozisyonu seçin.")]
        [Display(Name = "Başvurulan Pozisyon")]
        public string Position { get; set; } = string.Empty;

        // Motivasyon mektubu: zorunlu, minimum 50 karakter (kendini anlatsın)
        [Required(ErrorMessage = "Lütfen kendinizi kısaca tanıtın.")]
        [MinLength(50, ErrorMessage = "Lütfen en az 50 karakter yazın.")]
        [MaxLength(2000, ErrorMessage = "Mektup 2000 karakteri geçemez.")]
        [Display(Name = "Kendinizi Tanıtın")]
        public string CoverLetter { get; set; } = string.Empty;

        // Form başarıyla gönderildi mi? (teşekkür mesajı için)
        public bool IsSuccess { get; set; } = false;
    }
}
