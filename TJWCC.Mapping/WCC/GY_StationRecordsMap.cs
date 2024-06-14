using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using TJWCC.Domain.Entity.WCC;

namespace TJWCC.Mapping.WCC
{
    public class GY_StationRecordsMap : EntityTypeConfiguration<GY_StationRecordsEntity>
    {
        public GY_StationRecordsMap()
        {
            this.ToTable("GY_StationRecords");
            this.HasKey(t => t.ID);
        }
    }
}
