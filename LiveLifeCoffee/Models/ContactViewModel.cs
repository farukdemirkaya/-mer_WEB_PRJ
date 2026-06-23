// =========================================================
// Models/ContactViewModel.cs
// İletişim formundan gelen veriyi taşıyan ViewModel.
// Müşterinin mesajını ve iletişim bilgilerini içerir.
// =========================================================

using System.ComponentModel.DataAnnotations;

namespace LiveLifeCoffee.Models
{
    /// <summary>
    /// İletişim formu verilerini ve doğrulama kurallarını taşır.
    /// </summary>
    public class ContactViewModel
    {
        // Gönderenin adı soyadı zorunludur
        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir.")]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = string.Empty;

        // Gönderenin e-posta adresi zorunludur
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        // Mesajın konusu zorunludur
        [Required(ErrorMessage = "Konu zorunludur.")]
        [StringLength(200, ErrorMessage = "Konu en fazla 200 karakter olabilir.")]
        [Display(Name = "Konu")]
        public string Subject { get; set; } = string.Empty;

        // Mesajın içeriği zorunludur ve en az 10 karakter olmalıdır
        [Required(ErrorMessage = "Mesaj zorunludur.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Mesaj 10-2000 karakter arasında olmalıdır.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mesajınız")]
        public string Message { get; set; } = string.Empty;

        // Form başarıyla gönderildiğinde true olur; teşekkür mesajı göstermek için kullanılır
        public bool IsSuccess { get; set; } = false;
    }
}
