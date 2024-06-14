using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class DMAClassEntity : IEntity<DMAClassEntity>
    {
        public int ID { get; set; }
        public string DMA { get; set; }
        public Nullable<int> DistrictID { get; set; }
        public Nullable<int> ZoneID { get; set; }
    }
}
