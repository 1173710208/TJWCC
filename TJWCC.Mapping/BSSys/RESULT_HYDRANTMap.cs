using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class RESULT_HYDRANTMap : EntityTypeConfiguration<RESULT_HYDRANTEntity>
    {
        public RESULT_HYDRANTMap()
        {
            this.ToTable("RESULT_HYDRANT");
            this.HasKey(t => t.ElementId);
        }
    }
}
