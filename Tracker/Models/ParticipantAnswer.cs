using System;

namespace Tracker.Models {
    public class ParticipantAnswer {
        public int ID { get; set; }
        public int? QuestionID { get; set; }
        public int? ProjectID { get; set; }
        public string AnswerText { get; set; }
        public DateTime AnswerDate { get; set; }
    }
}