using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.WCC;

namespace TJWCC.Mapping.WCC
{
    public class CC_DispatchPlanMap : EntityTypeConfiguration<CC_DispatchPlanEntity>
    {
        public CC_DispatchPlanMap()
        {
            this.ToTable("CC_DispatchPlan");
            this.HasKey(t => t.ID);
        }
    }
}
