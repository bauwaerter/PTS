using System.Data.Entity.ModelConfiguration;
using Core.Domains;

namespace Data.Mappings
{
    public class UserMap : EntityTypeConfiguration<User>
    {

        public UserMap()
        {
            //Table
            ToTable("User", "sean_1");

            //Primary Key
            HasKey(u => u.Id);

            //Property(u => u.UserName).IsRequired().HasMaxLength(50);

        }

    }
}
