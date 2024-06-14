using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.WCC
{
    public class CC_DispatchPlanEntity : IEntity<CC_DispatchPlanEntity>
    {
        public long ID { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExecDate { get; set; }
        public DateTime? DataDate { get; set; }
        public int? Shift { get; set; }
        public string Operator { get; set; }
        public int? DispatchType { get; set; }
        public decimal? Deviation { get; set; }
        public string Status { get; set; }
        public string AccordingTo { get; set; }
        public string DispatchOrder { get; set; }
        public string Effect { get; set; }
        public string LinkedMeters { get; set; }
        public string PlanFile { get; set; }
        public int? IsPlanBase { get; set; }
        public string Remark { get; set; }
    }
}
