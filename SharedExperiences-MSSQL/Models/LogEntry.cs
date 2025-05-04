using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SharedExperiences.Models
{
    // Make the class more flexible with BsonIgnoreExtraElements to ignore unknown fields
    [BsonIgnoreExtraElements]
    public class LogEntry
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Timestamp")]
        public DateTime Timestamp { get; set; }

        // Change to string to handle different timestamp formats and provide conversion property
        [BsonElement("UtcTimestamp")]
        public string UtcTimestampString { get; set; }

        [BsonIgnore]
        public DateTime? UtcTimestamp 
        { 
            get 
            {
                if (string.IsNullOrEmpty(UtcTimestampString))
                    return null;

                // Try to parse in multiple formats
                if (DateTime.TryParse(UtcTimestampString, CultureInfo.InvariantCulture, 
                    DateTimeStyles.AdjustToUniversal, out DateTime result))
                {
                    return result;
                }

                // Try custom format "yyyy-MM-dd HH:mm:ssZ"
                string format = "yyyy-MM-dd HH:mm:ssZ";
                if (DateTime.TryParseExact(UtcTimestampString, format, CultureInfo.InvariantCulture, 
                    DateTimeStyles.AdjustToUniversal, out result))
                {
                    return result;
                }

                return null;
            }
        }

        [BsonElement("Level")]
        public string Level { get; set; }

        [BsonElement("MessageTemplate")]
        public string MessageTemplate { get; set; }

        [BsonElement("RenderedMessage")]
        public string RenderedMessage { get; set; }

        [BsonElement("Exception")]
        public string Exception { get; set; }

        [BsonElement("Properties")]
        public LogProperties Properties { get; set; }
        
        // Helper property to extract action description from log
        [BsonIgnore]
        public string ActionDescription
        {
            get 
            {
                if (Properties != null)
                {
                    if (!string.IsNullOrEmpty(Properties.Description))
                        return Properties.Description;
                        
                    // Extract description from rendered message if it contains the format "- {Description}"
                    if (!string.IsNullOrEmpty(RenderedMessage) && RenderedMessage.Contains(" - "))
                    {
                        var parts = RenderedMessage.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length > 1)
                            return parts[1].Split(new[] { " by user " }, StringSplitOptions.RemoveEmptyEntries)[0];
                    }
                }
                
                return Properties?.Method ?? "Unknown action";
            }
        }
    }

    // Make LogProperties class more flexible to handle varying document structures
    [BsonIgnoreExtraElements]
    public class LogProperties
    {
        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonElement("UserRole")]
        public string UserRole { get; set; }

        [BsonElement("UserEmail")]
        public string UserEmail { get; set; }

        [BsonElement("Method")]
        public string Method { get; set; }

        [BsonElement("RequestMethod")]
        public string RequestMethod { get; set; }

        [BsonElement("RequestPath")]
        public string RequestPath { get; set; }

        [BsonElement("Path")]
        public string Path { get; set; }

        [BsonElement("RequestId")]
        public string RequestId { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }
        
        [BsonElement("SourceContext")]
        public string SourceContext { get; set; }
        
        [BsonElement("ActionId")]
        public string ActionId { get; set; }
        
        [BsonElement("ActionName")]
        public string ActionName { get; set; }
        
        [BsonElement("RequestBody")]
        public string RequestBody { get; set; }

        [BsonElement("StatusCode")]
        public int? StatusCode { get; set; }

        [BsonElement("Elapsed")]
        public double? Elapsed { get; set; }

        [BsonElement("RequestHost")]
        public string RequestHost { get; set; }

        [BsonElement("MachineName")]
        public string MachineName { get; set; }

        [BsonElement("ThreadId")]
        public int? ThreadId { get; set; }

        // AdditionalElements is no longer needed with BsonIgnoreExtraElements attribute
    }
} 