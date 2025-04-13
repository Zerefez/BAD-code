using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ExperienceService.Models
{
    public class Discount
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ServiceId { get; set; }

        public int GuestCount { get; set; }

        [Range(0, 100)]
        public int Percentage { get; set; }

        [BsonIgnore]
        public Service Service { get; set; }
    }
}