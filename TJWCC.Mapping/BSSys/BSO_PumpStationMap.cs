using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSO_PumpStationMap : EntityTypeConfiguration<BSO_PumpStationEntity>
    {
        public BSO_PumpStationMap()
        {
            this.ToTable("BSO_PumpStation");
            this.HasKey(t => t.StationId);
        }
    }
}
