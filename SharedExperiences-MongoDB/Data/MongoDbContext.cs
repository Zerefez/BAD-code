using MongoDB.Driver;
using ExperienceService.Models;
using Microsoft.Extensions.Configuration;
using System;

namespace ExperienceService.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            try
            {
                var connectionString = configuration.GetConnectionString("MongoDB");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("MongoDB connection string is missing in configuration");
                }
                
                var client = new MongoClient(connectionString);
                _database = client.GetDatabase("SharedExperiencesDB");
                
                // Ensure collections exist
                CreateCollectionsIfNotExist();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"MongoDB connection error: {ex.Message}");
                throw;
            }
        }

        // Collections
        public IMongoCollection<Service> Services => _database.GetCollection<Service>("Services");
        public IMongoCollection<Provider> Providers => _database.GetCollection<Provider>("Providers");
        public IMongoCollection<SharedExperience> SharedExperiences => _database.GetCollection<SharedExperience>("SharedExperiences");
        public IMongoCollection<Guest> Guests => _database.GetCollection<Guest>("Guests");
        public IMongoCollection<Discount> Discounts => _database.GetCollection<Discount>("Discounts");
        public IMongoCollection<Billing> Billings => _database.GetCollection<Billing>("Billings");

        private void CreateCollectionsIfNotExist()
        {
            var collections = _database.ListCollectionNames().ToList();
            
            if (!collections.Contains("Services"))
                _database.CreateCollection("Services");
                
            if (!collections.Contains("Providers"))
                _database.CreateCollection("Providers");
                
            if (!collections.Contains("SharedExperiences"))
                _database.CreateCollection("SharedExperiences");
                
            if (!collections.Contains("Guests"))
                _database.CreateCollection("Guests");
                
            if (!collections.Contains("Discounts"))
                _database.CreateCollection("Discounts");
                
            if (!collections.Contains("Billings"))
                _database.CreateCollection("Billings");
        }
    }
} 