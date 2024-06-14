using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BSO_PumpStationEntity : IEntity<BSO_PumpStationEntity>
    {
        public long StationId { get; set; }
        public string StationName { get; set; }
        public int ConsPumps { get; set; }
        public int SpeedPumps { get; set; }
        public int HELRange { get; set; }
        public decimal LowerLimit { get; set; }
        public decimal PUpLimit { get; set; }
        public decimal PLowerLimit { get; set; }
        public int OutputNumber { get; set; }
        public int InputNumber { get; set; }
        public string TelePhone { get; set; }
        public string Address { get; set; }
        public int RefreshTime { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreaterId { get; set; }
        public string Remark { get; set; }
    }
}
