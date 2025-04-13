using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using SharedExperiences.DTO;
using SharedExperiences.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedExperiences.Services
{
    public class LogService
    {
        private readonly IMongoCollection<LogEntry> _logs;

        public LogService(IConfiguration configuration)
        {
            var mongoUrl = configuration["Serilog:WriteTo:2:Args:databaseUrl"] ?? "mongodb://localhost:27017/SharedExperiencesLogs";
            var databaseName = mongoUrl.Split('/').Last();
            var collectionName = configuration["Serilog:WriteTo:2:Args:collectionName"] ?? "ApiLogs";
            
            var client = new MongoClient(mongoUrl);
            var database = client.GetDatabase(databaseName);
            _logs = database.GetCollection<LogEntry>(collectionName);
        }

        public async Task<(IEnumerable<LogEntry> Logs, long TotalCount)> SearchLogsAsync(LogSearchQuery query)
        {
            var filterBuilder = Builders<LogEntry>.Filter;
            var filters = new List<FilterDefinition<LogEntry>>();

            // Filter by user ID
            if (!string.IsNullOrEmpty(query.UserId))
            {
                filters.Add(filterBuilder.Eq("Properties.UserId", query.UserId));
            }

            // Filter by date range
            if (query.StartDate.HasValue)
            {
                filters.Add(filterBuilder.Gte(log => log.Timestamp, query.StartDate.Value));
            }

            if (query.EndDate.HasValue)
            {
                filters.Add(filterBuilder.Lte(log => log.Timestamp, query.EndDate.Value.AddDays(1).AddSeconds(-1)));
            }

            // Filter by HTTP method
            if (!string.IsNullOrEmpty(query.Method))
            {
                filters.Add(filterBuilder.Eq("Properties.RequestMethod", query.Method.ToUpper()));
            }
            
            // Filter by description
            if (!string.IsNullOrEmpty(query.Description))
            {
                // First try to match Properties.Description
                var descriptionFilter = filterBuilder.Regex("Properties.Description", 
                    new MongoDB.Bson.BsonRegularExpression(query.Description, "i"));
                
                // Also try to match in RenderedMessage which might contain the description after a dash
                var messageFilter = filterBuilder.Regex("RenderedMessage", 
                    new MongoDB.Bson.BsonRegularExpression(" - .*" + query.Description, "i"));
                
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
        
        public async Task<List<OperationTypeCount>> GetOperationTypesAsync()
        {
            // Only include logs from POST, PUT, DELETE operations
            var filter = Builders<LogEntry>.Filter.In("Properties.RequestMethod", 
                new[] { "POST", "PUT", "DELETE" });
                
            // Get all logs matching the filter
            var logs = await _logs.Find(filter)
                .Sort(Builders<LogEntry>.Sort.Descending(log => log.Timestamp))
                .Limit(1000) // Limit to a reasonable number for analysis
                .ToListAsync();
                
            // Extract operation descriptions and count occurrences
            return logs
                .Select(log => log.ActionDescription)
                .GroupBy(desc => desc)
                .Select(group => new OperationTypeCount { 
                    Description = group.Key, 
                    Count = group.Count() 
                })
                .OrderByDescending(item => item.Count)
                .ToList();
        }
    }
    
    public class OperationTypeCount
    {
        public string Description { get; set; } = null!;
        public int Count { get; set; }
    }
} 