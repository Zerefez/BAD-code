using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ExperienceService.Models
{
    public class Provider
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [StringLength(20)]
        public string Number { get; set; }

        [StringLength(100)]
        public string TouristicOperatorPermit { get; set; }

        [BsonIgnore]
        public List<Service> Services { get; set; } = new();

        [BsonIgnore]
        public List<Billing> Billings { get; set; } = new();
    }
}