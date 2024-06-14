using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSB_ValveAreaMap : EntityTypeConfiguration<BSB_ValveAreaEntity>
    {
        public BSB_ValveAreaMap()
        {
            this.ToTable("BSB_ValveArea");
            this.HasKey(t => t.Valveid);
        }
    }
}
