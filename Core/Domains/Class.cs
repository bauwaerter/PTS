using System;
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

    public class SubjectToClass : BaseEntity
    {
        public virtual int SubjectId { get; set; }
        public virtual int ClassId { get; set; }
    }

    public class Class : BaseEntity
    {
        public virtual string Location { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime EndTime { get; set; }
        public virtual DateTime Duration { get; set; }
        public virtual int SubjectID { get; set; }
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
    }

    public class ReviewClass : BaseEntity
    {
        public virtual string Comment { get; set; }
        public virtual int Rating { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual int StudentId { get; set; }
        public virtual int ClassId { get; set; }
    }
}
