using System.Data.Entity.ModelConfiguration;
using Core.Domains;

namespace Data.Mappings
{
    public class UserMap : EntityTypeConfiguration<User>
    {

        public UserMap()
        {
            //Table
            ToTable("User", "dbo");
            //Primary Key
            HasKey(u => u.Id);
            
            //Property(u => u.UserName).IsRequired().HasMaxLength(50);
        }

    }

    public class StudentUserMap : EntityTypeConfiguration<StudentUser>
    {
        public StudentUserMap()
        {
            ToTable("StudentUser", "dbo");
            HasKey(s => s.Id);
        }

    }

    public class TeacherUserMap : EntityTypeConfiguration<TeacherUser>
    {
        public TeacherUserMap()
        {
            ToTable("TeacherUser", "dbo");
            HasKey(t => t.Id);

            HasMany(x => x.ReviewTeacher)
                .WithOptional()
                .HasForeignKey(xs => xs.TeacherId);

            HasMany(x => x.TeacherOffers)
               .WithOptional()
               .HasForeignKey(xs => xs.TeacherId);

        }
    }

    public class QualificationsMap : EntityTypeConfiguration<Qualifications>
    {
        public QualificationsMap()
        {
            ToTable("Qualifications", "dbo");
            HasKey(q => q.TeacherUserId);
        }
    }

    public class ReviewTeacherMap : EntityTypeConfiguration<ReviewTeacher>
    {
        public ReviewTeacherMap()
        {
            ToTable("ReviewTeacher", "dbo");
            HasKey(t => t.Id);//might need more keys
        }
    }

    public class Teacher_OffersMap : EntityTypeConfiguration<Teacher_Offers>
    {
        public Teacher_OffersMap()
        {
            ToTable("Teacher_Offers", "dbo");
            HasKey(t => t.Id);
        }
    }

    
    public class TutorsMap : EntityTypeConfiguration<Tutors>
    {
        public TutorsMap()
        {
            ToTable("Tutors", "dbo");
            HasKey(t => t.Id);
        }
    }
}
