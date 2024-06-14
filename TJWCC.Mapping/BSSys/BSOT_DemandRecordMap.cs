using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSOT_DemandRecordMap : EntityTypeConfiguration<BSOT_DemandRecordEntity>
    {
        public BSOT_DemandRecordMap()
        {
            this.ToTable("BSOT_DemandRecord");
            this.HasKey(t => t.NodeID);
        }
    }
}
