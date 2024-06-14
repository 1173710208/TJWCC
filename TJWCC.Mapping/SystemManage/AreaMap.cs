using TJWCC.Domain.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace TJWCC.Mapping.SystemManage
{
    public class AreaMap : EntityTypeConfiguration<AreaEntity>
    {
        public AreaMap()
        {
            this.ToTable("SYS_AREA");
            this.HasKey(t => t.ID);
        }
    }
}
