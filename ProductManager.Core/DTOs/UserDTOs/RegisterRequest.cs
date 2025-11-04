using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ProductManager.Core.DTOs.UserDTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "وارد کردن نام الزامی است")]
        [StringLength(40, ErrorMessage = "حداکثر کاراکتر های مجاز برای نام کاربر 40 کاراکتر است")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "وارد کردن ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمی باشد")]
        [Remote(action: "IsEmailInUseForRegister", controller: "Account", ErrorMessage = "ایمیل وارد شده قبلا ثبت نام شده است")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "وارد کردن شماره تلفن الزامی است")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "تنها اعداد برای شماره تلفن مجاز هستند")]
        [StringLength(11, ErrorMessage = "شماره تلفن نمیتواند بیشتر از 11 کاراکتر باشد")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "وارد کردن رمز عبور الزامی است")]
        public string Password { get; set; }

        [Required(ErrorMessage = "تکرار رمز عبور الزامی است")]
        [Compare(nameof(Password), ErrorMessage = "رمز های عبور مطابقت ندارند")]
        public string ConfirmPassword { get; set; }
    }
}
