using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.WCC
{
    public class GY_StationRecordsEntity : IEntity<GY_StationRecordsEntity>
    {
        public long ID { get; set; }
        public decimal? Value1 { get; set; }
        public decimal? Value2 { get; set; }
        public decimal? Value3 { get; set; }
        public DateTime? Save_date { get; set; }
        public int StationId { get; set; }
        public int TypeId { get; set; }
        public int? number { get; set; }
    }
}
