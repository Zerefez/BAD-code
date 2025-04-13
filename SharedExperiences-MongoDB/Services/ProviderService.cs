using ExperienceService.Data;
using ExperienceService.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExperienceService.Services
{
    public class ProviderService
    {
        private readonly MongoDbContext _context;

        public ProviderService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Provider>> GetAllProvidersAsync()
        {
            return await _context.Providers.Find(_ => true).ToListAsync();
        }

        public async Task<Provider> GetProviderByIdAsync(string id)
        {
            return await _context.Providers.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Service>> GetProviderServicesAsync(string providerId)
        {
            return await _context.Services.Find(s => s.ProviderId == providerId).ToListAsync();
        }

        public async Task<Provider> CreateProviderAsync(Provider provider)
        {
            await _context.Providers.InsertOneAsync(provider);
            return provider;
        }

        public async Task<Provider> UpdateProviderAsync(string id, Provider provider)
        {
            var filter = Builders<Provider>.Filter.Eq(p => p.Id, id);
            var update = Builders<Provider>.Update
                .Set(p => p.Name, provider.Name)
                .Set(p => p.Address, provider.Address)
                .Set(p => p.Number, provider.Number)
                .Set(p => p.TouristicOperatorPermit, provider.TouristicOperatorPermit);

            await _context.Providers.UpdateOneAsync(filter, update);
            return await GetProviderByIdAsync(id);
        }

        public async Task<bool> DeleteProviderAsync(string id)
        {
            var result = await _context.Providers.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount > 0;
        }
    }
}