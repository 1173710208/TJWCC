using TJWCC.Domain.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace TJWCC.Mapping.SystemManage
{
    public class UserMap : EntityTypeConfiguration<UserEntity>
    {
        public UserMap()
        {
            this.ToTable("SYS_USER");
            this.HasKey(t => t.ID);
        }
    }
}
