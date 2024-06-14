using TJWCC.Application.SystemSecurity;
using TJWCC.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TJWCC.Web.Areas.SystemSecurity.Controllers
{
    public class LogController : ControllerBase
    {
        private LogApp logApp = new LogApp();

        
        public ActionResult RemoveLog()
        {
            return View();
        }
        
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson)
        {
            var data = new
            {
                rows = logApp.GetList(pagination, queryJson),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitRemoveLog(string keepTime)
        {
            logApp.RemoveLog(keepTime);
            return Success("清空成功。");
        }
    }
}
