using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feedbackbackWidget_API.Models
{
    public class FeedbackResponse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("FeedbackForm")]
        public int FormId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(20)]
        public string FeedbackType { get; set; } // "emoji", "text", or "rating"

        [StringLength(10)]
        public string Emoji { get; set; } // Unicode emoji character

        [StringLength(500)]
        public string Text { get; set; }

        [Range(1, 5)]
        public int? Rating { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime SubmittedAt { get; set; }

        // Navigation properties
        public virtual FeedbackForm FeedbackForm { get; set; }
        public virtual User User { get; set; }
    }
}
