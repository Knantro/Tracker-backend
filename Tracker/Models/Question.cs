namespace Tracker.Models {
    public class Question {
        public int ID { get; set; }
        public int? ProjectID { get; set; }
        public int QuestionTypeID { get; set; }
        public string QuestionText { get; set; }
        public string QuestionSubtext { get; set; }
        public string InstructionText { get; set; }
        public int QuestionNumber { get; set; }
    }
}