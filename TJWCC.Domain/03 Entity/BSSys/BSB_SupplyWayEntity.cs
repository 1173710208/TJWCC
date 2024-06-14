using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BSB_SupplyWayEntity : IEntity<BSB_SupplyWayEntity>
    {
        public long ElementId { get; set; }
        public int ElementType { get; set; }
        public bool Is_Active { get; set; }
        public bool IsUpdate { get; set; }
        public string SupplyWayNode { get; set; }
        public string SupplyWayPipe { get; set; }
    }
}
