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
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackContext _context;

        public FeedbackController(FeedbackContext context)
        {
            _context = context;
        }

        // GET api/feedback (Get all feedback for admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeedbackResponse>>> GetAllFeedback()
        {
            return await _context.FeedbackResponses
                .Include(f => f.User)
                .Select(f => new FeedbackResponse
                {
                    Id = f.Id,
                    FormId = f.FormId,
                    Username = f.Username,
                    FeedbackType = f.FeedbackType,
                    Emoji = f.Emoji,
                    Text = f.Text,
                    Rating = f.Rating,
                    SubmittedAt = f.SubmittedAt
                })
                .ToListAsync();
        }

        // GET api/feedback/user (Get user's own feedback)
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<FeedbackResponse>>> GetUserFeedback()
        {
            return await _context.FeedbackResponses
                .Where(f => f.Username == User.Identity.Name)
                .Select(f => new FeedbackResponse
                {
                    Id = f.Id,
                    FormId = f.FormId,
                    FeedbackType = f.FeedbackType,
                    Emoji = f.Emoji,
                    Text = f.Text,
                    Rating = f.Rating,
                    SubmittedAt = f.SubmittedAt
                })
                .ToListAsync();
        }

        // POST api/feedback (Submit feedback)
        [HttpPost]
        public async Task<ActionResult<FeedbackResponse>> SubmitFeedback([FromBody] FeedbackSubmitRequest request)
        {
            var form = await _context.FeedbackForms.FindAsync(request.FormId);
            if (form == null || !form.IsActive)
                return BadRequest(new { message = "Invalid form" });

            var feedback = new FeedbackResponse
            {
                FormId = request.FormId,
                Username = User.Identity.Name,
                FeedbackType = request.FeedbackType,
                Emoji = request.FeedbackType == "emoji" ? request.Emoji : null,
                Text = request.FeedbackType == "text" ? request.Text : null,
                Rating = request.FeedbackType == "rating" ? request.Rating : null,
                SubmittedAt = DateTime.UtcNow
            };

            _context.FeedbackResponses.Add(feedback);
            await _context.SaveChangesAsync();

            return Ok(new FeedbackResponse
            {
                Id = feedback.Id,
                FormId = feedback.FormId,
                FeedbackType = feedback.FeedbackType,
                Emoji = feedback.Emoji,
                Text = feedback.Text,
                Rating = feedback.Rating,
                SubmittedAt = feedback.SubmittedAt
            });
        }
    }

    // Request/Response models
    public class FeedbackSubmitRequest
    {
        [Required] public int FormId { get; set; }
        [Required] public string FeedbackType { get; set; } // "emoji", "text", or "rating"
        public string Emoji { get; set; }
        public string Text { get; set; }
        [Range(1, 5)] public int? Rating { get; set; }
    }

    //public class FeedbackResponse
    //{
    //    public int Id { get; set; }
    //    public int FormId { get; set; }
    //    public string Username { get; set; }
    //    public string FeedbackType { get; set; }
    //    public string Emoji { get; set; }
    //    public string Text { get; set; }
    //    public int? Rating { get; set; }
    //    public DateTime SubmittedAt { get; set; }
    //}
}
