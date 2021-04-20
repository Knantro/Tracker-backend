using System;
using System.Collections.Generic;

namespace Tracker.Models {
    public class StatisticsModel {
        public List<string> RecordsStates { get; set; } // Done; Skipped; Soon
        public int DaysLeft { get; set; }
    }
}