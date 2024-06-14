using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSO_PumpParamMap : EntityTypeConfiguration<BSO_PumpParamEntity>
    {
        public BSO_PumpParamMap()
        {
            this.ToTable("BSO_PumpParam");
            this.HasKey(t => t.PumpId);
        }
    }
}
