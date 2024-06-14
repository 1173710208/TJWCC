using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.DataDisplay
{
    /// <summary>
    /// 显示用按品牌分类在线信息表
    /// </summary>
    public class DIS_MAKERCLASSEntity : IEntity<DIS_MAKERCLASSEntity>
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 水表生产商ID
        /// </summary>
        public int MAKERID { get; set; }

        /// <summary>
        /// 生产商名称
        /// </summary>
        public string MNAME { get; set; }

        /// <summary>
        /// 安装数量
        /// </summary>
        public int INSTALLAMOUNT { get; set; }

        /// <summary>
        /// 在线数量
        /// </summary>
        public int ONLINEAMOUNT { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string REMARK { get; set; }

    }
}
