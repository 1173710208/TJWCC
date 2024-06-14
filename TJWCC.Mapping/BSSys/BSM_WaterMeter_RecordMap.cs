using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSM_WaterMeter_RecordMap : EntityTypeConfiguration<BSM_WaterMeter_RecordEntity>
    {
        public BSM_WaterMeter_RecordMap()
        {
            this.ToTable("BSM_WaterMeter_Record");
            this.HasKey(t => t.ID);
        }
    }
}
