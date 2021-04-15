using System;

namespace Tracker.Models {
    public class Participant {
        public int ID { get; set; }
        public int? ProjectID { get; set; }
        public int ParticipantStatusID { get; set; }
        public DateTime TimeNotificationStart { get; set; }       
        public DateTime TimeNotificationEnd { get; set; }
        public int NotificationCountPerDay { get; set; }
        public int NotificationMinValueVariation { get; set; }
    }
}