using System;

namespace Tracker.Entities {
    public class Participant {
        public int ID { get; set; }
        public int? ProjectID { get; set; }
        public int ParticipantStatusID { get; set; }
        public TimeSpan TimeNotificationStart { get; set; }       
        public TimeSpan TimeNotificationEnd { get; set; }
        public int NotificationCountPerDay { get; set; }
        public int NotificationMinValueVariation { get; set; }
    }
}