using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.DataDisplay
{
    /// <summary>
    /// 显示用24小时每小时的用水量信息表
    /// </summary>
    public class DIS_WATEREntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 水务公司ID
        /// </summary>
        public int COMPANYID { get; set; }

        /// <summary>
        ///水务公司名称
        /// </summary>
        public string CNAME { get; set; }

        /// <summary>
        /// 营销公司ID
        /// </summary>
        public int BUSINESSID { get; set; }

        /// <summary>
        /// 营销公司名称
        /// </summary>
        public string BNAME { get; set; }

        /// <summary>
        /// 小时
        /// </summary>
        public int HOUR { get; set; }

        /// <summary>
        /// 该一小时内的用水量
        /// </summary>
        public int AMOUNT { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string REMARK { get; set; }

    }


}
