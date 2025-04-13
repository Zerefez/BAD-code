using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SharedExperiences.Models
{
    public class LogEntry
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;
        
        [BsonElement("Timestamp")]
        public DateTime Timestamp { get; set; }
        
        [BsonElement("Level")]
        public string Level { get; set; } = null!;
        
        [BsonElement("MessageTemplate")]
        public string MessageTemplate { get; set; } = null!;
        
        [BsonElement("RenderedMessage")]
        public string RenderedMessage { get; set; } = null!;
        
        [BsonElement("Properties")]
        public LogProperties Properties { get; set; } = null!;
        
        [BsonElement("Exception")]
        public string? Exception { get; set; }
    }
    
    public class LogProperties
    {
        [BsonElement("RequestMethod")]
        public string? RequestMethod { get; set; }
        
        [BsonElement("RequestPath")]
        public string? RequestPath { get; set; }
        
        [BsonElement("UserAgent")]
        public string? UserAgent { get; set; }
        
        [BsonElement("UserId")]
        public string? UserId { get; set; }
        
        [BsonElement("RequestHost")]
        public string? RequestHost { get; set; }
        
        [BsonElement("SourceContext")]
        public string? SourceContext { get; set; }
        
        [BsonElement("ActionId")]
        public string? ActionId { get; set; }
        
        [BsonElement("ActionName")]
        public string? ActionName { get; set; }
        
        [BsonElement("RequestId")]
        public string? RequestId { get; set; }
        
        [BsonElement("ConnectionId")]
        public string? ConnectionId { get; set; }
        
        [BsonElement("MachineName")]
        public string? MachineName { get; set; }
        
        [BsonElement("ThreadId")]
        public string? ThreadId { get; set; }
    }
} 