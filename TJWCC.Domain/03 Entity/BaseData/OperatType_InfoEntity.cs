using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BaseData
{
    public class OperatType_InfoEntity : IEntity<OperatType_InfoEntity>
    {
        public int OTYPEID { get; set; }
        public string OTYPENAME { get; set; }
        public string FIXEDCONTENT { get; set; }
        public string REMARK { get; set; }
    }
}
