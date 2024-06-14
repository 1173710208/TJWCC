using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSB_LinkedTableMap : EntityTypeConfiguration<BSB_LinkedTableEntity>
    {
        public BSB_LinkedTableMap()
        {
            this.ToTable("BSB_LinkedTable");
            this.HasKey(t => t.ID);
        }
    }
}
