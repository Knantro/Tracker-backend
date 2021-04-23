using System;
using System.Collections.Generic;

namespace Tracker.Models {
    public class ParticipantModel {
        public int ID { get; set; }
        public int ProjectID { get; set; }
        public string ParticipantStatus { get; set; }
        public string TimeNotificationStart { get; set; }       
        public string TimeNotificationEnd { get; set; }
        public int NotificationCountPerDay { get; set; }
        public int NotificationMinValueVariation { get; set; }
        public List<ParticipantAnswerModel> ParticipantAnswerModelList { get; set; }
    }
}