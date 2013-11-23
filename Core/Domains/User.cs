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
        public virtual int? LocationId { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public virtual string PassWord { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        //public virtual string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password salt.
        /// </summary>
        /// <value>
        /// The password salt.
        /// </value>
        //public virtual string PasswordSalt { get; set; }
        #region navigation
        public virtual Location Location { get; set; }
        #endregion

    }

    public class StudentUser : BaseEntity
    {
        public virtual string Major { get; set; }
        public virtual string Education { get; set; }

        #region navigation properties
        public virtual User User { get; set; }

        //private ICollection<Class> _classes;
        //public virtual ICollection<Class> Classes
        //{
        //    get {return _classes ?? (_classes = new List<Class>());}
        //    protected set {_classes = value;}
        //}

        #endregion

    }

    public class TeacherUser : BaseEntity
    {
        public virtual decimal HourlyRate { get; set; }
        public virtual decimal ClassRate { get; set; }
        
        #region navigation
        public virtual User User { get; set; }
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
    }

    public class Teacher_Offers : BaseEntity
    {
        public virtual int TeacherId { get; set; }
        public virtual int SubjectId { get; set; }
    }

    public class Teaches : BaseEntity
    {
        public virtual int TeacherId { get; set; }
        public virtual int ClassId { get; set; }
    }

    public class Tutors : BaseEntity
    {
        public virtual int StudentId { get; set; }
        public virtual int TeacherId { get; set; }
        public virtual int SubjectId { get; set; }
    }
}
