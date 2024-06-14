using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BSB_ForecastFlowEntity : IEntity<BSB_ForecastFlowEntity>
    {
        public long id { get; set; }
        public System.DateTime CreateTime { get; set; }
        public System.DateTime FlowTime { get; set; }
        public decimal ForecastFlow { get; set; }
        public Nullable<int> MoreAlert { get; set; }
        public Nullable<int> AlertClass { get; set; }
        public Nullable<int> ContinueDays { get; set; }
        public Nullable<decimal> Leakage { get; set; }
        public Nullable<int> DMA { get; set; }
        public string DMAGrade { get; set; }
        public string Remark { get; set; }
    }
}
