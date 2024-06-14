using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.WCC
{
    public class JUNCTION_VICEEntity : IEntity<JUNCTION_VICEEntity>
    {
        public int OBJECTID_1 { get; set; }
        public decimal ELEMENTID { get; set; }
        public string OBJECTID { get; set; }
        public string LABEL { get; set; }
        public System.Data.Entity.Spatial.DbGeometry Shape { get; set; }
    }
}
