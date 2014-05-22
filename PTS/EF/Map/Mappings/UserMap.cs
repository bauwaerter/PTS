using System.Data.Entity.ModelConfiguration;
using Core.Domains;

namespace Map.Mappings {
    public class UserMap : EntityTypeConfiguration<User> {

        public UserMap(){
            ToTable("User", "dbo");
            HasKey(u => u.Id);
        }
    }
}
