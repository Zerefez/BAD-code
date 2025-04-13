using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ExperienceService.Models
{
    public class Billing
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        public int Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string GuestId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ProviderId { get; set; }

        [BsonIgnore]
        public Guest Guest { get; set; }

        [BsonIgnore]
        public Provider Provider { get; set; }
    }
}
