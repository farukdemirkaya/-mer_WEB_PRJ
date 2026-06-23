// =========================================================
// Models/RegisterViewModel.cs
// Kayıt Ol formundan gelen veriyi taşıyan ViewModel.
// DataAnnotations ile server-side doğrulama kuralları tanımlanır.
// ViewModel kullanmak, Domain modelini (User) formdan ayırır.
// =========================================================

using System.ComponentModel.DataAnnotations;

namespace LiveLifeCoffee.Models
{
    /// <summary>
    /// Kayıt formu verilerini ve doğrulama kurallarını taşır.
    /// </summary>
    public class RegisterViewModel
    {
        // Kullanıcının tam adı zorunludur ve en az 2, en fazla 100 karakter olmalıdır
        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ad Soyad 2-100 karakter arasında olmalıdır.")]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = string.Empty;

        // E-posta adresi zorunludur ve geçerli format kontrolü yapılır
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        // Parola zorunludur ve en az 6 karakter olmalıdır
        // DataType.Password → input type="password" olarak render edilmesini sağlar
        [Required(ErrorMessage = "Parola zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Parola en az 6 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Password { get; set; } = string.Empty;

        // Parola tekrar alanı; [Compare] ile Password alanıyla eşleşmesi zorunludur
        [Required(ErrorMessage = "Parola tekrarı zorunludur.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Parolalar eşleşmiyor.")]
        [Display(Name = "Parola Tekrar")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
