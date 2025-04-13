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
        
        // Helper property to extract action description from rendered message or properties
        [BsonIgnore]
        public string ActionDescription => ExtractActionDescription();
        
        private string ExtractActionDescription()
        {
            // Try to extract description from RenderedMessage (e.g., "HTTP POST /api/users - Creating new user")
            if (!string.IsNullOrEmpty(RenderedMessage))
            {
                // Check if there's a dash followed by description
                int dashIndex = RenderedMessage.IndexOf(" - ");
                if (dashIndex > 0 && dashIndex < RenderedMessage.Length - 3)
                {
                    return RenderedMessage.Substring(dashIndex + 3).Trim();
                }
            }
            
            // Fall back to basic method + path format
            return $"{Properties?.RequestMethod ?? "Unknown"} {Properties?.RequestPath ?? "Unknown"}";
        }
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
        
        [BsonElement("Description")]
        public string? Description { get; set; }
        
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