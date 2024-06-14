using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BS_SCADA_TAG_INFOMap : EntityTypeConfiguration<BS_SCADA_TAG_INFOEntity>
    {
        public BS_SCADA_TAG_INFOMap()
        {
            this.ToTable("BS_SCADA_TAG_INFO");
            this.HasKey(t => t.ID);
        }
    }
}
