using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BS_SCADA_TAG_CURRENTMap : EntityTypeConfiguration<BS_SCADA_TAG_CURRENTEntity>
    {
        public BS_SCADA_TAG_CURRENTMap()
        {
            this.ToTable("BS_SCADA_TAG_CURRENT");
            this.HasKey(t => t.ID);
        }
    }
}
