using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Gia hạn hợp đồng.
    /// </summary>
    [ApiController]
    [Route("api/contract-extensions")]
    public class ContractExtensionsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContractExtensionsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sách gia hạn (lọc theo contractId).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ContractExtension>>> GetAll([FromQuery] int? contractId)
        {
            var query = _context.ContractExtensions.AsNoTracking();
            if (contractId.HasValue)
            {
                query = query.Where(x => x.ContractId == contractId.Value);
            }
            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        /// <summary>
        /// Tạo gia hạn mới.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ContractExtension>> Create(ContractExtension extension)
        {
            _context.ContractExtensions.Add(extension);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = extension.Id }, extension);
        }

        /// <summary>
        /// Xóa gia hạn.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var extension = await _context.ContractExtensions.FindAsync(id);
            if (extension == null) return NotFound();

            _context.ContractExtensions.Remove(extension);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
