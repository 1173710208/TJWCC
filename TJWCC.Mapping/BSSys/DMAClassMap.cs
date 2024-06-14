using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class DMAClassMap : EntityTypeConfiguration<DMAClassEntity>
    {
        public DMAClassMap()
        {
            this.ToTable("DMAClass");
            this.HasKey(t => t.ID);
        }
    }
}
