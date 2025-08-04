namespace feedbackbackWidget_API.Models
{
    public class FeedbackSubmissionViewModel
    {

        public int FormId { get; set; }
        public string FeedbackType { get; set; }
        public string Emoji { get; set; }
        public string Text { get; set; }
        public int? Rating { get; set; }
    }
}
