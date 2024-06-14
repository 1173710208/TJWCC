using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.Entity.DataDisplay;

namespace TJWCC.Mapping.DataDisplay
{
    public class Dis_WaterCompanyClassMap:EntityTypeConfiguration<DIS_WATERCOMPANYCLASSEntity>
    {
        public Dis_WaterCompanyClassMap()
        {
            this.ToTable("DIS_WATERCOMPANYCLASS");
            this.HasKey(t => t.ID);
        }
    }
}
