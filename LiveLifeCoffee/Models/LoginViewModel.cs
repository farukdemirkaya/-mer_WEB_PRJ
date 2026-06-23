// =========================================================
// Models/LoginViewModel.cs
// Giriş Yap formundan gelen veriyi taşıyan ViewModel.
// Sadece doğrulama için gerekli e-posta ve parola alanlarını içerir.
// =========================================================

using System.ComponentModel.DataAnnotations;

namespace LiveLifeCoffee.Models
{
    /// <summary>
    /// Giriş formu verilerini ve doğrulama kurallarını taşır.
    /// </summary>
    public class LoginViewModel
    {
        // E-posta alanı zorunludur ve geçerli format kontrolü yapılır
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        // Parola alanı zorunludur
        // DataType.Password ile tarayıcı bu alanı gizli olarak gösterir
        [Required(ErrorMessage = "Parola zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Password { get; set; } = string.Empty;

        // Kullanıcı başarısız giriş denemesinde geri döndürülen hata mesajı
        // Bu alan formdan gelmez; Controller tarafından set edilir
        public string? ErrorMessage { get; set; }
    }
}
