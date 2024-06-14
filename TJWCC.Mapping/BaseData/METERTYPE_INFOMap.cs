using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.Entity.BaseData;


namespace TJWCC.Mapping.BaseData
{
    class METERTYPE_INFOMap : EntityTypeConfiguration<METERTYPE_INFOEntity>
    {
        public METERTYPE_INFOMap()
        {
            this.ToTable("METERTYPE_INFO");
            this.HasKey(t => t.TYPEID);
        }
    }
}
