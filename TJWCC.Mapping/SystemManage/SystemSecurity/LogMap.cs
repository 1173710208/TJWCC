using TJWCC.Domain.Entity.SystemSecurity;
using System.Data.Entity.ModelConfiguration;

namespace TJWCC.Mapping.SystemSecurity
{
    public class LogMap : EntityTypeConfiguration<LogEntity>
    {
        public LogMap()
        {
            this.ToTable("SYS_LOG");
            this.HasKey(t => t.ID);
        }
    }
}
