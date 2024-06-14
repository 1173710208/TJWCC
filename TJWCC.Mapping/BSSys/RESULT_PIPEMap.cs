using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class RESULT_PIPEMap : EntityTypeConfiguration<RESULT_PIPEEntity>
    {
        public RESULT_PIPEMap()
        {
            this.ToTable("RESULT_PIPE");
            this.HasKey(t => t.ElementId);
        }
    }
}
