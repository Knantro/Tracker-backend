namespace Tracker.Entities {
    public class ChooseQuestion {
        public int ID { get; set; }
        public int? QuestionID { get; set; }
        public bool IsSingleChoice { get; set; }
    }
}