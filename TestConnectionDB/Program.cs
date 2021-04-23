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
            string token = "G$&RgFi3s;6*<<9/EMh)\"S[,\"kyeHW>f<YIciwt<kuUs\"9e~e9q8/),sGxP{";
            Console.WriteLine(WebUtility.UrlDecode(token));

            Console.WriteLine(TimeSpan.Parse("11:00"));
        }
    }
}