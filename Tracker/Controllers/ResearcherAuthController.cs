using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Tracker.Models;

namespace Tracker.Controllers {
    [ApiController]
    [Route(RouteTemplates.DefaultRoute)]
    public class ResearcherAuthController : ControllerBase { 
        [HttpPost]
        public ActionResult Login([FromBody]LoginModel loginModel) {
            using TrackerContext db = new();
            Researcher researcher = db.Researcher.FirstOrDefault(r =>
                (r.Email.Equals(loginModel.EmailOrPhoneNumber) || 
                 r.PhoneNumber.Equals(loginModel.EmailOrPhoneNumber)) && 
                r.Password.Equals(loginModel.Password)
            );

            if (researcher != null) {
                string token = string.Empty;
                do {
                    token = TokenGeneration.GenerateToken();
                } while (db.UserToken.FirstOrDefault(t => t.Token.Equals(token)) != null);
                UserToken ut = new () {
                    ResearcherID = researcher.ID,
                    Token = token
                };
                db.UserToken.Add(ut);
                db.SaveChanges();
                return Ok(ut.Token);
            }

            return BadRequest("Login attempt failed");
        }
        
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