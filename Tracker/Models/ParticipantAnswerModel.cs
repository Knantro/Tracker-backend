using System;
using System.Collections.Generic;

namespace Tracker.Models {
    public class ParticipantAnswerModel {
        public int ID { get; set; }
        public bool IsAnswered { get; set; }
        public int QuestionNumber { get; set; }
        public string Answer { get; set; }
        public DateTime AnswerDate { get; set; }
    }
}