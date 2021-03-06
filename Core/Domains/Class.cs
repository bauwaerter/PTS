﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core.Domains
{
    public class Subject : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }

    public class Class : BaseEntity
    {
        public virtual int TeacherId { get; set; }
        public virtual int LocationId { get; set; }
        public virtual string Description { get; set; }
        public virtual TimeSpan StartTime { get; set; }
        public virtual TimeSpan EndTime { get; set; }
        public virtual double Duration { get; set; }
        public virtual int SubjectID { get; set; }
        public virtual bool Active { get; set; }

        #region navigation
        public virtual Location Location { get; set; }
        public virtual TeacherUser Teacher { get; set; }
        public virtual Subject Subject { get; set; }

        private ICollection<ReviewClass> _reviewClass;
        public virtual ICollection<ReviewClass> ReviewClass
        {
            get { return _reviewClass ?? (_reviewClass = new List<ReviewClass>()); }
            protected set { _reviewClass = value; }
        }

        private ICollection<Class_Meeting_Dates> _classMeetingDates;
        public virtual ICollection<Class_Meeting_Dates> ClassMeetingDates
        {
            get { return _classMeetingDates ?? (_classMeetingDates = new List<Class_Meeting_Dates>()); }
            protected set { _classMeetingDates = value; }
        } 
        #endregion
    }

    public class Location : BaseEntity
    {
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Address { get; set; }
        public virtual int ZipCode { get; set; }
        public virtual string Country { get; set; }
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
    }

    public class Class_Meeting_Dates : BaseEntity
    {
        public virtual int ClassId { get; set; }
        public virtual DateTime Date { get; set; }
    }

    public class Enrolled : BaseEntity
    {
        public virtual int StudentId { get; set; }
        public virtual int ClassId { get; set; }

        #region navigation
        
        #endregion
    }

    public class ReviewClass : BaseEntity
    {
        public virtual string Comment { get; set; }
        public virtual int Rating { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual int StudentId { get; set; }
        public virtual int ClassId { get; set; }
    }


    public class Schedule : BaseEntity
    {
        public virtual bool Monday { get; set; }
        public virtual bool Tuesday { get; set; }
        public virtual bool Wednesday { get; set; }
        public virtual bool Thursday { get; set; }
        public virtual bool Friday { get; set; }
        public virtual bool Saturday { get; set; }
        public virtual bool Sunday { get; set; }
        public virtual TimeSpan MonStartTime { get; set; }
        public virtual TimeSpan MonEndTime { get; set; }
        public virtual TimeSpan TuesStartTime { get; set; }
        public virtual TimeSpan TuesEndTime { get; set; }
        public virtual TimeSpan WedStartTime { get; set; }
        public virtual TimeSpan WedEndTime { get; set; }
        public virtual TimeSpan ThursStartTime { get; set; }
        public virtual TimeSpan ThursEndTime { get; set; }
        public virtual TimeSpan FriStartTime { get; set; }
        public virtual TimeSpan FriEndTime { get; set; }
        public virtual TimeSpan SatStartTime { get; set; }
        public virtual TimeSpan SatEndTime { get; set; }
        public virtual TimeSpan SunStartTime { get; set; }
        public virtual TimeSpan SunEndTime { get; set; }

        #region navigation
        
        #endregion
    }

    public class Request : BaseEntity {
        public virtual bool Monday { get; set; }
        public virtual bool Tuesday { get; set; }
        public virtual bool Wednesday { get; set; }
        public virtual bool Thursday { get; set; }
        public virtual bool Friday { get; set; }
        public virtual bool Saturday { get; set; }
        public virtual bool Sunday { get; set; }
        public virtual int TeacherId { get; set; }
        public virtual int StudentId { get; set; }
        public virtual String Status { get; set; }
        #region navigation

        #endregion
    }
}
