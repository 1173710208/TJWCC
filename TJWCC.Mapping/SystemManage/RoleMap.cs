using TJWCC.Domain.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace TJWCC.Mapping.SystemManage
{
    public class RoleMap : EntityTypeConfiguration<RoleEntity>
    {
        public RoleMap()
        {
            this.ToTable("SYS_ROLE");
            this.HasKey(t => t.ID);
        }
    }
}
