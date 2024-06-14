using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BSM_WaterMeter_RecordEntity : IEntity<BSM_WaterMeter_RecordEntity>
    {
        public long ID { get; set; }
        public Nullable<long> WMeter_ID { get; set; }
        public Nullable<bool> Reissue { get; set; }
        public Nullable<System.DateTime> Sdate { get; set; }
        public Nullable<int> Stime { get; set; }
        public Nullable<long> Ss { get; set; }
        public Nullable<long> Zx { get; set; }
        public Nullable<long> Fx { get; set; }
        public Nullable<long> Zxcz { get; set; }
        public Nullable<long> Fxcz { get; set; }
        public Nullable<long> Jll { get; set; }
        public string Cutyn { get; set; }
        public Nullable<int> Hourbj { get; set; }
        public Nullable<long> Ddgz { get; set; }
        public Nullable<int> Day1 { get; set; }
        public Nullable<int> Day7 { get; set; }
        public Nullable<int> Day30 { get; set; }
        public Nullable<int> Day180 { get; set; }
        public string Remark { get; set; }
    }
}
