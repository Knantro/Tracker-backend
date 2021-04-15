using System;
using Microsoft.EntityFrameworkCore;
using Tracker.Models;

namespace Tracker {
    public class TrackerContext : DbContext {
        public DbSet<AffectGridQuestion> AffectGridQuestion { get; set; }
        public DbSet<ChoiceQuestion> ChoiceQuestion { get; set; }
        public DbSet<DiscreteSliderQuestion> DiscreteSliderQuestion { get; set; }
        public DbSet<MultipleChoiseQuestionVariant> MultipleChoiseQuestionVariant { get; set; }
        public DbSet<Participant> Participant { get; set; }
        public DbSet<ParticipantAnswer> ParticipantAnswer { get; set; }
        public DbSet<ParticipantStatus> ParticipantStatus { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectStatus> ProjectStatus { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<QuestionType> QuestionType { get; set; }
        public DbSet<Researcher> Researcher { get; set; }
        public DbSet<SliderQuestion> SliderQuestion { get; set; }
        
        public TrackerContext(DbContextOptions<TrackerContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}