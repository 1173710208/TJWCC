using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.DataDisplay
{
    /// <summary>
    /// 显示用按信号强度分类的水表数量信息表
    /// </summary>
    public class DIS_SIGNALSTRENGTHEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        ///  信号强度ID
        /// </summary>
        public int SIGNALID { get; set; }

        /// <summary>
        /// 信号强度名称
        /// </summary>
        public string SIGNALNAME { get; set; }

        /// <summary>
        /// 信号大于等于某值
        /// </summary>
        public int GREATER { get; set; }

        /// <summary>
        /// 信号小于某值
        /// </summary>
        public int LESS { get; set; }

        /// <summary>
        /// 该信号范围内水表数量
        /// </summary>
        public int AMOUNT { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string REMARK { get; set; }

    }

}
