using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSOT_SysInfoMap : EntityTypeConfiguration<BSOT_SysInfoEntity>
    {
        public BSOT_SysInfoMap()
        {
            this.ToTable("BSOT_SysInfo");
            this.HasKey(t => t.GradeNum);
        }
    }
}
