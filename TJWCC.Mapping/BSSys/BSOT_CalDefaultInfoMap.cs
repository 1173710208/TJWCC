using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSOT_CalDefaultInfoMap : EntityTypeConfiguration<BSOT_CalDefaultInfoEntity>
    {
        public BSOT_CalDefaultInfoMap()
        {
            this.ToTable("BSOT_CalDefaultInfo");
            this.HasKey(t => t.Duration);
        }
    }
}
