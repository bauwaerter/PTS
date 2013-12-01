using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTS.Models
{
    public class ReviewClassViewModel
    {
        public string Description { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public int ClassID { get; set; }
        public int StudentID { get; set; }

        public ReviewClassViewModel()
        {

        }


    }
}