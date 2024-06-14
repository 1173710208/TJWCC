using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.DataDisplay
{
    /// <summary>
    /// 显示水表信息表
    /// </summary>
    public class METER_INFOEntity : IEntity<METER_INFOEntity>
    {
        public long NBMETERID { get; set; }
        public string IMEI { get; set; }
        public string TYPECODE { get; set; }
        public string PROTOCOLVER { get; set; }
        public string EQUIPID { get; set; }
        public int COMMID { get; set; }
        public int COMPANYID { get; set; }
        public int BUSINESSID { get; set; }
        public Nullable<int> SUBBUSIID { get; set; }
        public Nullable<long> USERID { get; set; }
        public string SBZCH { get; set; }
        public Nullable<int> TYPEID { get; set; }
        public Nullable<int> MAKERID { get; set; }
        public Nullable<System.DateTime> INSTALLDATE { get; set; }
        public Nullable<decimal> INICOUNT { get; set; }
        public Nullable<decimal> CURCOUNT { get; set; }
        public Nullable<System.DateTime> CURTIME { get; set; }
        public string ALARM { get; set; }
        public Nullable<decimal> VOLTAGE { get; set; }
        public Nullable<decimal> OVERFLOW { get; set; }
        public Nullable<int> OVERTIME { get; set; }
        public Nullable<decimal> OPPOFLOW { get; set; }
        public Nullable<int> OPPOTIME { get; set; }
        public string RALARMID { get; set; }
        public string SERVERIP { get; set; }
        public string SERVERPORT { get; set; }
        public string APN { get; set; }
        public string CYCSTIME { get; set; }
        public string CYCETIME { get; set; }
        public Nullable<int> TIMELENGTH { get; set; }
        public Nullable<int> CYCRATE { get; set; }
        public Nullable<int> RETRYTIMES { get; set; }
        public Nullable<decimal> VOLTALARM { get; set; }
        public Nullable<int> ISTARTHOUR { get; set; }
        public string VALVESTATUS { get; set; }
        public Nullable<int> RETRYCYCLE { get; set; }
        public Nullable<int> CYCSENDPERIOD { get; set; }
        public Nullable<int> ICYCSENDPERIOD { get; set; }
        public string SNKEY { get; set; }
        public Nullable<int> RSRP { get; set; }
        public Nullable<int> SNR { get; set; }
        public Nullable<int> COVLEVEL { get; set; }
        public string CELLID { get; set; }
        public string IMSI { get; set; }
        public string METERCODE { get; set; }
        public string MAKERCODE { get; set; }
        public Nullable<System.DateTime> TERMINALTIME { get; set; }
        public Nullable<int> TIMEDIFF { get; set; }
        public Nullable<int> RUNDAYS { get; set; }
        public string TVERSION { get; set; }
        public string PSK { get; set; }
        public Nullable<int> X { get; set; }
        public Nullable<int> Y { get; set; }
        public string ISACTIVE { get; set; }
        public Nullable<System.DateTime> CREATEDATE { get; set; }
        public string CREATERID { get; set; }
        public string ISSEC { get; set; }
        //public Nullable<bool> ISSEC { get; set; }

        public string REMARK { get; set; }
    }
}
