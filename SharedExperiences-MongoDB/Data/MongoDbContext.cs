using MongoDB.Driver;
using ExperienceService.Models;
using Microsoft.Extensions.Configuration;

namespace ExperienceService.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            _database = client.GetDatabase("SharedExperiencesDB");
        }

        public IMongoCollection<Service> Services => _database.GetCollection<Service>("Services");
        public IMongoCollection<Provider> Providers => _database.GetCollection<Provider>("Providers");
        public IMongoCollection<SharedExperience> SharedExperiences => _database.GetCollection<SharedExperience>("SharedExperiences");
        public IMongoCollection<Guest> Guests => _database.GetCollection<Guest>("Guests");
        public IMongoCollection<Discount> Discounts => _database.GetCollection<Discount>("Discounts");
        public IMongoCollection<Billing> Billings => _database.GetCollection<Billing>("Billings");
    }
} 