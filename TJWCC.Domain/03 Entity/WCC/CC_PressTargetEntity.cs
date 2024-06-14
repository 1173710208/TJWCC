using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.WCC
{
    public class CC_PressTargetEntity : IEntity<CC_PressTargetEntity>
    {
        public long ID { get; set; }
        public string Station_key { get; set; }
        public TimeSpan? PlanTime { get; set; }
        public decimal? Value { get; set; }
        public int? Type { get; set; }
        public bool? Selected { get; set; }
    }
}
