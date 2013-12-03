using System.Data.Entity.ModelConfiguration;
using Core.Domains;

namespace Data.Mappings
{
    public class ClassMap : EntityTypeConfiguration<Class>
    {
        public ClassMap()
        {
            ToTable("Class", "dbo");
            HasKey(c => c.Id);

            HasMany(x => x.ReviewClass)
                .WithOptional()
                .HasForeignKey(xs => xs.ClassId);
        }
    }

    public class SubjectMap : EntityTypeConfiguration<Subject>
    {
        public SubjectMap()
        {
            ToTable("Subject", "dbo");
            HasKey(s => s.Id);
        }
    }

    public class Class_Meeting_Dates_Map : EntityTypeConfiguration<Class_Meeting_Dates>
    {
        public Class_Meeting_Dates_Map()
        {
            ToTable("Class_Meeting_Dates", "dbo");
            HasKey(cm => cm.Id);
        }
    }

    public class EnrolledMap : EntityTypeConfiguration<Enrolled>
    {
        public EnrolledMap()
        {
            ToTable("Enrolled", "dbo");
            HasKey(e => e.StudentId);
        }
    }

    public class ReviewClassMap : EntityTypeConfiguration<ReviewClass>
    {
        public ReviewClassMap()
        {
            ToTable("ReviewClass", "dbo");
            HasKey(r => r.Id);
        }
    }

    public class ScheduleMap : EntityTypeConfiguration<Schedule>
    {
        public ScheduleMap()
        {
            ToTable("Schedule", "dbo");
            HasKey(s => s.Id);
        }
    }

    public class RequestMap : EntityTypeConfiguration<Request> {
        public RequestMap() {
            ToTable("Request", "dbo");
            HasKey(rq => rq.Id);
        }
    }

}
