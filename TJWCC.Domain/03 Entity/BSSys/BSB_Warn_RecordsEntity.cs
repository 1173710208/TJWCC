using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BSB_Warn_RecordsEntity : IEntity<BSB_Warn_RecordsEntity>
    {
        public long id { get; set; }
        public string Information { get; set; }
        public Nullable<int> WarnType { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<decimal> Value { get; set; }
        public string StationName { get; set; }
        public string StationKey { get; set; }
        public Nullable<long> ElementId { get; set; }
        public string Remark { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string TagKey { get; set; }
        public int? Number { get; set; }
        public bool? IsRead { get; set; }
    }
}
