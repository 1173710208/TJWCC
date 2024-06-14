using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.WCC;

namespace TJWCC.Mapping.WCC
{
    public class GY_PumpStationInfoMap : EntityTypeConfiguration<GY_PumpStationInfoEntity>
    {
        public GY_PumpStationInfoMap()
        {
            this.ToTable("GY_PumpStationInfo");
            this.HasKey(t => t.ID);
        }
    }
}
