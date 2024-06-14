using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSB_Warn_RecordsMap : EntityTypeConfiguration<BSB_Warn_RecordsEntity>
    {
        public BSB_Warn_RecordsMap()
        {
            this.ToTable("BSB_Warn_Records");
            this.HasKey(t => t.id);
        }
    }
}
