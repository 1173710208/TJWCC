using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class DistrictClassEntity : IEntity<DistrictClassEntity>
    {
        public int ID { get; set; }
        public string District { get; set; }
        public Nullable<int> ZoneID { get; set; }
    }
}
