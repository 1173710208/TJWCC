using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.ViewModel
{
    public partial class BsTagHisResult
    {
        public long ID { get; set; }
        public decimal? Tag_value { get; set; }
        public DateTime? Save_date { get; set; }
        public string Station_key { get; set; }
        public string Meter_Name { get; set; }
        public string Tag_key { get; set; }
        public decimal? CleanedValue { get; set; }
        public decimal? ModifiedValue { get; set; }
    }
}
