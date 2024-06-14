using TJWCC.Domain.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace TJWCC.Mapping.SystemManage
{
    public class ItemsDetailMap : EntityTypeConfiguration<ItemsDetailEntity>
    {
        public ItemsDetailMap()
        {
            this.ToTable("SYS_ITEMSDETAIL");
            this.HasKey(t => t.ID);
        }
    }
}
