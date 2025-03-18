[Route("api/[controller]")]
[ApiController]
public class DiscountController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DiscountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Discount>>> GetDiscounts()
    {
        return await _context.Discounts.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Discount>> PostDiscount(Discount discount)
    {
        _context.Discounts.Add(discount);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetDiscounts), new { id = discount.DiscountID }, discount);
    }
}
