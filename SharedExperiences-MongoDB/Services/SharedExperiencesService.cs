using ExperienceService.Data;
using ExperienceService.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExperienceService.Services;
public class SharedExperiencesService
{
    private readonly MongoDbContext _context;

    public SharedExperiencesService(MongoDbContext context)
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

    public async Task<Provider> CreateProviderAsync(Provider provider)
    {
        await _context.Providers.InsertOneAsync(provider);
        return provider;
    }

    public async Task<Provider> UpdateProviderAsync(string id, Provider provider)
    {
        var filter = Builders<Provider>.Filter.Eq(p => p.Id, id);
        var update = Builders<Provider>.Update
            .Set(p => p.Name, provider.Name);

        await _context.Providers.UpdateOneAsync(filter, update);
        return await GetProviderByIdAsync(id);
    }

    public async Task<bool> DeleteProviderAsync(string id)
    {
        var result = await _context.Providers.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }

    
    // Table 1 - Get the data collected for each experience provider.
    public async Task<IEnumerable<object>> Table1()
    {
        var providers = await _context.Providers.Find(_ => true).ToListAsync();
        return providers.Select(p => new
        {
            p.Name,
            p.Address,
            p.Number,
            p.TouristicOperatorPermit
        });
    }

    // Table 2 - List the experiences/services available in the system.
    public async Task<IEnumerable<object>> Table2()
    {
        var services = await _context.Services.Find(_ => true).ToListAsync();
        return services.Select(s => new
        {
            s.Name,
            s.Description,
            s.Price
        });
    }

    // Table 3 - Get the list of shared experiences and their date in the system in descending order
    public async Task<IEnumerable<object>> Table3()
    {
        var sharedExperiences = await _context.SharedExperiences.Find(_ => true).ToListAsync();
        return sharedExperiences
            .OrderByDescending(se => se.Date)
            .Select(se => new
            {
                se.Name,
                se.Date
            });
    }

    // Table 4 - Get guests registered for a shared experience
    public async Task<IEnumerable<object>> Table4(string sharedExperienceId)
    {
        var sharedExperience = await _context.SharedExperiences
            .Find(se => se.Id == sharedExperienceId)
            .FirstOrDefaultAsync();

        if (sharedExperience == null)
            return new List<object>();

        var guestIds = sharedExperience.GuestIds ?? new List<string>();
        var guests = await _context.Guests
            .Find(g => guestIds.Contains(g.Id))
            .ToListAsync();

        return guests.Select(g => new { g.Name });
    }

    // Table 5 - Get experiences included in a shared experience
    public async Task<IEnumerable<object>> Table5(string sharedExperienceId)
    {
        var sharedExperience = await _context.SharedExperiences
            .Find(se => se.Id == sharedExperienceId)
            .FirstOrDefaultAsync();

        if (sharedExperience == null)
            return new List<object>();

        var serviceIds = sharedExperience.ServiceIds ?? new List<string>();
        var services = await _context.Services
            .Find(s => serviceIds.Contains(s.Id))
            .ToListAsync();

        return services.Select(s => new { s.Name });
    }

    // Table 6 - Get the guests registered for one of the experiences/services in a shared experience.
    public async Task<IEnumerable<object>> Table6(string serviceId)
    {
        var sharedExperiences = await _context.SharedExperiences
            .Find(se => se.ServiceIds.Contains(serviceId))
            .ToListAsync();

        if (!sharedExperiences.Any())
            return new List<object>();

        var service = await _context.Services
            .Find(s => s.Id == serviceId)
            .FirstOrDefaultAsync();

        if (service == null)
            return new List<object>();

        var provider = await _context.Providers
            .Find(p => p.Id == service.ProviderId)
            .FirstOrDefaultAsync();

        var allGuestIds = sharedExperiences
            .SelectMany(se => se.GuestIds ?? new List<string>())
            .Distinct()
            .ToList();

        var guests = await _context.Guests
            .Find(g => allGuestIds.Contains(g.Id))
            .ToListAsync();

        return guests.Select(g => new 
        {
            GuestName = g.Name,
            ServiceName = service.Name
        });
    }

   // Table 7 - Get the minimum, average, and maximum price for the whole experience in the system.
    public async Task<object> Table7()
    {
        var services = await _context.Services.Find(_ => true).ToListAsync();
        
        if (!services.Any())
            return new { MinPrice = 0, AvgPrice = 0, MaxPrice = 0 };

        var minPrice = services.Min(s => s.Price);
        var avgPrice = services.Average(s => s.Price);
        var maxPrice = services.Max(s => s.Price);

        return new
        {
            MinPrice = minPrice,
            AvgPrice = avgPrice,
            MaxPrice = maxPrice
        };
    }

    // Table 8 - Get the number of guests and sum of sales for each experience available in the system.
    public async Task<IEnumerable<object>> Table8()
    {
        var services = await _context.Services.Find(_ => true).ToListAsync();
        var sharedExperiences = await _context.SharedExperiences.Find(_ => true).ToListAsync();
        
        return await Task.FromResult(services.Select(s => 
        {
            var relatedSharedExperiences = sharedExperiences
                .Where(se => se.ServiceIds.Contains(s.Id))
                .ToList();
            
            var distinctGuestIds = relatedSharedExperiences
                .SelectMany(se => se.GuestIds ?? new List<string>())
                .Distinct()
                .ToList();
            
            var guestCount = distinctGuestIds.Count;
            
            return new
            {
                s.Name,
                NumberOfGuests = guestCount,
                TotalSales = s.Price * guestCount
            };
        }));
    }

    // Table 9 - Get the total amount billed for each guest in the system.
    public async Task<IEnumerable<object>> Table9()
    {
        var guests = await _context.Guests.Find(_ => true).ToListAsync();
        var billings = await _context.Billings.Find(_ => true).ToListAsync();
        
        return guests.Select(g => new
        {
            g.Name,
            TotalAmountBilled = billings
                .Where(b => b.GuestId == g.Id)
                .Sum(b => b.Amount)
        });
    }
}