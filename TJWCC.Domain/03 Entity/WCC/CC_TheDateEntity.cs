using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.WCC
{
    public class CC_TheDateEntity : IEntity<CC_TheDateEntity>
    {
        public int ID { get; set; }
        public DateTime TheDate { get; set; }
        public int Type { get; set; }
    }
}
