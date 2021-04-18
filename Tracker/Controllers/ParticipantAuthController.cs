using System.Linq;
using Tracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace Tracker.Controllers {
    [ApiController]
    [Route(RouteTemplates.DefaultRoute)]
    public class ParticipantAuthController : ControllerBase { 
        [HttpPost]
        public ActionResult Login(int id) {
            using TrackerContext db = new();
            Participant participant = db.Participant.FirstOrDefault(p =>
                p.ID.Equals(id)
            );

            if (participant != null) {
                string token = string.Empty;
                do {
                    token = TokenGeneration.GenerateToken();
                } while (db.UserToken.FirstOrDefault(t => t.Token.Equals(token)) != null);
                UserToken ut = new () {
                    ParticipantID = participant.ID,
                };
                db.UserToken.Add(ut);
                db.SaveChanges();
                return Ok(ut.Token);
            }

            return BadRequest("Login attempt failed");
        }

        // [HttpGet]
        // public ActionResult GetQuestions(string token) {
        //     
        // }
        
        [HttpPost]
        public ActionResult Logout(string token) {
            using TrackerContext db = new();
            UserToken ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            
            if (ut is null) {
                return Unauthorized();
            }

            db.UserToken.Remove(ut);
            db.SaveChanges();

            return Ok();
        }
    }
}