using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.WCC;

namespace TJWCC.Mapping.WCC
{
    public class CC_CleanStatisticsMap : EntityTypeConfiguration<CC_CleanStatisticsEntity>
    {
        public CC_CleanStatisticsMap()
        {
            this.ToTable("CC_CleanStatistics");
            this.HasKey(t => t.ID);
        }
    }
}
