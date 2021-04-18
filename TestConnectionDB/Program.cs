using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Tracker;
using Tracker.Models;

namespace TestConnectionDB {
    class Program {
        static void Main(string[] args) {
            // using (TrackerContext db = new TrackerContext(options))
            // {
            //     QuestionType ps1 = new QuestionType { ID = 1, QuestionTypeText = "smth" };
            //
            //     db.QuestionType.AddRange(ps1);
            //     db.SaveChanges();
            // }
            // using (TrackerContext db = new TrackerContext()) {
            //     var statusList = db.ParticipantStatus.ToList();
            //     Console.WriteLine("List:");
            //     foreach (ParticipantStatus u in statusList) {
            //         Console.WriteLine($"{u.ID}.{u.StatusText}");
            //     }
            // }
            // WebRequest request = WebRequest.Create("http://45.67.230.70/swagger/");
            // WebResponse response = request.GetResponse();
            // using (Stream stream = response.GetResponseStream())
            // {
            //     using (StreamReader reader = new(stream))
            //     {
            //         string line = "";
            //         while ((line = reader.ReadLine()) != null)
            //         {
            //             Console.WriteLine(line);
            //         }
            //     }     
            // }
            // response.Close();
            // Console.WriteLine("Запрос выполнен");
            // Console.Read();
        }
    }
}