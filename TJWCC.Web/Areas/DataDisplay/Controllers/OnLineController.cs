using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.DataDisplay;

namespace TJWCC.Web.Areas.DataDisplay.Controllers
{
    public class OnLineController : ControllerBase
    {
        Dis_WaterCompanyClassApp appMeter = new Dis_WaterCompanyClassApp();
        Dis_OffLineClassApp appOffline = new Dis_OffLineClassApp();

        /// <summary>
        /// 获取水务公司水表的总安装数量
        /// </summary>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetAllInstallAmount()
        {
            return Content(appMeter.GetAllInstallAmount().ToString());
        }

        /// <summary>
        /// 获取总的水表在线数量
        /// </summary>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetOnlineAmount()
        {
            return Content(appMeter.GetOnlineAmount().ToString());
        }

        /// <summary>
        /// 获取水务公司在线率
        /// </summary>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetOnlineRate()
        {
            return Content(appMeter.GetOnlineRate().ToString("N2"));
        }

        /// <summary>
        /// 获取水务公司二级公司的在线率
        /// </summary>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetBranchCompanyOnlineRate()
        {
            return Content(appMeter.GetBranchCompanyOnlineRate());
        }

        /// <summary>
        /// 获取水务公司三级公司的在线率
        /// </summary>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetAllSubCompanyOnlineRate()
        {
            return Content(appMeter.GetSubCompanyOnlineRate());
        }

        /// <summary>
        /// 按照二级水司名称获取水务公司三级公司的在线率
        /// </summary>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetSubCompanyOnlineRateByCName()
        {
            return Content(appMeter.GetInstallAndOnlineAmountByCname(Request["CName"]));
        }

        /// <summary>
        /// 离线天数区间对比
        /// </summary>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetOffLineInfo()
        {
            return Content(appOffline.GetOffLineInfo());
        }



    }
}