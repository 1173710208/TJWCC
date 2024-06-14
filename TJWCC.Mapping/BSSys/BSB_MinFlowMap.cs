using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSB_MinFlowMap : EntityTypeConfiguration<BSB_MinFlowEntity>
    {
        public BSB_MinFlowMap()
        {
            this.ToTable("BSB_MinFlow");
            this.HasKey(t => t.id);
        }
    }
}
