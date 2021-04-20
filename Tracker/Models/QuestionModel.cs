using System;
using System.Collections.Generic;

namespace Tracker.Models {
    public class QuestionModel {
        public int ID { get; set; }
        public string QuestionType { get; set; }
        public string QuestionText { get; set; }
        public string QuestionSubtext { get; set; }
        public string InstructionText { get; set; }
        public int QuestionNumber { get; set; }
    }

    public class AffectGridQuestionModel : QuestionModel {
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
    
    public class ChooseQuestionModel : QuestionModel {
        public bool IsSingleChoice { get; set; }
        public List<string> Answers { get; set; }
    }
    
    public class DiscreteSliderQuestionModel : QuestionModel {
        public int DiscreteSliderMinValue { get; set; }
        public int DiscreteSliderMaxValue { get; set; }
        public List<string> ScaleTexts { get; set; }
    }
    
    public class SliderQuestionModel : QuestionModel {
        public int SliderMinValue { get; set; }
        public int SliderMaxValue { get; set; }
        public bool IsDiscrete { get; set; }
        public string LeftText { get; set; }
        public string RightText { get; set; }
    }
}