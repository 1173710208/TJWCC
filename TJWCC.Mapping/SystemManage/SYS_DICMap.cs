using TJWCC.Domain.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace TJWCC.Mapping.SystemManage
{
    public class SYS_DICMap : EntityTypeConfiguration<SYS_DICEntity>
    {
        public SYS_DICMap()
        {
            this.ToTable("SYS_DIC");
            this.HasKey(t => t.ID);
        }
    }
}
