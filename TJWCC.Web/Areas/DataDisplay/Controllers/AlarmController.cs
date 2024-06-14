using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.DataDisplay;

namespace TJWCC.Web.Areas.DataDisplay.Controllers
{
    public class AlarmController : ControllerBase
    {

        Dis_AlarmApp appAlarm = new Dis_AlarmApp();


        /// <summary>
        /// 获取水务公司6中分类报警
        /// </summary>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetSubCompanyOnlineRate()
        {
            return Content(appAlarm.GetAlarmType());
        }

        /// <summary>
        /// 获取二级公司报警分类数量
        /// </summary>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetSubAlarmType()
        {
            return Content(appAlarm.GetSubAlarmType());
        }

        /// <summary>
        /// 获取水务公司总报警条数
        /// </summary>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetAllAlarmCount()
        {
            return Content(appAlarm.GetAllAlarmCount().ToString());
        }
    }
}