using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTS.Models
{
    public class TutorAd
    {
        public string TutorName { get; set; }
        public string Review { get; set; }
        public int Rating { get; set; }
        public DateTime Date { get; set; }
    }
}