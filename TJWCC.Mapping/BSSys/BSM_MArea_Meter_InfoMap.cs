using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSM_MArea_Meter_InfoMap : EntityTypeConfiguration<BSM_MArea_Meter_InfoEntity>
    {
        public BSM_MArea_Meter_InfoMap()
        {
            this.ToTable("BSM_MArea_Meter_Info");
            this.HasKey(t => t.ID);
        }
    }
}
