using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSB_ForecastFlowMap : EntityTypeConfiguration<BSB_ForecastFlowEntity>
    {
        public BSB_ForecastFlowMap()
        {
            this.ToTable("BSB_ForecastFlow");
            this.HasKey(t => t.id);
        }
    }
}
