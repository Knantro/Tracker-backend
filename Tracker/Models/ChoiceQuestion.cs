namespace Tracker.Models {
    public class ChoiceQuestion {
        public int ID { get; set; }
        public int? QuestionID { get; set; }
        public bool IsSingleChoice { get; set; }
    }
}