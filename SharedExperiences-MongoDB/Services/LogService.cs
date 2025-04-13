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
    }
} 