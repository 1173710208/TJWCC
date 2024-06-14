using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.Entity.DataDisplay;

namespace TJWCC.Mapping.DataDisplay
{
    public class Dis_OffLineClassMap: EntityTypeConfiguration<DIS_OFFLINECLASSEntity>
    {
        public Dis_OffLineClassMap()
        {
            this.ToTable("DIS_OFFLINECLASS");
            this.HasKey(t => t.ID);
        }
    }
}
