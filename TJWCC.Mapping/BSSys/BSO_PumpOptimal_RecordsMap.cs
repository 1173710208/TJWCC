using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSO_PumpOptimal_RecordsMap : EntityTypeConfiguration<BSO_PumpOptimal_RecordsEntity>
    {
        public BSO_PumpOptimal_RecordsMap()
        {
            this.ToTable("BSO_PumpOptimal_Records");
            this.HasKey(t => t.ID);
        }
    }
}
