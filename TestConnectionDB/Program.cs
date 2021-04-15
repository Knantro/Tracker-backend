using System;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Tracker;
using Tracker.Models;

namespace TestConnectionDB {
    class Program {
        static void Main(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<TrackerContext>();

            var options = optionsBuilder
                    .UseMySql(
                        "server=localhost;user=knantro;password=3jrcfql478;database=TrackerDB;",
                        new MySqlServerVersion(new Version(10, 5, 9))
                    )
                    .Options;
            // using (TrackerContext db = new TrackerContext(options))
            // {
            //     QuestionType ps1 = new QuestionType { ID = 1, QuestionTypeText = "smth" };
            //
            //     db.QuestionType.AddRange(ps1);
            //     db.SaveChanges();
            // }
            using (TrackerContext db = new(options)) {
                var statusList = db.ParticipantStatus.ToList();
                Console.WriteLine("List:");
                foreach (ParticipantStatus u in statusList) {
                    Console.WriteLine($"{u.ID}.{u.StatusText}");
                }
            }
        }
    }
}