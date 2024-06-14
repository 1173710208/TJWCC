using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.DataDisplay;

namespace TJWCC.Web.Areas.DataDisplay.Controllers
{
    public class WaterController : ControllerBase
    {

        private Dis_WaterApp app = new Dis_WaterApp();


        /// <summary>
        /// 获取水务公司总用水量
        /// </summary>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetTotalWater()
        {
            return Content(app.GetAllWater().ToString("N2"));
        }

        /// <summary>
        /// 获取水务公司24小时用水量曲线数据
        /// </summary>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetHourWaterTJ()
        {
            return Content(app.GetHourWaterTJ());
        }

        /// <summary>
        /// 获取二级分公司24小时用水量曲线数据
        /// </summary>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetSecondaryHourWater()
        {
            return Content(app.GetHourWaterEJGS());
        }

        /// <summary>
        /// 获取三级分公司24小时用水量曲线数据
        /// </summary>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetThreeLevelHourWater()
        {
            return Content(app.GetHourWaterSJGS());
        }

        /// <summary>
        /// 获取用水比例图表
        /// 该图表为下钻式图表
        /// </summary>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetWaterRation()
        {
            return Content(app.GetWaterRation());
        }


    }
}