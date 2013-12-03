using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTS.Models
{
    public class TransactionsVM
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public int TeacherID { get; set; }
        public string TeacherName { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}