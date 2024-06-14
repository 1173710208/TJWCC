using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Domain.Entity.BaseData;

namespace TJWCC.Mapping.BaseData
{
    class OperatType_InfoMap : EntityTypeConfiguration<OperatType_InfoEntity>
    {
        public OperatType_InfoMap()
        {
            this.ToTable("OperatType_Info");
            this.HasKey(t => t.OTYPEID);
        }
    }
}
