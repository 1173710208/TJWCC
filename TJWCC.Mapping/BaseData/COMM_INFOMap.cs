using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.Entity.BaseData;

namespace TJWCC.Mapping.BaseData
{
    class COMM_INFOMap : EntityTypeConfiguration<COMM_INFOEntity>
    {
        public COMM_INFOMap()
        {
            this.ToTable("COMM_INFO");
            this.HasKey(t => t.COMMID);
        }
    }
}
