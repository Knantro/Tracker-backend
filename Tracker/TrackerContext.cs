using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tracker.Entities;

namespace Tracker {
    public class TrackerContext : DbContext {
        public DbSet<AffectGridQuestion> AffectGridQuestion { get; set; }
        public DbSet<ChooseQuestion> ChooseQuestion { get; set; }
        public DbSet<DiscreteSliderQuestion> DiscreteSliderQuestion { get; set; }
        public DbSet<MultipleChooseQuestionVariant> MultipleChooseQuestionVariant { get; set; }
        public DbSet<Participant> Participant { get; set; }
        public DbSet<ParticipantAnswer> ParticipantAnswer { get; set; }
        public DbSet<ParticipantStatus> ParticipantStatus { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<ProjectStatus> ProjectStatus { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<QuestionType> QuestionType { get; set; }
        public DbSet<Researcher> Researcher { get; set; }
        public DbSet<SliderQuestion> SliderQuestion { get; set; }
        public DbSet<UserToken> UserToken { get; set; }

        private static DbContextOptionsBuilder<TrackerContext> optionsBuilder = new DbContextOptionsBuilder<TrackerContext>();

        public TrackerContext() : base(optionsBuilder.UseMySql(
            Startup.Configuration.GetConnectionString("TrackerContext"),
            new MySqlServerVersion(new Version(10, 5, 9))
        ).Options) {
            Database.EnsureCreated();
        }

        public TrackerContext(DbContextOptions<TrackerContext> options) : base(options) {
            Database.EnsureCreated();
        }
    }
}