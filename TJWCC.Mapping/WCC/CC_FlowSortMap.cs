using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.WCC;

namespace TJWCC.Mapping.WCC
{
    public class CC_FlowSortMap : EntityTypeConfiguration<CC_FlowSortEntity>
    {
        public CC_FlowSortMap()
        {
            this.ToTable("CC_FlowSort");
            this.HasKey(t => t.ID);
        }
    }
}
