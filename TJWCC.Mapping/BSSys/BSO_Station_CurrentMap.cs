using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSO_Station_CurrentMap : EntityTypeConfiguration<BSO_Station_CurrentEntity>
    {
        public BSO_Station_CurrentMap()
        {
            this.ToTable("BSO_Station_Current");
            this.HasKey(t => t.ID);
        }
    }
}
