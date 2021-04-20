using System;

namespace Tracker.Entities {
    public class ParticipantAnswer {
        public int ID { get; set; }
        public bool IsAnswered { get; set; }
        public int? ParticipantID { get; set; }
        public int? QuestionID { get; set; }
        public string AnswerText { get; set; }
        public DateTime AnswerDate { get; set; }
    }
}