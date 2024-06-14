using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.Entity.DataDisplay;

namespace TJWCC.Mapping.DataDisplay
{
    public class Dis_MakerClassMap: EntityTypeConfiguration<DIS_MAKERCLASSEntity>
    {
        public Dis_MakerClassMap()
        {
            this.ToTable("DIS_MAKERCLASS");
            this.HasKey(t => t.ID); 
        }
    }
}
