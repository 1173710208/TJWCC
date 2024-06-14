using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSB_SourceData_ClMap : EntityTypeConfiguration<BSB_SourceData_ClEntity>
    {
        public BSB_SourceData_ClMap()
        {
            this.ToTable("BSB_SourceData_Cl");
            this.HasKey(t => t.ID);
        }
    }
}
