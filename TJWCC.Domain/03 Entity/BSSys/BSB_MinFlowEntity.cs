using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BSB_MinFlowEntity : IEntity<BSB_MinFlowEntity>
    {
        public long id { get; set; }
        public System.DateTime CreateTime { get; set; }
        public System.DateTime FlowTime { get; set; }
        public decimal MinFlow { get; set; }
        public int OnceAlert { get; set; }
        public int MoreAlert { get; set; }
        public int AlertClass { get; set; }
        public int ContinueDays { get; set; }
        public Nullable<decimal> Leakage { get; set; }
        public Nullable<int> DMA { get; set; }
        public string DMAGrade { get; set; }
        public string Remark { get; set; }
    }
}
