using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Cms;
using Tracker.Models;
using Tracker.Entities;

namespace Tracker.Controllers {
    [ApiController]
    [Route(RouteTemplates.DefaultRoute)]
    public class ResearcherController : ControllerBase {
        [HttpPost]
        public ActionResult AddProject([FromBody] Project project, string token) {
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }

            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorised");
            }

            if (project.DateEnd <= project.DateStart) {
                return BadRequest("End date should be greater than start date");
            }

            if ((project.DateEnd is null || project.DateStart is null) && project.ProjectStatusID > 2) {
                return BadRequest("Started or ended project should have dates of start and end of current project");
            }

            project.ResearcherID = ut.ResearcherID;

            try {
                db.Project.Add(project);
                db.SaveChanges();
            }
            catch (Exception ex) {
                return BadRequest(ex);
                //return BadRequest("Error while adding project");
            }

            return Ok("Project added successfully");
        }

        [HttpPost]
        public ActionResult AddParticipant([FromBody] ParticipantModel participantModel, string token) {
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }

            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorised");
            }

            if (db.Project.FirstOrDefault(p => p.ID == participantModel.ProjectID) is null) {
                return BadRequest("Project does not exist");
            }

            if (db.Participant.FirstOrDefault(p => p.ID == participantModel.ID) != null) {
                return BadRequest("Participant with current id already exists");
            }

            if (db.Project.First(p => p.ID == participantModel.ProjectID).ResearcherID != ut.ResearcherID) {
                return BadRequest("Access denied");
            }

            var timeNotificationEnd =
                TimeSpan.Parse(participantModel.TimeNotificationEnd, CultureInfo.InvariantCulture);

            var timeNotificationStart =
                TimeSpan.Parse(participantModel.TimeNotificationStart, CultureInfo.InvariantCulture);

            if (timeNotificationEnd <= timeNotificationStart) {
                return BadRequest(
                    "Time of end sending notifications should be greater than time of start sending notifications");
            }

            if ((timeNotificationEnd - timeNotificationStart).TotalMinutes <
                participantModel.NotificationCountPerDay * participantModel.NotificationMinValueVariation) {
                return BadRequest("Cannot send notifications properly by current parameters: " +
                                  $"No enough time for sending {participantModel.NotificationCountPerDay} notifications");
            }

            try {
                db.Participant.Add(new Participant() {
                    ID = participantModel.ID,
                    ProjectID = participantModel.ProjectID,
                    NotificationCountPerDay = participantModel.NotificationCountPerDay,
                    NotificationMinValueVariation = participantModel.NotificationMinValueVariation,
                    TimeNotificationEnd = timeNotificationEnd,
                    TimeNotificationStart = timeNotificationStart,
                    ParticipantStatusID = db.ParticipantStatus
                        .First(s => s.StatusText == participantModel.ParticipantStatus).ID
                });

                db.SaveChanges();
            }
            catch (Exception ex) {
                return BadRequest(ex);
                //return BadRequest("Cannot add new participant. Possible reasons:" +
                //   "You");
            }

            return Ok("Participant added successfully");
        }

        [HttpPost]
        public ActionResult AddAffectGridQuestion([FromBody] AffectGridQuestionAddModel model, string token) {
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }

            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorised");
            }

            if (db.Project.FirstOrDefault(p => p.ID == model.ProjectID) is null) {
                return BadRequest("Project does not exists");
            }

            if (db.Project.First(p => p.ID == model.ProjectID).ResearcherID != ut.ResearcherID) {
                return BadRequest("Access denied");
            }

            if (model.MinX >= model.MaxX || model.MinY >= model.MaxY) {
                BadRequest("Maximum values should be greater than minimum values");
            }

            if (db.Question.Where(q => q.ProjectID == model.ProjectID)
                .FirstOrDefault(q => q.QuestionNumber == model.QuestionNumber) != null) {
                return BadRequest("Question with current position number exists in project");
            }

            try {
                int id = db.Question.Max(q => q.ID) + 1;
                db.Question.Add(new Question() {
                    ID = id,
                    ProjectID = model.ProjectID,
                    InstructionText = model.InstructionText,
                    QuestionNumber = model.QuestionNumber,
                    QuestionSubtext = model.QuestionSubtext,
                    QuestionText = model.QuestionText,
                    QuestionTypeID = 1
                });

                db.SaveChanges();

                db.AffectGridQuestion.Add(new AffectGridQuestion() {
                    QuestionID = id,
                    DelimiterCount = model.DelimiterCount,
                    IsGridVisible = model.IsGridVisible,
                    LeftLowerSquareText = model.LeftLowerSquareText,
                    LeftUpperSquareText = model.LeftUpperSquareText,
                    RightLowerSquareText = model.RightLowerSquareText,
                    RightUpperSquareText = model.RightUpperSquareText,
                    LowerXText = model.LowerXText,
                    LowerYText = model.LowerYText,
                    UpperXText = model.UpperXText,
                    UpperYText = model.UpperYText,
                    MaxX = model.MaxX,
                    MaxY = model.MaxY,
                    MinX = model.MinX,
                    MinY = model.MinY
                });

                db.SaveChanges();
            }
            catch (Exception ex) {
                return BadRequest(ex);
                //return BadRequest("Cannot add new participant. Possible reasons:" +
                //   "You");
            }

            return Ok("Question added successfully");
        }

        [HttpPost]
        public ActionResult AddChooseQuestion([FromBody] ChooseQuestionAddModel model, string token) {
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }

            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorised");
            }

            if (db.Project.FirstOrDefault(p => p.ID == model.ProjectID) is null) {
                return BadRequest("Project does not exists");
            }

            if (db.Project.First(p => p.ID == model.ProjectID).ResearcherID != ut.ResearcherID) {
                return BadRequest("Access denied");
            }

            if (db.Question.Where(q => q.ProjectID == model.ProjectID)
                .FirstOrDefault(q => q.QuestionNumber == model.QuestionNumber) != null) {
                return BadRequest("Question with current position number exists in project");
            }

            try {
                int id = db.Question.Max(q => q.ID) + 1;
                db.Question.Add(new Question() {
                    ID = id,
                    ProjectID = model.ProjectID,
                    InstructionText = model.InstructionText,
                    QuestionNumber = model.QuestionNumber,
                    QuestionSubtext = model.QuestionSubtext,
                    QuestionText = model.QuestionText,
                    QuestionTypeID = 2
                });

                db.SaveChanges();

                db.ChooseQuestion.Add(new ChooseQuestion() {
                    QuestionID = id,
                    IsSingleChoice = model.IsSingleChoice
                });

                foreach (var item in model.AnswerVariants) {
                    db.MultipleChooseQuestionVariant.Add(new MultipleChooseQuestionVariant() {
                        AnswerText = item,
                        QuestionID = id
                    });
                }

                db.SaveChanges();
            }
            catch (Exception ex) {
                return BadRequest(ex);
                //return BadRequest("Cannot add new participant. Possible reasons:" +
                //   "You");
            }

            return Ok("Question added successfully");
        }

        [HttpPost]
        public ActionResult AddDiscreteSliderQuestion([FromBody] DiscreteSliderQuestionAddModel model, string token) {
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }

            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorised");
            }

            if (db.Project.FirstOrDefault(p => p.ID == model.ProjectID) is null) {
                return BadRequest("Project does not exists");
            }

            if (db.Project.First(p => p.ID == model.ProjectID).ResearcherID != ut.ResearcherID) {
                return BadRequest("Access denied");
            }

            if (model.DiscreteSliderMinValue >= model.DiscreteSliderMaxValue) {
                return BadRequest("Maximum values should be greater than minimum values");
            }

            if (db.Question.Where(q => q.ProjectID == model.ProjectID)
                .FirstOrDefault(q => q.QuestionNumber == model.QuestionNumber) != null) {
                return BadRequest("Question with current position number exists in project");
            }

            try {
                int id = db.Question.Max(q => q.ID) + 1;
                db.Question.Add(new Question() {
                    ID = id,
                    ProjectID = model.ProjectID,
                    InstructionText = model.InstructionText,
                    QuestionNumber = model.QuestionNumber,
                    QuestionSubtext = model.QuestionSubtext,
                    QuestionText = model.QuestionText,
                    QuestionTypeID = 3
                });

                db.SaveChanges();

                db.DiscreteSliderQuestion.Add(new DiscreteSliderQuestion() {
                    DiscreteSliderMinValue = model.DiscreteSliderMinValue,
                    DiscreteSliderMaxValue = model.DiscreteSliderMaxValue,
                    QuestionID = id,
                    ScaleText = model.ScaleText
                });

                db.SaveChanges();
            }
            catch (Exception ex) {
                return BadRequest(ex);
                //return BadRequest("Cannot add new participant. Possible reasons:" +
                //   "You");
            }

            return Ok("Question added successfully");
        }

        [HttpPost]
        public ActionResult AddSliderQuestion([FromBody] SliderQuestionAddModel model, string token) {
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }

            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorised");
            }

            if (db.Project.FirstOrDefault(p => p.ID == model.ProjectID) is null) {
                return BadRequest("Project does not exists");
            }

            if (db.Project.First(p => p.ID == model.ProjectID).ResearcherID != ut.ResearcherID) {
                return BadRequest("Access denied");
            }

            if (model.SliderMinValue >= model.SliderMaxValue) {
                return BadRequest("Maximum values should be greater than minimum values");
            }

            if (db.Question.Where(q => q.ProjectID == model.ProjectID)
                .FirstOrDefault(q => q.QuestionNumber == model.QuestionNumber) != null) {
                return BadRequest("Question with current position number exists in project");
            }

            try {
                int id = db.Question.Max(q => q.ID) + 1;
                db.Question.Add(new Question() {
                    ID = id,
                    ProjectID = model.ProjectID,
                    InstructionText = model.InstructionText,
                    QuestionNumber = model.QuestionNumber,
                    QuestionSubtext = model.QuestionSubtext,
                    QuestionText = model.QuestionText,
                    QuestionTypeID = 4
                });

                db.SaveChanges();

                db.SliderQuestion.Add(new SliderQuestion() {
                    SliderMaxValue = model.SliderMaxValue,
                    SliderMinValue = model.SliderMinValue,
                    IsDiscrete = model.IsDiscrete,
                    LeftText = model.LeftText,
                    RightText = model.RightText,
                    QuestionID = id
                });

                db.SaveChanges();
            }
            catch (Exception ex) {
                return BadRequest(ex);
                //return BadRequest("Cannot add new participant. Possible reasons:" +
                //   "You");
            }

            return Ok("Question added successfully");
        }

        //PUT
        [HttpPut]
        public ActionResult EditProject([FromBody] Project project, string token) {
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }

            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorised");
            }

            if (db.Project.First(p => p.ID == project.ID).ResearcherID != ut.ResearcherID) {
                return BadRequest("Access denied");
            }

            if (project.DateEnd <= project.DateStart) {
                return BadRequest("End date should be greater than start date");
            }

            if ((project.DateEnd is null || project.DateStart is null) && project.ProjectStatusID > 2) {
                return BadRequest("Started or ended project should have dates of start and end of current project");
            }

            var projectEdited = db.Project.FirstOrDefault(p => p.ID == project.ID);
            if (projectEdited is null) {
                return BadRequest("Project does not exist");
            }

            try {
                projectEdited.Title = project.Title;
                projectEdited.DateEnd = project.DateEnd;
                projectEdited.DateStart = project.DateStart;
                projectEdited.InstructionText = project.InstructionText;
                projectEdited.NotificationTimeout = project.NotificationTimeout;
                projectEdited.IsNotificationsEnabled = project.IsNotificationsEnabled;
                projectEdited.NotificationCountPerDay = project.NotificationCountPerDay;
                db.SaveChanges();
            }
            catch (Exception ex) {
                return BadRequest(ex);
                //return BadRequest("Error while adding project");
            }

            return Ok("Project edited successfully");
        }

        [HttpPut]
        public ActionResult EditParticipant([FromBody] ParticipantModel participantModel, string token) {
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }

            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorised");
            }

            var participantEdited = db.Participant.FirstOrDefault(p => p.ID == participantModel.ID);
            if (participantEdited is null) {
                return BadRequest("Participant does not exist");
            }

            if (db.Project.FirstOrDefault(p => p.ID == participantModel.ProjectID) is null) {
                return BadRequest("Project does not exist");
            }

            if (db.Project.First(p => p.ID == participantModel.ProjectID).ResearcherID != ut.ResearcherID) {
                return BadRequest("Access denied");
            }

            var timeNotificationEnd =
                TimeSpan.Parse(participantModel.TimeNotificationEnd, CultureInfo.InvariantCulture);

            var timeNotificationStart =
                TimeSpan.Parse(participantModel.TimeNotificationStart, CultureInfo.InvariantCulture);

            if (timeNotificationEnd <= timeNotificationStart) {
                return BadRequest(
                    "Time of end sending notifications should be greater than time of start sending notifications");
            }

            if ((timeNotificationEnd - timeNotificationStart).TotalMinutes <
                participantModel.NotificationCountPerDay * participantModel.NotificationMinValueVariation) {
                return BadRequest("Cannot send notifications properly by current parameters: " +
                                  $"No enough time for sending {participantModel.NotificationCountPerDay} notifications");
            }

            try {
                participantEdited.ID = participantModel.ID;
                participantEdited.ProjectID = participantModel.ProjectID;
                participantEdited.TimeNotificationEnd = timeNotificationEnd;
                participantEdited.TimeNotificationStart = timeNotificationStart;
                participantEdited.NotificationCountPerDay = participantModel.NotificationCountPerDay;
                participantEdited.NotificationMinValueVariation = participantModel.NotificationMinValueVariation;
                participantEdited.ParticipantStatusID = db.ParticipantStatus
                    .First(s => s.StatusText == participantModel.ParticipantStatus).ID;
                db.SaveChanges();
            }
            catch (Exception ex) {
                return BadRequest(ex);
                //return BadRequest("Cannot add new participant. Possible reasons:" +
                //   "You");
            }

            return Ok("Participant edited successfully");
        }

        [HttpPut]
        public ActionResult EditAffectGridQuestion([FromBody] AffectGridQuestionAddModel model, string token) {
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }

            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorised");
            }

            var questionEdited = db.Question.FirstOrDefault(p => p.ID == model.ID);
            if (questionEdited is null) {
                return BadRequest("Question does not exist");
            }

            if (db.Project.FirstOrDefault(p => p.ID == model.ProjectID) is null) {
                return BadRequest("Project does not exists");
            }

            if (db.Project.First(p => p.ID == model.ProjectID).ResearcherID != ut.ResearcherID) {
                return BadRequest("Access denied");
            }

            if (model.MinX >= model.MaxX || model.MinY >= model.MaxY) {
                BadRequest("Maximum values should be greater than minimum values");
            }

            if (db.Question.Where(q => q.ProjectID == model.ProjectID)
                .FirstOrDefault(q => q.QuestionNumber == model.QuestionNumber) != null) {
                return BadRequest("Question with current position number exists in project");
            }

            var affectQuestionEdited = db.AffectGridQuestion.First(q => q.QuestionID == questionEdited.ID);

            try {
                questionEdited.ProjectID = model.ProjectID;
                questionEdited.InstructionText = model.InstructionText;
                questionEdited.QuestionNumber = model.QuestionNumber;
                questionEdited.QuestionSubtext = model.QuestionSubtext;
                questionEdited.QuestionText = model.QuestionText;
                questionEdited.QuestionTypeID = 1;

                affectQuestionEdited.DelimiterCount = model.DelimiterCount;
                affectQuestionEdited.IsGridVisible = model.IsGridVisible;
                affectQuestionEdited.LeftLowerSquareText = model.LeftLowerSquareText;
                affectQuestionEdited.LeftUpperSquareText = model.LeftUpperSquareText;
                affectQuestionEdited.RightLowerSquareText = model.RightLowerSquareText;
                affectQuestionEdited.RightUpperSquareText = model.RightUpperSquareText;
                affectQuestionEdited.LowerXText = model.LowerXText;
                affectQuestionEdited.LowerYText = model.LowerYText;
                affectQuestionEdited.UpperXText = model.UpperXText;
                affectQuestionEdited.UpperYText = model.UpperYText;
                affectQuestionEdited.MaxX = model.MaxX;
                affectQuestionEdited.MaxY = model.MaxY;
                affectQuestionEdited.MinX = model.MinX;
                affectQuestionEdited.MinY = model.MinY;

                db.SaveChanges();
            }
            catch (Exception ex) {
                return BadRequest(ex);
                //return BadRequest("Cannot add new participant. Possible reasons:" +
                //   "You");
            }

            return Ok("Question edited successfully");
        }

        [HttpPut]
        public ActionResult EditChooseQuestion([FromBody] ChooseQuestionAddModel model, string token) {
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }

            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorised");
            }

            var questionEdited = db.Question.FirstOrDefault(p => p.ID == model.ID);
            if (questionEdited is null) {
                return BadRequest("Question does not exist");
            }

            if (db.Project.FirstOrDefault(p => p.ID == model.ProjectID) is null) {
                return BadRequest("Project does not exists");
            }

            if (db.Project.First(p => p.ID == model.ProjectID).ResearcherID != ut.ResearcherID) {
                return BadRequest("Access denied");
            }

            if (db.Question.Where(q => q.ProjectID == model.ProjectID)
                .FirstOrDefault(q => q.QuestionNumber == model.QuestionNumber) != null) {
                return BadRequest("Question with current position number exists in project");
            }

            var chooseQuestion = db.ChooseQuestion.First(q => q.QuestionID == questionEdited.ID);

            try {
                questionEdited.ProjectID = model.ProjectID;
                questionEdited.InstructionText = model.InstructionText;
                questionEdited.QuestionNumber = model.QuestionNumber;
                questionEdited.QuestionSubtext = model.QuestionSubtext;
                questionEdited.QuestionText = model.QuestionText;
                questionEdited.QuestionTypeID = 2;

                chooseQuestion.IsSingleChoice = model.IsSingleChoice;

                int index = 0;
                var multipleChooseQuestionVariants = new List<MultipleChooseQuestionVariant>();
                foreach (var item in model.AnswerVariants) {
                    multipleChooseQuestionVariants.Add(new MultipleChooseQuestionVariant());
                    multipleChooseQuestionVariants[index].AnswerText = item;
                    multipleChooseQuestionVariants[index].QuestionID = questionEdited.ID;
                    index++;
                }
                db.MultipleChooseQuestionVariant.RemoveRange(
                    db.MultipleChooseQuestionVariant.Where(q => q.QuestionID == questionEdited.ID));

                db.SaveChanges();                
                
                db.MultipleChooseQuestionVariant.AddRange(multipleChooseQuestionVariants);

                db.SaveChanges();
            }
            catch (Exception ex) {
                return BadRequest(ex);
                //return BadRequest("Cannot add new participant. Possible reasons:" +
                //   "You");
            }

            return Ok("Question edited successfully");
        }

        [HttpPut]
        public ActionResult EditDiscreteSliderQuestion([FromBody] DiscreteSliderQuestionAddModel model, string token) {
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }

            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorised");
            }

            var questionEdited = db.Question.FirstOrDefault(p => p.ID == model.ID);
            if (questionEdited is null) {
                return BadRequest("Question does not exist");
            }

            if (db.Project.FirstOrDefault(p => p.ID == model.ProjectID) is null) {
                return BadRequest("Project does not exists");
            }

            if (db.Project.First(p => p.ID == model.ProjectID).ResearcherID != ut.ResearcherID) {
                return BadRequest("Access denied");
            }

            if (model.DiscreteSliderMinValue >= model.DiscreteSliderMaxValue) {
                return BadRequest("Maximum values should be greater than minimum values");
            }

            if (db.Question.Where(q => q.ProjectID == model.ProjectID)
                .FirstOrDefault(q => q.QuestionNumber == model.QuestionNumber) != null) {
                return BadRequest("Question with current position number exists in project");
            }

            var discreteSliderEdited = db.DiscreteSliderQuestion.First(q => q.QuestionID == questionEdited.ID);

            try {
                questionEdited.ProjectID = model.ProjectID;
                questionEdited.InstructionText = model.InstructionText;
                questionEdited.QuestionNumber = model.QuestionNumber;
                questionEdited.QuestionSubtext = model.QuestionSubtext;
                questionEdited.QuestionText = model.QuestionText;
                questionEdited.QuestionTypeID = 3;

                discreteSliderEdited.DiscreteSliderMinValue = model.DiscreteSliderMinValue;
                discreteSliderEdited.DiscreteSliderMaxValue = model.DiscreteSliderMaxValue;
                discreteSliderEdited.ScaleText = model.ScaleText;

                db.SaveChanges();
            }
            catch (Exception ex) {
                return BadRequest(ex);
                //return BadRequest("Cannot add new participant. Possible reasons:" +
                //   "You");
            }

            return Ok("Question edited successfully");
        }

        [HttpPut]
        public ActionResult EditSliderQuestion([FromBody] SliderQuestionAddModel model, string token) {
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }

            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorised");
            }

            var questionEdited = db.Question.FirstOrDefault(p => p.ID == model.ID);
            if (questionEdited is null) {
                return BadRequest("Question does not exist");
            }

            if (db.Project.FirstOrDefault(p => p.ID == model.ProjectID) is null) {
                return BadRequest("Project does not exists");
            }

            if (db.Project.First(p => p.ID == model.ProjectID).ResearcherID != ut.ResearcherID) {
                return BadRequest("Access denied");
            }

            if (model.SliderMinValue >= model.SliderMaxValue) {
                return BadRequest("Maximum values should be greater than minimum values");
            }

            if (db.Question.Where(q => q.ProjectID == model.ProjectID)
                .FirstOrDefault(q => q.QuestionNumber == model.QuestionNumber) != null) {
                return BadRequest("Question with current position number exists in project");
            }

            var sliderEdited = db.SliderQuestion.First(q => q.QuestionID == questionEdited.ID);

            try {
                questionEdited.ProjectID = model.ProjectID;
                questionEdited.InstructionText = model.InstructionText;
                questionEdited.QuestionNumber = model.QuestionNumber;
                questionEdited.QuestionSubtext = model.QuestionSubtext;
                questionEdited.QuestionText = model.QuestionText;
                questionEdited.QuestionTypeID = 4;

                sliderEdited.SliderMaxValue = model.SliderMaxValue;
                sliderEdited.SliderMinValue = model.SliderMinValue;
                sliderEdited.IsDiscrete = model.IsDiscrete;
                sliderEdited.LeftText = model.LeftText;
                sliderEdited.RightText = model.RightText;

                db.SaveChanges();
            }
            catch (Exception ex) {
                return BadRequest(ex);
                //return BadRequest("Cannot add new participant. Possible reasons:" +
                //   "You");
            }

            return Ok("Question edited successfully");
        }

        [HttpDelete]
        public ActionResult DeleteParticipant(int id, string token) {
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }

            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorised");
            }

            var participant = db.Participant.FirstOrDefault(p => p.ID == id);

            if (participant is null) {
                return BadRequest("Participant does not exist");
            }

            if (db.Project.First(p => p.ID == participant.ProjectID).ResearcherID != ut.ResearcherID) {
                return BadRequest("Access denied");
            }

            try {
                db.Participant.Remove(participant);
                db.SaveChanges();
            }
            catch (Exception ex) {
                return BadRequest(ex);
                //return BadRequest("Cannot add new participant. Possible reasons:" +
                //   "You");
            }

            return Ok("Participant deleted successfully");
        }

        [HttpDelete]
        public ActionResult DeleteProject(int id, string token) {
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }

            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorised");
            }

            var project = db.Project.FirstOrDefault(p => p.ID == id);

            if (project is null) {
                return BadRequest("Project does not exist");
            }

            if (project.ResearcherID != ut.ResearcherID) {
                return BadRequest("Access denied");
            }

            try {
                db.Project.Remove(project);
                db.SaveChanges();
            }
            catch (Exception ex) {
                return BadRequest(ex);
                //return BadRequest("Cannot add new participant. Possible reasons:" +
                //   "You");
            }

            return Ok("Project deleted successfully");
        }

        [HttpDelete]
        public ActionResult DeleteQuestion(int id, string token) {
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }

            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorised");
            }

            var question = db.Question.FirstOrDefault(p => p.ID == id);

            if (question is null) {
                return BadRequest("Question does not exist");
            }

            if (db.Project.First(q => q.ID == question.ProjectID).ResearcherID != ut.ResearcherID) {
                return BadRequest("Access denied");
            }

            try {
                db.Question.Remove(question);
                db.SaveChanges();
            }
            catch (Exception ex) {
                return BadRequest(ex);
                //return BadRequest("Cannot add new participant. Possible reasons:" +
                //   "You");
            }

            return Ok("Question deleted successfully");
        }

        [HttpGet]
        public ActionResult GetAllProjects(string token) {
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }

            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorised");
            }

            var projects = db.Project.Where(p => p.ResearcherID == ut.ResearcherID).ToList();

            return Ok(MakeListProjectModel(projects));
        }

        [HttpGet]
        public ActionResult GetInfoForProjectById(int id, string token) {
            if (token.Length != TokenManagement.TokenLength) {
                token = WebUtility.UrlDecode(token);
            }

            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorized");
            }

            var project = db.Project.FirstOrDefault(p => p.ID == id);

            if (project is null) {
                return NotFound();
            }

            if (project.ResearcherID != ut.ResearcherID) {
                return BadRequest("Access denied");
            }

            var projectModel = new ProjectModel() {
                ID = project.ID,
                Status = db.ProjectStatus.First(s => s.ID == project.ProjectStatusID).StatusText,
                Title = project.Title,
                InstructionText = project.InstructionText,
                DateStart = project.DateStart,
                DateEnd = project.DateEnd,
                NotificationCountPerDay = project.NotificationCountPerDay,
                NotificationTimeout = project.NotificationTimeout,
                IsNotificationsEnabled = project.IsNotificationsEnabled,
                Questions = MakeListQuestionModel(db.Question.Where(q => q.ProjectID == id).ToList()),
                ParticipantInfoList = MakeListParticipantModel(project)
            };

            //var questions = MakeListQuestionModel(db.Question.Where(q => q.ProjectID == id).ToList());

            return Ok(projectModel);
        }

        private List<ParticipantModel> MakeListParticipantModel(Project project) {
            using TrackerContext db = new();
            var participants = db.Participant
                .Where(p => p.ProjectID == project.ID)
                .ToList();
            var participantModelList = new List<ParticipantModel>();

            foreach (var item in participants) {
                var participantAnswers = db.ParticipantAnswer
                    .Where(a => a.ParticipantID == item.ID)
                    .ToList();
                var participantAnswerModelList = new List<ParticipantAnswerModel>();

                foreach (var i in participantAnswers) {
                    participantAnswerModelList.Add(new ParticipantAnswerModel() {
                        ID = i.ID,
                        AnswerDate = i.AnswerDate,
                        Answer = i.AnswerText,
                        IsAnswered = i.IsAnswered,
                        QuestionNumber = db.Question.First(q => q.ID == i.QuestionID).QuestionNumber
                    });
                }

                participantModelList.Add(new ParticipantModel() {
                    ID = item.ID,
                    ProjectID = project.ID,
                    ParticipantStatus = db.ParticipantStatus.First(s => s.ID == item.ParticipantStatusID).StatusText,
                    NotificationCountPerDay = item.NotificationCountPerDay,
                    NotificationMinValueVariation = item.NotificationMinValueVariation,
                    TimeNotificationEnd = item.TimeNotificationEnd.ToString(),
                    TimeNotificationStart = item.TimeNotificationStart.ToString(),
                    ParticipantAnswerModelList = participantAnswerModelList
                });
            }

            return participantModelList;
        }

        private List<ProjectModel> MakeListProjectModel(List<Project> projects) {
            using TrackerContext db = new();
            var projectModelList = new List<ProjectModel>();

            foreach (var item in projects) {
                projectModelList.Add(new ProjectModel() {
                    ID = item.ID,
                    Status = db.ProjectStatus.FirstOrDefault(s => s.ID == item.ProjectStatusID)?.StatusText,
                    Title = item.Title,
                    InstructionText = item.InstructionText,
                    DateStart = item.DateStart,
                    DateEnd = item.DateEnd,
                    Questions = MakeListQuestionModel(db.Question.Where(q => q.ProjectID == item.ID).ToList()),
                    ParticipantInfoList = MakeListParticipantModel(item),
                    NotificationCountPerDay = item.NotificationCountPerDay,
                    NotificationTimeout = item.NotificationTimeout,
                    IsNotificationsEnabled = item.IsNotificationsEnabled,
                });
            }

            return projectModelList;
        }

        private List<object> MakeListQuestionModel(List<Question> questions) {
            using TrackerContext db = new();
            var questionModelList = new List<object>();

            foreach (var item in questions) {
                var type = db.QuestionType.First(i => i.ID == item.QuestionTypeID).QuestionTypeText;
                switch (type) {
                    case "AffectGrid":
                        var affectGridQuestion = db.AffectGridQuestion.First(q => q.QuestionID == item.ID);
                        questionModelList.Add(new AffectGridQuestionModel() {
                            ID = item.ID,
                            QuestionType = type,
                            QuestionNumber = item.QuestionNumber,
                            QuestionSubtext = item.QuestionSubtext,
                            QuestionText = item.QuestionText,
                            InstructionText = item.InstructionText,
                            MinX = affectGridQuestion.MinX,
                            MaxX = affectGridQuestion.MaxX,
                            MinY = affectGridQuestion.MinY,
                            MaxY = affectGridQuestion.MaxY,
                            DelimiterCount = affectGridQuestion.DelimiterCount,
                            IsGridVisible = affectGridQuestion.IsGridVisible,
                            UpperXText = affectGridQuestion.UpperXText,
                            LowerXText = affectGridQuestion.LowerXText,
                            UpperYText = affectGridQuestion.UpperYText,
                            LowerYText = affectGridQuestion.LowerYText,
                            LeftUpperSquareText = affectGridQuestion.LeftUpperSquareText,
                            RightUpperSquareText = affectGridQuestion.RightUpperSquareText,
                            LeftLowerSquareText = affectGridQuestion.LeftLowerSquareText,
                            RightLowerSquareText = affectGridQuestion.RightLowerSquareText
                        });
                        break;
                    case "Choose":
                        var chooseQuestion = db.ChooseQuestion.First(q => q.QuestionID == item.ID);
                        var answers = db.MultipleChooseQuestionVariant
                            .Where(q => q.QuestionID == item.ID)
                            .Select(p => p.AnswerText)
                            .ToList();
                        questionModelList.Add(new ChooseQuestionModel() {
                            ID = item.ID,
                            QuestionType = type,
                            QuestionNumber = item.QuestionNumber,
                            QuestionSubtext = item.QuestionSubtext,
                            QuestionText = item.QuestionText,
                            InstructionText = item.InstructionText,
                            IsSingleChoice = chooseQuestion.IsSingleChoice,
                            Answers = answers
                        });
                        break;
                    case "DiscreteSlider":
                        var discreteSliderQuestion = db.DiscreteSliderQuestion.First(q => q.QuestionID == item.ID);
                        questionModelList.Add(new DiscreteSliderQuestionModel() {
                            ID = item.ID,
                            QuestionType = type,
                            QuestionNumber = item.QuestionNumber,
                            QuestionSubtext = item.QuestionSubtext,
                            QuestionText = item.QuestionText,
                            InstructionText = item.InstructionText,
                            DiscreteSliderMaxValue = discreteSliderQuestion.DiscreteSliderMaxValue,
                            DiscreteSliderMinValue = discreteSliderQuestion.DiscreteSliderMinValue,
                            ScaleTexts = discreteSliderQuestion.ScaleText
                                .Split("#$%^", StringSplitOptions.RemoveEmptyEntries).ToList()
                        });
                        break;
                    case "Slider":
                        var sliderQuestion = db.SliderQuestion.First(q => q.QuestionID == item.ID);
                        questionModelList.Add(new SliderQuestionModel() {
                            ID = item.ID,
                            QuestionType = type,
                            QuestionNumber = item.QuestionNumber,
                            QuestionSubtext = item.QuestionSubtext,
                            QuestionText = item.QuestionText,
                            InstructionText = item.InstructionText,
                            SliderMaxValue = sliderQuestion.SliderMaxValue,
                            SliderMinValue = sliderQuestion.SliderMinValue,
                            IsDiscrete = sliderQuestion.IsDiscrete,
                            LeftText = sliderQuestion.LeftText,
                            RightText = sliderQuestion.RightText
                        });
                        break;
                }
            }

            return questionModelList;
        }
    }
}