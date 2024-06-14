using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.MeterManage
{
    public class ALARM_RAW_RECORDEntity : IEntity<ALARM_RAW_RECORDEntity>
    {
        public long ID { get; set; }
        public string IMEI { get; set; }
        public DateTime? ALARMTIME { get; set; }
        public decimal? ALARMVALUE { get; set; }
        public int ATYPEID { get; set; }
        public DateTime SAVEDATE { get; set; }
        public string REMARK { get; set; }
    }
}
