using TJWCC.Domain.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace TJWCC.Mapping.SystemManage
{
    public class Business_InfoMap : EntityTypeConfiguration<Business_InfoEntity>
    {
        public Business_InfoMap()
        {
            this.ToTable("BUSINESS_INFO");
            this.HasKey(t => t.BUSINESSID);
        }
    }
}
