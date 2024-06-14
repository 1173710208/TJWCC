using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.WCC
{
    public class GY_TypeInfoEntity : IEntity<GY_TypeInfoEntity>
    {
        public long TypeId { get; set; }
        public string TypeName { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public string Remark { get; set; }

    }
}
