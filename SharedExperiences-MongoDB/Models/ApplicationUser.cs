using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ExperienceService.Models
{
    public class ApplicationUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string Username { get; set; }
        
        public string Email { get; set; }
        
        public string PasswordHash { get; set; }
        
        public List<string> Roles { get; set; } = new List<string>();
    }
} 