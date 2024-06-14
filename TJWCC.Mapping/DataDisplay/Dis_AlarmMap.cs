using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.Entity.DataDisplay;
using System.Data.Entity.ModelConfiguration;

namespace TJWCC.Mapping.DataDisplay
{
    class Dis_AlarmMap : EntityTypeConfiguration<DIS_ALARMEntity>
    {
        public Dis_AlarmMap()
        {
            this.ToTable("DIS_ALARM");
            this.HasKey(t => t.ID);
        }
    }
}
