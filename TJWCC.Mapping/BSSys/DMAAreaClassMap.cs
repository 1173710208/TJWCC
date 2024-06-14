using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class DMAAreaClassMap : EntityTypeConfiguration<DMAAreaClassEntity>
    {
        public DMAAreaClassMap()
        {
            this.ToTable("DMAAreaClass");
            this.HasKey(t => t.ID);
        }
    }
}
