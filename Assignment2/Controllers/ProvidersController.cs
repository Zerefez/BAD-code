[Route("api/[controller]")]
[ApiController]
public class ProvidersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProvidersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Provider>>> GetProviders()
    {
        return await _context.Providers.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Provider>> PostProvider(Provider provider)
    {
        _context.Providers.Add(provider);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProviders), new { id = provider.ProviderID }, provider);
    }
}
