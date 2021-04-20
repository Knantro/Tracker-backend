namespace Tracker.Entities {
    public class DiscreteSliderQuestion {
        public int ID { get; set; }
        public int? QuestionID { get; set; }
        public int DiscreteSliderMinValue { get; set; }
        public int DiscreteSliderMaxValue { get; set; }
        public string ScaleText { get; set; }
    }
}