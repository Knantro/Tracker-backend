using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Tracker.Models;
using Tracker.Entities;

namespace Tracker.Controllers {
    [ApiController]
    [Route(RouteTemplates.DefaultRoute)]
    public class ResearcherAuthController : ControllerBase { 
        [HttpPost]
        public ActionResult Login([FromBody]LoginModel loginModel) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            
            using TrackerContext db = new();
            Researcher researcher = db.Researcher.FirstOrDefault(r =>
                (r.Email.Equals(loginModel.EmailOrPhoneNumber) || 
                 r.PhoneNumber.Equals(loginModel.EmailOrPhoneNumber)) && 
                r.Password.Equals(loginModel.Password)
            );

            if (researcher != null) {
                string token = string.Empty;
                do {
                    token = TokenManagement.GenerateToken();
                } while (db.UserToken.FirstOrDefault(t => t.Token.Equals(token)) != null);
                UserToken ut = new () {
                    ResearcherID = researcher.ID,
                    Token = token
                };
                db.UserToken.Add(ut);
                db.SaveChanges();
                var researcherModel = new ResearcherModel() {
                    Nickname = researcher.Nickname,
                    Email = researcher.Email,
                    PhoneNumber = researcher.PhoneNumber,
                    ID = researcher.ID,
                    Token = token
                };
                return Ok(researcherModel);
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