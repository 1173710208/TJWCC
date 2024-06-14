using TJWCC.Application.SystemManage;
using TJWCC.Code;
using TJWCC.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TJWCC.Web.Areas.SystemManage.Controllers
{
    public class DutyController : ControllerBase
    {
        private DutyApp dutyApp = new DutyApp();

        
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(string keyword)
        {
            var data = dutyApp.GetList(keyword);
            return Content(data.ToJson());
        }
        
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = dutyApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(RoleEntity roleEntity, string keyValue)
        {
            dutyApp.SubmitForm(roleEntity, keyValue);
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            dutyApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
    }
}
