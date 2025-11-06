using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace ProductManager.Core.DTOs.UserDTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "وارد کردن ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمی باشد")]
        [Remote(action: "IsEmailInUseForLogin", controller: "Account", ErrorMessage = "ایمیل وارد شده قبلا ثبت نام شده است")]
        public string Email { get; set; }

        [Required(ErrorMessage = "وارد کردن رمز عبور الزامی است")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
