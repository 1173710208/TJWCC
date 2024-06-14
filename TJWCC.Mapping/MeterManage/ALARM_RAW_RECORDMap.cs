using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.Entity.MeterManage;

namespace TJWCC.Mapping.MeterManage
{
    class ALARM_RAW_RECORDMap : EntityTypeConfiguration<ALARM_RAW_RECORDEntity>
    {
        public ALARM_RAW_RECORDMap()
        {
            this.ToTable("ALARM_RAW_RECORD");
            this.HasKey(t => t.ID);
        }
    }
}
