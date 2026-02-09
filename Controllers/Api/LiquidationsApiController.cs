using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Thanh lý hợp đồng và quyết toán.
    /// </summary>
    [ApiController]
    [Route("api/liquidations")]
    public class LiquidationsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LiquidationsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sách thanh lý (lọc theo contractId).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Liquidation>>> GetAll([FromQuery] int? contractId)
        {
            var query = _context.Liquidations.AsNoTracking();
            if (contractId.HasValue)
            {
                query = query.Where(x => x.ContractId == contractId.Value);
            }
            return await query.ToListAsync();
        }

        /// <summary>
        /// Tạo thanh lý mới.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Liquidation>> Create(Liquidation liquidation)
        {
            _context.Liquidations.Add(liquidation);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = liquidation.Id }, liquidation);
        }

        /// <summary>
        /// Xóa thanh lý.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var liquidation = await _context.Liquidations.FindAsync(id);
            if (liquidation == null) return NotFound();

            _context.Liquidations.Remove(liquidation);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
