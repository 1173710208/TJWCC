using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.MeterManage
{
    public class METER_RECORDEntity : IEntity<METER_RECORDEntity>
    {

        public long ID { get; set; }
        public string IMEI { get; set; }
        public DateTime RECORDTIME { get; set; }
        public decimal COUNT { get; set; }
        public DateTime SAVEDATE { get; set; }
        public int VTYPEID { get; set; }
        public string REMARK { get; set; }

    }
}
