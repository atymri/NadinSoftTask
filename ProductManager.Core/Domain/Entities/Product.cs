using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManager.Core.Domain.Entities
{
    public class Product
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)] 
        public string Name { get; set; } = string.Empty;

        [Required] 
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        [RegularExpression("^[0-9]*$")]
        [StringLength(11)]
        public string ManufacturePhone { get; set; } = string.Empty;

        [Required] 
        [StringLength(100)]
        public string ManufactureEmail { get; set; } = string.Empty;

        [Required]
        public uint Count { get; set; } = 0;
        [Required]
        public bool IsAvailable => Count > 0;

    }
}
