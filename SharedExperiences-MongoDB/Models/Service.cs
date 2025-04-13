using ExperienceService.Models.Validators;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ExperienceService.Models
{
    public class Service
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
        
        [Required]
        [PositivePrice(ErrorMessage = "Price must be greater than or equal to 0")]
        public int Price { get; set; }

        public DateTime Date { get; set; }

        // Navigation properties
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProviderId { get; set; }
        
        [BsonIgnore]
        public Provider Provider { get; set; }
        
        [BsonIgnore]
        public List<SharedExperience> SharedExperiences { get; set; } = new();

        [BsonIgnore]
        public List<Guest> Guests { get; set; } = new();
        
        [BsonIgnore]
        public Discount Discount { get; set; }
    }
}