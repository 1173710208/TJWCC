using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.Entity.MeterManage;

namespace TJWCC.Mapping.MeterManage
{
    class METER_RECORDMap : EntityTypeConfiguration<METER_RECORDEntity>
    {
        public METER_RECORDMap()
        {
            this.ToTable("METER_RECORD");
            this.HasKey(t => t.ID);
        }
    }
}
