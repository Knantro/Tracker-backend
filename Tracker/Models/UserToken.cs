namespace Tracker.Models {
    public class UserToken {
        public int ID { get; set; }
        public int? ParticipantID { get; set; }
        public int? ResearcherID { get; set; }
        public string Token { get; set; }
    }
}