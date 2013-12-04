using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace Core.Domains
{
    public enum UserRole
    {
        Admin,
        Teacher,
        Student,
        StudentTeacher
    }

    public class User : BaseEntity
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual DateTime DOB { get; set; }
        public virtual int SSN { get; set; }
        public virtual string Email { get; set; }
        public virtual UserRole Role { get; set; }
        public virtual int LocationId { get; set; }
        public virtual string PassWord { get; set; }
        public virtual string PasswordSalt { get; set; }
        public virtual string Major { get; set; }
        public virtual string Education { get; set; }
        #region navigation
        public virtual Location Location { get; set; }

        private ICollection<Enrolled> _enrolled;
        public virtual ICollection<Enrolled> Enrolled
        {
            get { return _enrolled ?? (_enrolled = new List<Enrolled>()); }
            protected set { _enrolled = value; }
        }
        #endregion

    }
       

    public class TeacherUser : BaseEntity
    {
        public virtual decimal HourlyRate { get; set; }
        public virtual decimal ClassRate { get; set; }
        public virtual bool Active { get; set; }
        public virtual int ScheduleId { get; set; }
                
        #region navigation
        public virtual User User { get; set; }
        public virtual Schedule Schedule { get; set; }
        
        private ICollection<ReviewTeacher> _reviewTeacher;
        public virtual ICollection<ReviewTeacher> ReviewTeacher
	    {
            get { return _reviewTeacher ?? (_reviewTeacher = new List<ReviewTeacher>()); }
            protected set { _reviewTeacher = value; }
        }

        private ICollection<Teacher_Offers> _teacherOffers;
        public virtual ICollection<Teacher_Offers> TeacherOffers
        {
            get { return _teacherOffers ?? (_teacherOffers = new List<Teacher_Offers>()); }
            protected set { _teacherOffers = value; }
        }
        #endregion
    }

    public class Qualifications : BaseEntity
    {
        public virtual int TeacherUserId { get; set; }
        public virtual string Type { get; set; }
        public virtual string Description { get; set; }
    }

    public class ReviewTeacher : BaseEntity
    {
        public virtual string Comment { get; set; }
        public virtual int Rating { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual int StudentId { get; set; }
        public virtual int TeacherId { get; set; }

        #region navigation
        
        #endregion
    }

    public class Teacher_Offers : BaseEntity
    {
        public virtual int TeacherId { get; set; }
        public virtual int SubjectId { get; set; }

        #region navigation
        public virtual Subject Subject { get; set; }
        #endregion
    }

    public class Tutors : BaseEntity
    {
        public virtual int StudentId { get; set; }
        public virtual int TeacherId { get; set; }
        public virtual int SubjectId { get; set; }
    }
}
