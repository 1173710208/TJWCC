using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class DMAAreaClassEntity : IEntity<DMAAreaClassEntity>
    {
        public int ID { get; set; }
        public string DMAArea { get; set; }
        public Nullable<int> DMAID { get; set; }
    }
}
