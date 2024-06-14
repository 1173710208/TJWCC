using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.Public
{

    /// <summary>
    /// 用户缴费临时数据
    /// </summary>
    public class UserFeeTemp
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }

    }
}
