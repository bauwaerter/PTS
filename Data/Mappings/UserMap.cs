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
}
