[Route("api/[controller]")]
[ApiController]
public class SharedExperienceController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SharedExperienceController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SharedExperience>>> GetSharedExperiences()
    {
        return await _context.SharedExperiences.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<SharedExperience>> PostSharedExperience(SharedExperience sharedExperience)
    {
        _context.SharedExperiences.Add(sharedExperience);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetSharedExperiences), new { id = sharedExperience.ExperiencesID }, sharedExperience);
    }
}
