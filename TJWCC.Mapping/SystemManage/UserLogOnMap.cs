using TJWCC.Domain.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace TJWCC.Mapping.SystemManage
{
    public class UserLogOnMap : EntityTypeConfiguration<UserLogOnEntity>
    {
        public UserLogOnMap()
        {
            this.ToTable("SYS_USERLOGON");
            this.HasKey(t => t.ID);
        }
    }
}
