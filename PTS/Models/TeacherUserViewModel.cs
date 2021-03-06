﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Domains;

namespace PTS.Models
{
    public class TeacherUserViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal ClassRate { get; set; }
        public string AverageRating { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public IList<Teacher_Offers> TeacherOffers { get; set; }

        public TeacherUserViewModel()
        {

        }

    }

    public class TutorAvailabilityViewModel
    {
        public int ScheduleId { get; set; }
        public string Sunday { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }

        public TutorAvailabilityViewModel()
        {

        }
    }

    public class TutorSubjectViewModel
    {
        public int SubjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public TutorSubjectViewModel()
        {

        }
    }
}