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
}