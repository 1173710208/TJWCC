using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.Public
{

    /// <summary>
    /// 用户月用水量
    /// </summary>
    public class DisMonthRecord
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public decimal Count { get; set; }
        public decimal MonthSum { get; set; }
        public string YearMonth { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreaterId { get; set; }
        public string Remark { get; set; }
        public string UserName { get; set; }

    }
}
