using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJWCC.Domain.Entity.DataDisplay
{

    /// <summary>
    /// 显示用按水务公司分类在线信息表
    /// </summary>
    public class DIS_WATERCOMPANYCLASSEntity
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
        /// 水务公司名称
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
