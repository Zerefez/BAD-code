using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using SharedExperiences.DTO;
using SharedExperiences.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Text.Json;

namespace SharedExperiences.Services
{
    public class LogService
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoCollection<LogEntry> _logs;
        private readonly string _databaseName;
        private readonly string _collectionName;

        public LogService(IConfiguration configuration)
        {
            try
            {
                var mongoUrl = configuration["Serilog:WriteTo:2:Args:databaseUrl"] ?? "mongodb://localhost:27017/SharedExperiencesLogs";
                _databaseName = mongoUrl.Split('/').Last();
                _collectionName = configuration["Serilog:WriteTo:2:Args:collectionName"] ?? "ApiLogs";
                
                _mongoClient = new MongoClient(mongoUrl);
                var database = _mongoClient.GetDatabase(_databaseName);
                _logs = database.GetCollection<LogEntry>(_collectionName);
                
                Console.WriteLine($"Connected to MongoDB: {_databaseName}.{_collectionName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to MongoDB: {ex.Message}");
                throw;
            }
        }

        // Add diagnostic method to check for any logs
        public async Task<DiagnosticInfo> GetDiagnosticInfoAsync()
        {
            var info = new DiagnosticInfo
            {
                DatabaseName = _databaseName,
                CollectionName = _collectionName
            };
            
            try
            {
                // First check if we can connect to MongoDB
                await _mongoClient.GetDatabase("admin").RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}");
                
                // Count all logs
                info.TotalLogCount = await _logs.CountDocumentsAsync(Builders<LogEntry>.Filter.Empty);
                
                // Try to get sample document to check structure
                if (info.TotalLogCount > 0)
                {
                    try
                    {
                        // Get a sample log document as raw BsonDocument first
                        var rawDoc = await _mongoClient.GetDatabase(_databaseName)
                            .GetCollection<BsonDocument>(_collectionName)
                            .Find(new BsonDocument())
                            .Sort(Builders<BsonDocument>.Sort.Descending("Timestamp"))
                            .Limit(1)
                            .FirstOrDefaultAsync();
                        
                        if (rawDoc != null)
                        {
                            info.RawDocumentStructure = rawDoc.ToString();
                            
                            // Try to deserialize to LogEntry
                            var sampleLog = BsonSerializer.Deserialize<LogEntry>(rawDoc);
                            
                            if (sampleLog != null)
                            {
                                info.HasSampleLog = true;
                                info.SampleLogId = sampleLog.Id;
                                info.SampleLogTime = sampleLog.Timestamp;
                                info.SampleLogMessage = sampleLog.RenderedMessage;
                                
                                // Check if user ID exists
                                info.HasUserIdField = sampleLog.Properties?.UserId != null;
                                info.SampleUserId = sampleLog.Properties?.UserId;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        info.ErrorMessage = $"Could retrieve document count but failed to deserialize: {ex.Message}";
                        Console.WriteLine(info.ErrorMessage);
                    }
                }
                
                info.Success = true;
            }
            catch (Exception ex)
            {
                info.Success = false;
                info.ErrorMessage = ex.Message;
                Console.WriteLine($"Error in GetDiagnosticInfoAsync: {ex.Message}");
            }
            
            return info;
        }

        public async Task<(IEnumerable<LogEntry> Logs, long TotalCount)> SearchLogsAsync(LogSearchQuery query)
        {
            try
            {
                var filterBuilder = Builders<LogEntry>.Filter;
                var filters = new List<FilterDefinition<LogEntry>>();

                // Filter by user ID - now with more flexible matching options
                if (!string.IsNullOrEmpty(query.UserId))
                {
                    // Try multiple possible fields where user info might be stored
                    var userFilters = new List<FilterDefinition<LogEntry>>
                    {
                        // Direct matches for exact user ID
                        filterBuilder.Eq("UserId", query.UserId),
                        filterBuilder.Eq("Properties.UserId", query.UserId),
                        
                        // Match by username using regex for partial matching
                        filterBuilder.Regex("Properties.UserId", 
                            new MongoDB.Bson.BsonRegularExpression(query.UserId, "i")),
                        
                        // Also check other user-related fields
                        filterBuilder.Eq("Properties.UserRole", query.UserId), // Could search by role name
                        filterBuilder.Regex("RenderedMessage", 
                            new MongoDB.Bson.BsonRegularExpression(query.UserId, "i")) // Message might contain username
                    };
                    
                    // Combine with OR since user ID could be in any of these fields
                    filters.Add(filterBuilder.Or(userFilters));
                }

                // Filter by date range using regular Timestamp field
                if (query.StartDate.HasValue)
                {
                    filters.Add(filterBuilder.Gte(log => log.Timestamp, query.StartDate.Value));
                }

                if (query.EndDate.HasValue)
                {
                    var endDate = query.EndDate.Value.AddDays(1).AddSeconds(-1);
                    filters.Add(filterBuilder.Lte(log => log.Timestamp, endDate));
                }

                // Filter by HTTP method - handle both Method and RequestMethod fields
                if (!string.IsNullOrEmpty(query.Method))
                {
                    var methodFilters = new List<FilterDefinition<LogEntry>>
                    {
                        filterBuilder.Eq("Properties.Method", query.Method.ToUpper()),
                        filterBuilder.Eq("Properties.RequestMethod", query.Method.ToUpper()),
                        // Also try with lowercase for flexibility
                        filterBuilder.Eq("Properties.Method", query.Method.ToLower()),
                        filterBuilder.Eq("Properties.RequestMethod", query.Method.ToLower())
                    };
                    filters.Add(filterBuilder.Or(methodFilters));
                }
                
                // Filter by description
                if (!string.IsNullOrEmpty(query.Description))
                {
                    // First try to match Properties.Description
                    var descriptionFilter = filterBuilder.Regex("Properties.Description", 
                        new MongoDB.Bson.BsonRegularExpression(query.Description, "i"));
                    
                    // Also try to match in RenderedMessage which might contain the description after a dash
                    var messageFilter = filterBuilder.Regex("RenderedMessage", 
                        new MongoDB.Bson.BsonRegularExpression(query.Description, "i"));
                    
                    // Combine with OR since description could be in either field
                    filters.Add(filterBuilder.Or(descriptionFilter, messageFilter));
                }

                // Combine filters
                var combinedFilter = filters.Count > 0 
                    ? filterBuilder.And(filters) 
                    : filterBuilder.Empty;

                // Count total matching documents
                var totalCount = await _logs.CountDocumentsAsync(combinedFilter);

                // Calculate pagination
                var skip = (query.Page - 1) * query.PageSize;
                var limit = query.PageSize;

                // Get paginated results
                var logs = await _logs.Find(combinedFilter)
                    .Sort(Builders<LogEntry>.Sort.Descending(log => log.Timestamp))
                    .Skip(skip)
                    .Limit(limit)
                    .ToListAsync();

                return (logs, totalCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchLogsAsync: {ex.Message}");
                return (new List<LogEntry>(), 0);
            }
        }
        
        public async Task<List<OperationTypeCount>> GetOperationTypesAsync()
        {
            try
            {
                // Count total logs first
                var totalCount = await _logs.CountDocumentsAsync(Builders<LogEntry>.Filter.Empty);
                Console.WriteLine($"Total log count: {totalCount}");
                
                if (totalCount == 0)
                {
                    Console.WriteLine("No logs found in the database");
                    return new List<OperationTypeCount>();
                }
                
                // Include logs from POST, PUT, DELETE operations - check both Method and RequestMethod fields
                var methodFilter = Builders<LogEntry>.Filter.In("Properties.Method", 
                    new[] { "POST", "PUT", "DELETE" });
                var requestMethodFilter = Builders<LogEntry>.Filter.In("Properties.RequestMethod", 
                    new[] { "POST", "PUT", "DELETE" });
                var filter = Builders<LogEntry>.Filter.Or(methodFilter, requestMethodFilter);
                    
                // Get all logs matching the filter
                var logs = await _logs.Find(filter)
                    .Sort(Builders<LogEntry>.Sort.Descending(log => log.Timestamp))
                    .Limit(1000) // Limit to a reasonable number for analysis
                    .ToListAsync();
                
                Console.WriteLine($"Found {logs.Count} logs with POST/PUT/DELETE methods");
                
                // Extract operation descriptions and count occurrences
                return logs
                    .Select(log => log.ActionDescription ?? "Unknown")
                    .GroupBy(desc => desc)
                    .Select(group => new OperationTypeCount { 
                        Description = group.Key, 
                        Count = group.Count() 
                    })
                    .OrderByDescending(item => item.Count)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetOperationTypesAsync: {ex.Message}");
                return new List<OperationTypeCount>();
            }
        }
    }
    
    public class OperationTypeCount
    {
        public string Description { get; set; } = null!;
        public int Count { get; set; }
    }
    
    public class DiagnosticInfo
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public long TotalLogCount { get; set; }
        public bool HasSampleLog { get; set; }
        public string SampleLogId { get; set; }
        public DateTime SampleLogTime { get; set; }
        public string SampleLogMessage { get; set; }
        public bool HasUserIdField { get; set; }
        public string SampleUserId { get; set; }
        public string RawDocumentStructure { get; set; }
    }
} 