using System;

namespace Tracker.Models {
    public class ParticipantInfoModel {
        public int ID { get; set; }
        public int? ProjectID { get; set; }
        public string ParticipantStatus { get; set; }
        public TimeSpan TimeNotificationStart { get; set; }       
        public TimeSpan TimeNotificationEnd { get; set; }
        public int NotificationCountPerDay { get; set; }
        public int NotificationMinValueVariation { get; set; }
        public string Token { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}