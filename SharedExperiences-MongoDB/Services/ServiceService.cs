using ExperienceService.Data;
using ExperienceService.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExperienceService.Services
{
    public class ServiceService
    {
        private readonly MongoDbContext _context;

        public ServiceService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            return await _context.Services.Find(_ => true).ToListAsync();
        }

        public async Task<Service> GetServiceByIdAsync(string id)
        {
            return await _context.Services.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Service> CreateServiceAsync(Service service)
        {
            await _context.Services.InsertOneAsync(service);
            return service;
        }

        public async Task<Service> UpdateServiceAsync(string id, Service service)
        {
            var filter = Builders<Service>.Filter.Eq(s => s.Id, id);
            var update = Builders<Service>.Update
                .Set(s => s.Name, service.Name)
                .Set(s => s.Description, service.Description)
                .Set(s => s.Price, service.Price)
                .Set(s => s.ProviderId, service.ProviderId)
                .Set(s => s.Date, service.Date);

            await _context.Services.UpdateOneAsync(filter, update);
            return await GetServiceByIdAsync(id);
        }

        public async Task<bool> DeleteServiceAsync(string id)
        {
            var result = await _context.Services.DeleteOneAsync(s => s.Id == id);
            return result.DeletedCount > 0;
        }
    }
}