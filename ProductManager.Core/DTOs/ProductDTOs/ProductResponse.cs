using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManager.Core.DTOs.ProductDTOs
{
    public class ProductResponse
    {
        public Guid ID { get; set; }
        public string? Name { get; set; }
        public DateTime? Date { get; set; }
        public string? ManufacturePhone { get; set; }
        public string? ManufactureEmail { get; set; }
        public uint? Count { get; set; } = 0;
        public bool? IsAvailable => Count > 0;
    }
}
