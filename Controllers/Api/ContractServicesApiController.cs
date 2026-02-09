using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quản lý dịch vụ gắn với hợp đồng.
    /// </summary>
    [ApiController]
    [Route("api/contract-services")]
    public class ContractServicesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContractServicesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sách dịch vụ theo hợp đồng.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ContractService>>> GetAll([FromQuery] int? contractId)
        {
            var query = _context.ContractServices.AsNoTracking();
            if (contractId.HasValue)
            {
                query = query.Where(x => x.ContractId == contractId.Value);
            }
            return await query.ToListAsync();
        }

        /// <summary>
        /// Thêm dịch vụ vào hợp đồng.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ContractService>> Create(ContractService service)
        {
            _context.ContractServices.Add(service);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = service.Id }, service);
        }

        /// <summary>
        /// Cập nhật dịch vụ của hợp đồng.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, ContractService service)
        {
            if (id != service.Id) return BadRequest();

            _context.Entry(service).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Xóa dịch vụ khỏi hợp đồng.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _context.ContractServices.FindAsync(id);
            if (service == null) return NotFound();

            _context.ContractServices.Remove(service);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
