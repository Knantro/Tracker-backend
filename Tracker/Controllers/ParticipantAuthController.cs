using System.Collections.Generic;
using System.Linq;
using Tracker.Entities;
using Microsoft.AspNetCore.Mvc;
using Tracker.Models;

namespace Tracker.Controllers {
    [ApiController]
    [Route(RouteTemplates.DefaultRoute)]
    public class ParticipantAuthController : ControllerBase { 
        [HttpPost]
        public ActionResult Login(int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            using TrackerContext db = new();
            Participant participant = db.Participant.FirstOrDefault(p =>
                p.ID.Equals(id)
            );

            if (db.UserToken.FirstOrDefault(u => u.ParticipantID == id) != null) {
                return BadRequest("You're already authorized");
            }

            if (participant != null) {
                string token = string.Empty;
                do {
                    token = TokenManagement.GenerateToken();
                } while (db.UserToken.FirstOrDefault(t => t.Token.Equals(token)) != null);
                UserToken ut = new () {
                    ParticipantID = participant.ID,
                    Token = token
                };
                db.UserToken.Add(ut);
                db.SaveChanges();

                Researcher researcher = db.Researcher.First(r =>
                    r.ID == db.Project.First(p => p.ID == participant.ProjectID).ResearcherID);
                
                var participantInfo = new ParticipantInfoModel() {
                    Nickname = researcher.Nickname,
                    Email = researcher.Email,
                    PhoneNumber = researcher.PhoneNumber,
                    ID = participant.ID,
                    Token = token,
                    NotificationCountPerDay = participant.NotificationCountPerDay,
                    NotificationMinValueVariation = participant.NotificationMinValueVariation,
                    ParticipantStatus = db.ParticipantStatus.First(s => s.ID == participant.ParticipantStatusID).StatusText,
                    ProjectID = participant.ProjectID,
                    TimeNotificationEnd = participant.TimeNotificationEnd,
                    TimeNotificationStart = participant.TimeNotificationStart
                };
                return Ok(participantInfo);
            }

            return BadRequest("Login attempt failed");
        }

        [HttpPost]
        public ActionResult Logout(string token) {
            using TrackerContext db = new();
            UserToken ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            
            if (ut is null) {
                return Unauthorized("You're not authorized");
            }

            db.UserToken.Remove(ut);
            db.SaveChanges();

            return Ok();
        }
    }
}