[Route("api/[controller]")]
[ApiController]
public class GuestsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public GuestsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Guest>>> GetGuests()
    {
        return await _context.Guests.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Guest>> PostGuest(Guest guest)
    {
        _context.Guests.Add(guest);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetGuests), new { id = guest.GuestID }, guest);
    }
}
