namespace Tracker.Entities {
    public class AffectGridQuestion {
        public int ID { get; set; }
        public int? QuestionID { get; set; }
        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MaxY { get; set; }
        public int? DelimiterCount { get; set; }
        public bool IsGridVisible { get; set; }
        public string UpperXText { get; set; }
        public string LowerXText { get; set; }
        public string UpperYText { get; set; }
        public string LowerYText { get; set; }
        public string LeftUpperSquareText { get; set; }
        public string RightUpperSquareText { get; set; }
        public string LeftLowerSquareText { get; set; }
        public string RightLowerSquareText { get; set; }
    }
}