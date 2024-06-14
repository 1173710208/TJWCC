using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Code;
using TJWCC.Application.BaseData;
using TJWCC.Domain.Entity.BaseData;

namespace TJWCC.Web.Areas.BaseData.Controllers
{
    public class OperatTypeController : ControllerBase
    {
        private OperatType_InfoApp app = new OperatType_InfoApp();

        
        [HandlerAjaxOnly]
        public ActionResult GetGridJson()
        {
            var data = app.GetList();
            return Content(data.ToJson());
        }

        
        [HandlerAjaxOnly]
        public ActionResult GetGridJsonRemark()
        {
            string remark = Request.Params["REMARK"];
            var data = app.GetList(remark);
            return Content(data.ToJson());
        }

        /// <summary>
        /// 添加查询指令类型
        /// </summary>
        /// <param name="roleEntity"></param>
        /// <param name="permissionIds"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(OperatType_InfoEntity operatType_InfoEntity, string keyValue)
        {
            app.SubmitForm(operatType_InfoEntity, keyValue);
            return Success("操作成功。");
        }
        /// <summary>
        /// 删除查询指令类型
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(int oTypeID)
        {
            app.DeleteForm(oTypeID);
            return Success("删除成功。");
        }

    }
}