namespace feedbackbackWidget_API.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string FeedbackType { get; set; } // 'emoji', 'rating', 'text'
        public string Value { get; set; }
        public byte? Rating { get; set; }
        public string PageUrl { get; set; }
        public string UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserIP { get; set; }
    }

    public class FeedbackInputModel
    {
        public string FeedbackType { get; set; }
        public string Value { get; set; }
        public byte? Rating { get; set; }
        public string PageUrl { get; set; }
    }
}
