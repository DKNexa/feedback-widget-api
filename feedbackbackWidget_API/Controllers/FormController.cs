using System.ComponentModel.DataAnnotations;
using feedbackbackWidget_API.Data;
using feedbackbackWidget_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace feedbackbackWidget_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FormController : ControllerBase
    {
        private readonly FeedbackContext _context;

        public FormController(FeedbackContext context)
        {
            _context = context;
        }

        // GET api/forms (Get all forms - for admin dashboard)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FormResponse>>> GetForms()
        {
            return await _context.FeedbackForms
                .Where(f => f.IsActive)
                .Select(f => new FormResponse
                {
                    Id = f.Id,
                    Title = f.Title,
                    Description = f.Description,
                    CreatedAt = f.CreatedAt
                })
                .ToListAsync();
        }

        // GET api/forms/{id} (Get form details)
        [HttpGet("{id}")]
        public async Task<ActionResult<FormResponse>> GetForm(int id)
        {
            var form = await _context.FeedbackForms.FindAsync(id);
            if (form == null || !form.IsActive)
                return NotFound();

            return new FormResponse
            {
                Id = form.Id,
                Title = form.Title,
                Description = form.Description,
                CreatedAt = form.CreatedAt
            };
        }

        // POST api/forms (Create new form - Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<FormResponse>> CreateForm([FromBody] FormCreateRequest request)
        {
            var form = new FeedbackForm
            {
                Title = request.Title,
                Description = request.Description,
                CreatedBy = User.Identity.Name
            };

            _context.FeedbackForms.Add(form);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetForm), new { id = form.Id }, new FormResponse
            {
                Id = form.Id,
                Title = form.Title,
                Description = form.Description,
                CreatedAt = form.CreatedAt
            });
        }

        // DELETE api/forms/{id} (Deactivate form - Admin only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForm(int id)
        {
            var form = await _context.FeedbackForms.FindAsync(id);
            if (form == null)
                return NotFound();

            form.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // Request/Response models
    public class FormCreateRequest
    {
        [Required] public string Title { get; set; }
        public string Description { get; set; }
    }

    public class FormResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
