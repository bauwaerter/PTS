using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Domains;
using Service.Interfaces;
using Service.Services;
using System.Web.Mvc;

namespace PTS.Models
{
    public class ClassViewModel : LocationVM
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int SubjectId { get; set; }
        public string TeacherName { get; set; }
        public string Description { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public double Duration { get; set; }
        public string Dates { get; set; }
        public string AverageRating { get; set; }
        public string SubjectName { get; set; }
        public IEnumerable<SelectListItem> Subjects { get; set; }
        
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }

        public ClassViewModel()
        {
            
        }
    }
}