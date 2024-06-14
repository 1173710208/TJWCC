using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.BaseData
{
    public class METERTYPE_INFOEntity : IEntity<METERTYPE_INFOEntity>
    {
        public int TYPEID { get; set; }
        public string TYPENAME { get; set; }
        public int? MAKERID { get; set; }
        public int? DIAMETER { get; set; }
        public string MATERIAL { get; set; }
        public string GRADE { get; set; }
        public double? OVERFLOW { get; set; }
        public int? OVERTIME { get; set; }
        public double? OPPOFLOW { get; set; }
        public int? OPPOTIME { get; set; }
        public string RALARMID { get; set; }
        public string SERVERIP { get; set; }
        public string SERVERPORT { get; set; }
        public string APN { get; set; }
        public string CYCSTIME { get; set; }
        public string CYCETIME { get; set; }
        public int? TIMELENGTH { get; set; }
        public int? CYCRATE { get; set; }
        public int? RETRYTIMES { get; set; }
        public int? VOLTALARM { get; set; }
        public int? ISTARTHOUR { get; set; }
        public string VALVESTATUS { get; set; }
        public int? RETRYCYCLE { get; set; }
        public int? CYCSENDPERIOD { get; set; }
        public int? ICYCSENDPERIOD { get; set; }
        public string SNKEY { get; set; }
        public string REMARK { get; set; }

    }
}
