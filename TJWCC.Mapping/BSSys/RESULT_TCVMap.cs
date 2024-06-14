using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class RESULT_TCVMap : EntityTypeConfiguration<RESULT_TCVEntity>
    {
        public RESULT_TCVMap()
        {
            this.ToTable("RESULT_TCV");
            this.HasKey(t => t.ElementId);
        }
    }
}
