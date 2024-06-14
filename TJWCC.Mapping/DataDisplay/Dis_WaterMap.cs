using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.Entity.DataDisplay;

namespace TJWCC.Mapping.DataDisplay
{
    public class Dis_WaterMap : EntityTypeConfiguration<DIS_WATEREntity>
    {
        public Dis_WaterMap()
        {
            this.ToTable("DIS_WATER");
            this.HasKey(t => t.ID);
        }
    }
}
