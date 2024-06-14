using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.DataDisplay
{
    /// <summary>
    /// 显示用按水表离线分类统计信息表
    /// </summary>
    public class DIS_OFFLINECLASSEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 离线天数分类ID
        /// </summary>
        public int OFFLINEID { get; set; }

        /// <summary>
        /// 离线天数分类名称
        /// </summary>
        public string OFFLINENAME { get; set; }

        /// <summary>
        /// 天数大于等于
        /// </summary>
        public int GREATER { get; set; }

        /// <summary>
        /// 天数小于
        /// </summary>
        public int LESS { get; set; }

        /// <summary>
        /// 该天数范围内的水表离线数量
        /// </summary>
        public int AMOUNT { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string REMARK { get; set; }
        
    }
}
