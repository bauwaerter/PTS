using System.Data.Entity.ModelConfiguration;
using Core.Domains;

namespace Data.Mappings {
    public class LoginMap : EntityTypeConfiguration<Login> {
        public LoginMap() {
            // Table
            ToTable("Login");
            HasKey(l => l.Id);

            // Relations
            HasRequired(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);
        }
    }
}
