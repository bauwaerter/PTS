using System.Data.Entity.ModelConfiguration;
using Core.Domains;

namespace Data.Mappings
{
    public class StudentUserMap : EntityTypeConfiguration<StudentUser>
    {
        public StudentUserMap()
        {
            ToTable("StudentUser", "sean_1");
            HasKey(s => s.Id);
        }
    }
}
