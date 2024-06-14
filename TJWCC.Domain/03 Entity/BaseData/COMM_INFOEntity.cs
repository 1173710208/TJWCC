using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BaseData
{
    public class COMM_INFOEntity : IEntity<COMM_INFOEntity>
    {
        public int COMMID { get; set; }
        public string COMMNAME { get; set; }
        public string SERVERIP { get; set; }
        public string SERVERPORT { get; set; }
        public string ADDRESS { get; set; }
        public string CONTACT { get; set; }
        public string MOBILE { get; set; }
        public string REMARK { get; set; }

}
}
