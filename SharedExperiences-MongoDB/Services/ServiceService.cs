using ExperienceService.Data;
using ExperienceService.Models;
using Microsoft.EntityFrameworkCore;

namespace ExperienceService.Services
{
    public class ServiceService
    {
        private readonly SharedExperiencesDbContext _context;

        public ServiceService(SharedExperiencesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            return await _context.Services
                .Include(s => s.Provider)
                .Include(s => s.Discount)
                .ToListAsync();
        }

        public async Task<Service> GetServiceByIdAsync(int id)
        {
            return await _context.Services
                .Include(s => s.Provider)
                .Include(s => s.Discount)
                .FirstOrDefaultAsync(s => s.ServiceId == id);
        }

        public async Task<Service> CreateServiceAsync(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return service;
        }

        public async Task<Service> UpdateServiceAsync(int id, Service service)
        {
            var existingService = await _context.Services.FindAsync(id);
            if (existingService == null)
                return null;

            existingService.Name = service.Name;
            existingService.Description = service.Description;
            existingService.Price = service.Price;
            existingService.ProviderId = service.ProviderId;

            await _context.SaveChangesAsync();
            return existingService;
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return false;

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}