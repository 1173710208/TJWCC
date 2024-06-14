using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.WCC
{
    public class CC_FlowSortEntity : IEntity<CC_FlowSortEntity>
    {
        public int ID { get; set; }
        public DateTime? Save_date { get; set; }
        public int? Sort_key { get; set; }
        public decimal? Sort_value{ get; set; }
        public int? Type { get; set; }
        public string Remark{ get; set; }
    }
}
