using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.WCC;

namespace TJWCC.Mapping.WCC
{
    public class CC_TheDateMap : EntityTypeConfiguration<CC_TheDateEntity>
    {
        public CC_TheDateMap()
        {
            this.ToTable("CC_TheDate");
            this.HasKey(t => t.ID);
        }
    }
}
