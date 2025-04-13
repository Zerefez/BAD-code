using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExperienceService.Models
{
    public class Role
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string Name { get; set; }
    }
    
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Provider = "Provider";
        public const string Guest = "Guest";
        
        public static readonly string[] AllRoles = new[] { Admin, Manager, Provider, Guest };
    }
} 