using TJWCC.Domain.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace TJWCC.Mapping.SystemManage
{
    public class ModuleButtonMap : EntityTypeConfiguration<ModuleButtonEntity>
    {
        public ModuleButtonMap()
        {
            this.ToTable("SYS_MODULEBUTTON");
            this.HasKey(t => t.ID);
        }
    }
}
