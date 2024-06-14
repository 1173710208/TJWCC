using TJWCC.Domain.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace TJWCC.Mapping.SystemManage
{
    public class SubBusi_InfoMap : EntityTypeConfiguration<SubBusi_InfoEntity>
    {
        public SubBusi_InfoMap()
        {
            this.ToTable("SubBusi_Info");
            this.HasKey(t => t.SUBBUSIID);
        }
    }
}
