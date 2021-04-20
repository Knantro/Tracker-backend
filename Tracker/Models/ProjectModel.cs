using System;
using System.Collections.Generic;

namespace Tracker.Models {
    public class ProjectModel {
        public int ID { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string InstructionText { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int NotificationCountPerDay { get; set; }
        public int NotificationTimeout { get; set; }
        public bool IsNotificationsEnabled { get; set; }
        public List<object> Questions { get; set; }
        public List<ParticipantModel> ParticipantInfoList { get; set; } 
    }
}