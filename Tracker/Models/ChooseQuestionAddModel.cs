using System.Collections.Generic;

namespace Tracker.Models {
    public class ChooseQuestionAddModel {
        public int ID { get; set; }
        public int? ProjectID { get; set; }
        public int QuestionTypeID { get; set; }
        public string QuestionText { get; set; }
        public string QuestionSubtext { get; set; }
        public string InstructionText { get; set; }
        public int QuestionNumber { get; set; }
        public bool IsSingleChoice { get; set; }
        public List<string> AnswerVariants { get; set; }
    }
}