using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class BillingController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BillingController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Billing>>> GetBillings()
    {
        return await _context.Billings.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Billing>> PostBilling(Billing billing)
    {
        _context.Billings.Add(billing);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBillings), new { id = billing.BillingID }, billing);
    }
}
