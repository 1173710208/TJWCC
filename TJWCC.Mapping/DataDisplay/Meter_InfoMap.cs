using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.Entity.DataDisplay;

namespace TJWCC.Mapping.DataDisplay
{
    public class Meter_InfoMap : EntityTypeConfiguration<METER_INFOEntity>
    {
        public Meter_InfoMap()
        {
            this.ToTable("METER_INFO");
            this.HasKey(t => t.NBMETERID);
        }
    }
}
