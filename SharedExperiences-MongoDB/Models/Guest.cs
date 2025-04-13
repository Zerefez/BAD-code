using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ExperienceService.Models
{
    public class Guest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string Number { get; set; }

        public int Age { get; set; }

        [BsonIgnore]
        public List<SharedExperience> SharedExperiences { get; set; } = new();

        [BsonIgnore]
        public List<Billing> Billings { get; set; } = new();
    }
}