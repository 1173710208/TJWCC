using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class RESULT_JUNCTIONMap : EntityTypeConfiguration<RESULT_JUNCTIONEntity>
    {
        public RESULT_JUNCTIONMap()
        {
            this.ToTable("RESULT_JUNCTION");
            this.HasKey(t => t.ElementId);
        }
    }
}
