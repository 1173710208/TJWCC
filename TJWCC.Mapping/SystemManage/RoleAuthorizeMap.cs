using TJWCC.Domain.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace TJWCC.Mapping.SystemManage
{
    public class RoleAuthorizeMap : EntityTypeConfiguration<RoleAuthorizeEntity>
    {
        public RoleAuthorizeMap()
        {
            this.ToTable("SYS_ROLEAUTHORIZE");
            this.HasKey(t => t.ID);
        }
    }
}
