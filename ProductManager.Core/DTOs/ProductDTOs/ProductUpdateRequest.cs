using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductManager.Core.Domain.Entities;

namespace ProductManager.Core.DTOs.ProductDTOs
{
    public class ProductUpdateRequest
    {
        [Required(ErrorMessage = "نام محصول نمیتواند خالی باشد")]
        [StringLength(50, ErrorMessage = "حداکثر کاراکتر های مجاز برای نام محصول 50 کاراکتر میباشد")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "وارد کردن تعداد محصول الزامی است")]
        [Range(1, 100, ErrorMessage = "حداقل تعداد محصول 1 و حداکثر تعداد محصول 100 عدد میباشد")]
        public uint Count { get; set; } = 0;
        public bool IsAvailable => Count > 0;
    }
}
