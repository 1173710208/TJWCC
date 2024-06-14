using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.MeterManage
{
    public class METERDAY_RECORDEntity : IEntity<METERDAY_RECORDEntity>
    {
        public long ID { get; set; }
        public string IMEI { get; set; }
        public int? RSRP { get; set; }
        public int? SNR { get; set; }
        public int? COVLEVEL { get; set; }
        public string CELLID { get; set; }
        public DateTime TFTIME { get; set; }
        public decimal? TFLOW { get; set; }
        public decimal? DFTFLOW { get; set; }
        public decimal? DOTFLOW { get; set; }
        public DateTime REPORTTIME { get; set; }
        public decimal? DAYTOPFLOW { get; set; }
        public DateTime? DTFTIME { get; set; }
        public decimal? VOLTAGE { get; set; }
        public int? SUCCCOUNT { get; set; }
        public int? FAILCOUNT { get; set; }
        public string VALVESTATE { get; set; } 
        public decimal? NETCOUNT { get; set; }
        public DateTime SAVEDATE { get; set; }
        public string REMARK { get; set; }
    }
}
