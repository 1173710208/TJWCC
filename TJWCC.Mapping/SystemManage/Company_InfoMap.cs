using TJWCC.Domain.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace TJWCC.Mapping.SystemManage
{
    public class Company_InfoMap : EntityTypeConfiguration<Company_InfoEntity>
    {
        public Company_InfoMap()
        {
            this.ToTable("COMPANY_INFO");
            this.HasKey(t => t.COMPANYID);
        }
    }
}
