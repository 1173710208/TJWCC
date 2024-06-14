using TJWCC.Domain.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace TJWCC.Mapping.SystemManage
{
    public class OrganizeMap : EntityTypeConfiguration<OrganizeEntity>
    {
        public OrganizeMap()
        {
            this.ToTable("SYS_ORGANIZE");
            this.HasKey(t => t.ID);
        }
    }
}
