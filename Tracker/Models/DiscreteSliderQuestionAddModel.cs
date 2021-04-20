namespace Tracker.Models {
    public class DiscreteSliderQuestionAddModel {
        public int ID { get; set; }
        public int? ProjectID { get; set; }
        public int QuestionTypeID { get; set; }
        public string QuestionText { get; set; }
        public string QuestionSubtext { get; set; }
        public string InstructionText { get; set; }
        public int QuestionNumber { get; set; }
        public int DiscreteSliderMinValue { get; set; }
        public int DiscreteSliderMaxValue { get; set; }
        public string ScaleText { get; set; }
    }
}