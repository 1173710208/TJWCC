using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.WCC;

namespace TJWCC.Mapping.WCC
{
    public class CC_PressTargetMap : EntityTypeConfiguration<CC_PressTargetEntity>
    {
        public CC_PressTargetMap()
        {
            this.ToTable("CC_PressTarget");
            this.HasKey(t => t.ID);
        }
    }
}
