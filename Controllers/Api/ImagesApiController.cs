using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    [ApiController]
    [Route("api/images")]
    public class ImagesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ImagesApiController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/images/{entityType}/{entityId}
        [HttpGet("{entityType}/{entityId}")]
        public async Task<IActionResult> GetImages(string entityType, int entityId)
        {
            IQueryable<Image> query = _context.Images.AsNoTracking();

            if (entityType.ToLower() == "building")
            {
                query = query.Where(i => i.BuildingId == entityId);
            }
            else if (entityType.ToLower() == "room")
            {
                query = query.Where(i => i.RoomId == entityId);
            }
            else
            {
                return BadRequest("Invalid entity type. Use 'building' or 'room'.");
            }

            var images = await query.OrderByDescending(i => i.IsThumbnail).ThenBy(i => i.UploadedAt).ToListAsync();
            return Ok(images);
        }

        // POST: api/images/upload/{entityType}/{entityId}
        [HttpPost("upload/{entityType}/{entityId}")]
        public async Task<IActionResult> UploadImages(string entityType, int entityId)
        {
            if (!Request.HasFormContentType) return BadRequest("Invalid content type.");
            var files = Request.Form.Files;
            if (files == null || files.Count == 0) return BadRequest("No files uploaded.");

            string? folderName = entityType.ToLower() == "building" ? "buildings" :
                              (entityType.ToLower() == "room" ? "rooms" : null);

            if (folderName == null) return BadRequest("Invalid entity type.");

            // Use WebRootPath if exists, else fallback to current directory + wwwroot
            string webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // Ensure the main folder exists
            string uploadsFolder = Path.Combine(webRoot, "uploads", folderName);
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            // Ensure entity subfolder exists
            string entityFolder = Path.Combine(uploadsFolder, entityId.ToString());
            if (!Directory.Exists(entityFolder)) Directory.CreateDirectory(entityFolder);

            var uploadedImages = new List<Image>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string fileExtension = Path.GetExtension(file.FileName);
                    string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                    string filePath = Path.Combine(entityFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Determine relative path to save in DB
                    string relativePath = $"/uploads/{folderName}/{entityId}/{uniqueFileName}";

                    var img = new Image
                    {
                        BuildingId = entityType.ToLower() == "building" ? entityId : (int?)null,
                        RoomId = entityType.ToLower() == "room" ? entityId : (int?)null,
                        ImageUrl = relativePath,
                        UploadedAt = DateTime.UtcNow,
                        // If it's the first image in the whole entity, make it a thumbnail
                        IsThumbnail = false
                    };

                    _context.Images.Add(img);
                    await _context.SaveChangesAsync(); // save immediately to get ID if needed, or save all at once

                    uploadedImages.Add(img);
                }
            }

            // Check if there is a thumbnail for this entity. If not, set the first uploaded one.
            if (uploadedImages.Count > 0)
            {
                bool hasThumbnail = false;
                if (entityType.ToLower() == "building")
                {
                    hasThumbnail = await _context.Images.AnyAsync(i => i.BuildingId == entityId && i.IsThumbnail);
                }
                else
                {
                    hasThumbnail = await _context.Images.AnyAsync(i => i.RoomId == entityId && i.IsThumbnail);
                }

                if (!hasThumbnail)
                {
                    uploadedImages[0].IsThumbnail = true;
                    _context.Entry(uploadedImages[0]).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(uploadedImages);
        }

        // DELETE: api/images/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null) return NotFound();

            // Check file exists physically
            string webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            try
            {
                string filePath = Path.Combine(webRoot, image.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch { /* Ignore file delete errors if any */ }

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();

            // If it was a thumbnail, we might want to promote another image to be thumbnail 
            // but we can let that be for now or handle it automatically
            if (image.IsThumbnail)
            {
                IQueryable<Image> query = _context.Images;
                if (image.BuildingId.HasValue)
                    query = query.Where(i => i.BuildingId == image.BuildingId);
                if (image.RoomId.HasValue)
                    query = query.Where(i => i.RoomId == image.RoomId);

                var newThumb = await query.FirstOrDefaultAsync();
                if (newThumb != null)
                {
                    newThumb.IsThumbnail = true;
                    _context.Entry(newThumb).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }

            return NoContent();
        }

        // POST: api/images/{id}/set-thumbnail
        [HttpPost("{id}/set-thumbnail")]
        public async Task<IActionResult> SetThumbnail(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null) return NotFound();

            // Reset all others
            IQueryable<Image> query = _context.Images;
            if (image.BuildingId.HasValue)
            {
                var others = await query.Where(i => i.BuildingId == image.BuildingId).ToListAsync();
                foreach (var o in others) { o.IsThumbnail = false; _context.Entry(o).State = EntityState.Modified; }
            }
            if (image.RoomId.HasValue)
            {
                var others = await query.Where(i => i.RoomId == image.RoomId).ToListAsync();
                foreach (var o in others) { o.IsThumbnail = false; _context.Entry(o).State = EntityState.Modified; }
            }

            image.IsThumbnail = true;
            _context.Entry(image).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Set as thumbnail successfully." });
        }
    }
}
