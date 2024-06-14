using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BaseData
{
    public class METERMAKER_INFOEntity : IEntity<METERMAKER_INFOEntity>
    {
        public int MAKERID { get; set; }
        public string MNAME { get; set; }
        public string ADDRESS { get; set; }
        public string CONTACT { get; set; }
        public string MOBILE { get; set; }
        public string REMARK { get; set; }
    }
}
