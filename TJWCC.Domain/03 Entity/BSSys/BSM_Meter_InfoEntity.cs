using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BSM_Meter_InfoEntity : IEntity<BSM_Meter_InfoEntity>
    {
        public long ID { get; set; }
        public Nullable<long> ElementID { get; set; }
        public string Meter_Name { get; set; }
        public Nullable<decimal> Geo_x { get; set; }
        public Nullable<decimal> Geo_y { get; set; }
        public string Station_Key { get; set; }
        public string Station_Unit { get; set; }
        public string MeasureAreaId { get; set; }
        public string Measure_Grade { get; set; }
        public Nullable<int> DistrictAreaId { get; set; }
        public string Original { get; set; }
        public Nullable<decimal> tz { get; set; }
        public string WMeter_ID { get; set; }
        public Nullable<int> Meter_Type { get; set; }
        public string Explain { get; set; }
        public string Flow_Image { get; set; }
        public string Remark { get; set; }
        public Nullable<decimal> PressureUp { get; set; }
        public Nullable<decimal> PressureDown { get; set; }
        public Nullable<decimal> ClUp { get; set; }
        public Nullable<decimal> ClDown { get; set; }
        public Nullable<decimal> PHUp { get; set; }
        public Nullable<decimal> PHDown { get; set; }
        public Nullable<decimal> Turbidity { get; set; }
        public Nullable<short> Display { get; set; }
        public string evaluation_flag { get; set; }
        public string default_flag { get; set; }
        public Nullable<decimal> standard_press { get; set; }
        public Nullable<decimal> default_press { get; set; }
        public Nullable<int> sWater { get; set; }
        public Nullable<int> sPoint { get; set; }
    }
}
