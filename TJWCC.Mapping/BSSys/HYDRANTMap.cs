using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class HYDRANTMap : EntityTypeConfiguration<HYDRANTEntity>
    {
        public HYDRANTMap()
        {
            this.ToTable("HYDRANT");
            this.HasKey(t => t.OBJECTID);
        }
    }
}
