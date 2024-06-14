using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.Public
{

    /// <summary>
    /// 用户缴费记录显示
    /// </summary>
    public class DisFeeRecord
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public decimal Amount { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreaterId { get; set; }
        public string Remark { get; set; }
        public string UserName { get; set; }

    }
}
