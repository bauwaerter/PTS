using System.Data.Entity.ModelConfiguration;
using Core.Domains;

namespace Data.Mappings
{
    public class PaymentMap : EntityTypeConfiguration<Payment>
    {
        public PaymentMap()
        {
            ToTable("Payment", "dbo");
            HasKey(p => p.Id);
                
        }
    }
}
