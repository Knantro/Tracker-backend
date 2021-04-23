using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

                Project project = db.Project.First(p => p.ID == participant.ProjectID);
                
                var participantInfo = new ParticipantInfoModel() {
                    Nickname = researcher.Nickname,
                    Email = researcher.Email,
                    PhoneNumber = researcher.PhoneNumber,
                    ID = participant.ID,
                    DateEnd = project.DateEnd,
                    DateStart = project.DateStart,
                    NotificationTimeout = project.NotificationTimeout,
                    IsNotificationsEnabled = project.IsNotificationsEnabled,
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
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }
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