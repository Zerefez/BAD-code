using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ExperienceService.Models
{
    public class SharedExperience
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public DateTime Date { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> ServiceIds { get; set; } = new();

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> GuestIds { get; set; } = new();

        [BsonIgnore]
        public List<Service> Services { get; set; } = new();

        [BsonIgnore]
        public List<Guest> Guests { get; set; } = new();
    }
}