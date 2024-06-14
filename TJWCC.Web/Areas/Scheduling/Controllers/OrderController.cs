using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.WCC;
using TJWCC.Code;
using TJWCC.Domain.Entity.WCC;

namespace TJWCC.Web.Areas.Scheduling.Controllers
{
    public class OrderController : ControllerBase
    {
        private CC_DispatchPlanApp dpApp = new CC_DispatchPlanApp();
        // 调度指令: Scheduling/Order

        /// <summary>
        /// 根据条件获取调度日志数据
        /// </summary>
        ///<param name="sDate">开始时间</param>
        ///<param name="eDate">结束时间</param>
        /// <returns></returns>

        public ActionResult GetDispatchPlan(Pagination pagination, DateTime? sdDate, DateTime? edDate, DateTime? seDate, DateTime? eeDate, int? dtype, string status, int? isPlan)
        {
            var data = new
            {
                rows = dpApp.GetOrderList(pagination, sdDate, edDate, seDate, eeDate, dtype, status, isPlan),
                pagination.total,
                pagination.page,
                pagination.records
            };
            return Content(data.ToJson());
        }
        
        public ActionResult GetDownloadJson(DateTime? sdDate, DateTime? edDate, DateTime? seDate, DateTime? eeDate, int? dtype, string status, int? isPlan)
        {
            string path = dpApp.GetOrderDownload(sdDate, edDate, dtype, seDate, eeDate, status, isPlan);
            //StringBuilder sbScript = new StringBuilder();
            //sbScript.Append("<script type='text/javascript'>$.loading(false);</script>");
            return Content("http://" + Request.Url.Host + ":" + Request.Url.Port + path);
        }

    }
}