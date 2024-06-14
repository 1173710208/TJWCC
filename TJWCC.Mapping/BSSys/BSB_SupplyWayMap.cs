using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSB_SupplyWayMap : EntityTypeConfiguration<BSB_SupplyWayEntity>
    {
        public BSB_SupplyWayMap()
        {
            this.ToTable("BSB_SupplyWay");
            this.HasKey(t => t.ElementId);
        }
    }
}
