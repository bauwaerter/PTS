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
        }
    }
}
