using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using Tracker.Entities;
using Microsoft.AspNetCore.Mvc;
using Tracker.Models;

namespace Tracker.Controllers {
    [ApiController]
    [Route(RouteTemplates.DefaultRoute)]
    public class ParticipantController : ControllerBase {
        [HttpGet]
        public ActionResult GetQuestions(string token) {
            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorized");
            }

            var participant = db.Participant.FirstOrDefault(p => p.ID == ut.ParticipantID);
            if (participant is null) {
                return BadRequest("Cannot find participant with current id");
            }
            
            var project = db.Project.FirstOrDefault(p => p.ID == participant.ProjectID);
            if (project is null) {
                return BadRequest("Cannot find project for current participant");
            }
            
            var questions = db.Question.Where(q => q.ProjectID == project.ID).ToList();
            
            
            return Ok(MakeListQuestionModel(questions));
        }

        [HttpPost]
        public ActionResult SendAnswer([FromBody]ParticipantAnswer model, string token) {
            using TrackerContext db = new();
            var ut = db.UserToken.FirstOrDefault(t => t.Token.Equals(token));
            if (ut is null) {
                return Unauthorized("You're not authorized");
            }
            
            var participant = db.Participant.FirstOrDefault(p => p.ID == ut.ParticipantID);
            if (participant is null) {
                return BadRequest("Cannot find participant with current id");
            }
            
            var project = db.Project.FirstOrDefault(p => p.ID == participant.ProjectID);
            if (project is null) {
                return BadRequest("Cannot find project for current participant");
            }

            var question = db.Question.FirstOrDefault(q => q.ID == model.QuestionID);
            if (question is null) {
                return BadRequest("Question not found");
            }

            try {
                db.ParticipantAnswer.Add(model);
                db.SaveChanges();
            }
            catch (Exception ex) {
                return BadRequest(ex);
            }

            return Ok();
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
                            QuestionType = type,
                            QuestionNumber = item.QuestionNumber,
                            QuestionSubtext = item.QuestionSubtext,
                            QuestionText = item.QuestionText,
                            InstructionText = item.InstructionText,
                            DiscreteSliderMaxValue = discreteSliderQuestion.DiscreteSliderMaxValue,
                            DiscreteSliderMinValue = discreteSliderQuestion.DiscreteSliderMinValue,
                            ScaleTexts = discreteSliderQuestion.ScaleText.Split("#$%^", StringSplitOptions.RemoveEmptyEntries).ToList()
                        });
                        break;
                    case "Slider":
                        var sliderQuestion = db.SliderQuestion.First(q => q.QuestionID == item.ID);
                        questionModelList.Add(new SliderQuestionModel() {
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