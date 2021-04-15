namespace Tracker.Models {
    public class SliderQuestion {
        public int ID { get; set; }
        public int? QuestionID { get; set; }
        public int SliderMinValue { get; set; }
        public int SliderMaxValue { get; set; }
        public bool IsDiscrete { get; set; }
        public string LeftText { get; set; }
        public string RightText { get; set; }
    }
}