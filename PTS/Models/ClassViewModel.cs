using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Domains;
using Service.Interfaces;
using Service.Services;

namespace PTS.Models
{
    public class ClassViewModel
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public int SubjectId { get; set; }
        public string TeacherName { get; set; }
        public string Description { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public double Duration { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public int ZipCode { get; set; }
        public string Country { get; set; }

        public ClassViewModel()
        {
            
        }
    }
}