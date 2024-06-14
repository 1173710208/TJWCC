using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.Entity.BaseData;

namespace TJWCC.Mapping.BaseData
{
    class METERMAKER_INFOMap : EntityTypeConfiguration<METERMAKER_INFOEntity>
    {
        public METERMAKER_INFOMap()
        {
            this.ToTable("METERMAKER_INFO");
            this.HasKey(t => t.MAKERID);
        }
    }
}
