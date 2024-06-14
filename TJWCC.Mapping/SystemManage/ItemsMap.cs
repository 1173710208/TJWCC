using TJWCC.Domain.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;


namespace TJWCC.Mapping.SystemManage
{
    public class Itemsmap : EntityTypeConfiguration<ItemsEntity>
    {
        public Itemsmap()
        {
            this.ToTable("SYS_ITEMS");
            this.HasKey(t => t.ID);
        }
    }
}
