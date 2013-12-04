using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTS.Models
{
    public class CalendarEvent
    {
        public int id { get; set; }
        public string title { get; set; }
        public bool allday { get; set; }
        public string start { get; set; }
        public string end { get; set; }
    }
}