using TJWCC.Domain.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace TJWCC.Mapping.SystemManage
{
    public class ModuleMap : EntityTypeConfiguration<ModuleEntity>
    {
        public ModuleMap()
        {
            this.ToTable("SYS_MODULE");
            this.HasKey(t => t.ID);
        }
    }
}
