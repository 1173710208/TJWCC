using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.Entity.MeterManage;

namespace TJWCC.Mapping.MeterManage
{
    class METERDAY_RECORDMap : EntityTypeConfiguration<METERDAY_RECORDEntity>
    {
        public METERDAY_RECORDMap()
        {
            this.ToTable("METERDAY_RECORD");
            this.HasKey(t => t.ID);
        }
    }
}
