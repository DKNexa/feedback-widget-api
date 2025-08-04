using System.Collections.Generic;
using System.Reflection.Emit;
using feedbackbackWidget_API.Models;
using Microsoft.EntityFrameworkCore;

namespace feedbackbackWidget_API.Data
{
    public class FeedbackContext : DbContext
    {
        public FeedbackContext(DbContextOptions<FeedbackContext> options) : base(options)
        {
        }

        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<FeedbackForm> FeedbackForms { get; set; }
        public DbSet<FeedbackResponse> FeedbackResponses { get; set; }
        public DbSet<User> Users { get; set; } // Assuming you have a User model for authentication
        //public DbSet<FeedbackSubmissionViewModel> FeedbackSubmissions { get; set; } // Assuming you have a view model for feedback submission
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Feedback>()
                .Property(f => f.FeedbackType)
                .HasConversion<string>();
        }
    }
}
