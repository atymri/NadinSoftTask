using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace ProductManager.Core.Domain.Entities
{
    public class Product
    {
        [Key]
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid ID { get; set; } = Guid.NewGuid();

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
