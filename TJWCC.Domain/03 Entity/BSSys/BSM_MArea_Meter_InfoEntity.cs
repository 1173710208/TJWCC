using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BSM_MArea_Meter_InfoEntity : IEntity<BSM_MArea_Meter_InfoEntity>
    {
        public long ID { get; set; }
        public Nullable<long> Measure_Area_ID { get; set; }
        public Nullable<long> WMeter_ID { get; set; }
        public Nullable<int> Direction { get; set; }
        public string Measure_Grade { get; set; }
        public string Remark { get; set; }
    }
}
