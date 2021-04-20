namespace Tracker.Models {
    public class SliderQuestionAddModel {
        public int ID { get; set; }
        public int? ProjectID { get; set; }
        public int QuestionTypeID { get; set; }
        public string QuestionText { get; set; }
        public string QuestionSubtext { get; set; }
        public string InstructionText { get; set; }
        public int QuestionNumber { get; set; }
        public int SliderMinValue { get; set; }
        public int SliderMaxValue { get; set; }
        public bool IsDiscrete { get; set; }
        public string LeftText { get; set; }
        public string RightText { get; set; }
    }
}