using ExperienceService.Data;
using ExperienceService.Models;
using Microsoft.EntityFrameworkCore;

namespace ExperienceService.Services;
public class SharedExperiencesService
{
    private readonly SharedExperiencesDbContext _context;

    public SharedExperiencesService(SharedExperiencesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Provider>> GetAllProvidersAsync()
    {
        return await _context.Providers
            .Include(p => p.Services)
            .ToListAsync();
    }

    public async Task<Provider> GetProviderByIdAsync(int id)
    {
        return await _context.Providers
            .Include(p => p.Services)
            .FirstOrDefaultAsync(p => p.ProviderId == id);
    }

    public async Task<Provider> CreateProviderAsync(Provider provider)
    {
        _context.Providers.Add(provider);
        await _context.SaveChangesAsync();
        return provider;
    }

    public async Task<Provider> UpdateProviderAsync(int id, Provider provider)
    {
        var existingProvider = await _context.Providers.FindAsync(id);
        if (existingProvider == null)
            return null;

        existingProvider.Name = provider.Name;
        existingProvider.Services = provider.Services;

        await _context.SaveChangesAsync();
        return existingProvider;
    }

    public async Task<bool> DeleteProviderAsync(int id)
    {
        var provider = await _context.Providers.FindAsync(id);
        if (provider == null)
            return false;

        _context.Providers.Remove(provider);
        await _context.SaveChangesAsync();
        return true;
    }

    
    // Table 1 - Get the data collected for each experience provider.
    public async Task<IEnumerable<object>> Table1()
    {
        return await _context.Providers
            .Select(p => new
            {
                p.Name,
                p.Address,
                p.Number,
                p.TouristicOperatorPermit
            })
            .ToListAsync();
    }

    // Table 2 - List the experiences/services available in the system.
    public async Task<IEnumerable<object>> Table2()
    {
        return await _context.Services
            .Select(s => new
            {
                s.Name,
                s.Description,
                s.Price
            })
            .ToListAsync();
    }

    // Table 3 - Get the list of shared experiences and their date in the system in descending order
    public async Task<IEnumerable<object>> Table3()
    {
        return await _context.SharedExperiences
            .OrderByDescending(se => se.Date)
            .Select(se => new
            {
                se.Name,
                se.Date
            })
            .ToListAsync();
    }

    // Table 4 - Get guests registered for a shared experience
    public async Task<IEnumerable<object>> Table4(int sharedExperienceId)
    {
        return await _context.SharedExperiences
            .Where(se => se.SharedExperienceId == sharedExperienceId)
            .SelectMany(se => se.Guests)
            .Select(g => new
            {
                g.Name
            })
            .ToListAsync();
    }

    // Table 5 - Get experiences included in a shared experience
    public async Task<IEnumerable<object>> Table5(int sharedExperienceId)
    {
        return await _context.Services
            .Where(s => s.SharedExperiences.Any(se => se.SharedExperienceId == sharedExperienceId))
            .Select(s => new
            {
                s.Name
            })
            .ToListAsync();
    }

    // Table 6 - Get the guests registered for one of the experiences/services in a shared experience.
    public async Task<IEnumerable<object>> Table6(int serviceId)
    {
        return await _context.Services
            .Where(s => s.ServiceId == serviceId)
            .Select(s => new {
                ProviderName = s.Provider.Name,
                ServiceName = s.Name,
                Guests = s.SharedExperiences
                    .SelectMany(se => se.Guests)
                    .Select(g => g.Name)
                    .Distinct()
            })
            .SelectMany(
                x => x.Guests,
                (x, guestName) => new {
                    GuestName = guestName,
                    x.ServiceName
                }
            )
            .ToListAsync();
    }

   // Table 7 -Get the minimum, average, and maximum price for the whole experience in the system.
    public async Task<object> Table7()
    {
        var minPrice = await _context.Services.MinAsync(s => s.Price);
        var avgPrice = await _context.Services.AverageAsync(s => s.Price);
        var maxPrice = await _context.Services.MaxAsync(s => s.Price);

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
        return await _context.Services
            .Select(s => new
            {
                s.Name,
                NumberOfGuests = s.SharedExperiences
                    .SelectMany(se => se.Guests)
                    .Distinct()
                    .Count(), // Ensure distinct guests are counted
                TotalSales = s.Price * s.SharedExperiences
                    .SelectMany(se => se.Guests)
                    .Distinct()
                    .Count()
            })
            .ToListAsync();
    }

    // Table 9 - Get the total amount billed for each guest in the system.
    public async Task<IEnumerable<object>> Table9()
    {
        return await _context.Guests
            .Select(g => new
            {
                g.Name,
                TotalAmountBilled = g.Billings.Sum(b => b.Amount)
            })
            .ToListAsync();
    }


}