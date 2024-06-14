using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.Entity.DataDisplay;

namespace TJWCC.Mapping.DataDisplay
{
    public class Dis_SignalStrengthMap : EntityTypeConfiguration<DIS_SIGNALSTRENGTHEntity>
    {
        public Dis_SignalStrengthMap()
        {
            this.ToTable("DIS_SIGNALSTRENGTH");
            this.HasKey(t => t.ID);
        }
    }
}
