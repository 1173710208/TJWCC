using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.WCC
{
    public class CC_WetherRecordEntity : IEntity<CC_WetherRecordEntity>
    {
        public long ID { get; set; }
        public DateTime? PreDate { get; set; }
        public int Type { get; set; }
        public string Weather { get; set; }
        public int Temp { get; set; }
        public int? TempL { get; set; }
        public int Wind { get; set; }
        public string WindDir { get; set; }
        public string Air { get; set; }
        public string Remark { get; set; }
    }
}
