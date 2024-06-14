using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BSSys
{
    public class BSOT_CalDefaultInfoEntity : IEntity<BSOT_CalDefaultInfoEntity>
    {
        public int Duration { get; set; }
        public Nullable<System.DateTime> FirstDate { get; set; }
        public Nullable<System.DateTime> CurrentDate { get; set; }
        public Nullable<int> Counter { get; set; }
        public Nullable<int> IsNetUpdate { get; set; }
        public Nullable<System.DateTime> GisEndTime { get; set; }
        public Nullable<System.DateTime> JllEndTime { get; set; }
        public Nullable<int> calcuStep { get; set; }
        public Nullable<int> inpFinish { get; set; }
    }
}
