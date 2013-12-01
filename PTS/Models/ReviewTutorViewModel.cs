using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTS.Models
{
    public class ReviewTutorViewModel
    {
        public string TutorName { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public int TeacherID { get; set; }
        public int StudentID { get; set; }

        public ReviewTutorViewModel()
        {

        }


    }
}