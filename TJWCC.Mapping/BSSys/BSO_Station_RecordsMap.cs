using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Mapping.BSSys
{
    public class BSO_Station_RecordsMap : EntityTypeConfiguration<BSO_Station_RecordsEntity>
    {
        public BSO_Station_RecordsMap()
        {
            this.ToTable("BSO_Station_Records");
            this.HasKey(t => t.ID);
        }
    }
}
