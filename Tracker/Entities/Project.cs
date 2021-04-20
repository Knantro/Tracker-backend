using System;

namespace Tracker.Entities {
    public class Project {
        public int ID { get; set; }
        public int? ResearcherID { get; set; }
        public int? ProjectStatusID { get; set; }
        public string Title { get; set; }
        public string InstructionText { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int NotificationCountPerDay { get; set; }
        public int NotificationTimeout { get; set; }
        public bool IsNotificationsEnabled { get; set; }
    }
}