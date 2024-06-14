using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.WCC;

namespace TJWCC.Mapping.WCC
{
    public class CC_WetherRecordMap : EntityTypeConfiguration<CC_WetherRecordEntity>
    {
        public CC_WetherRecordMap()
        {
            this.ToTable("CC_WetherRecord");
            this.HasKey(t => t.ID);
        }
    }
}
