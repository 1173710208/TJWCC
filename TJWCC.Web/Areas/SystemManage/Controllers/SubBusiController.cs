using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.SystemManage;
using TJWCC.Code;
using TJWCC.Domain.Entity.SystemManage;

namespace TJWCC.Web.Areas.SystemManage.Controllers
{
    public class SubBusiController : ControllerBase
    {
        private SubBusi_InfoApp app = new SubBusi_InfoApp();

        /// <summary>
        /// 查找营销分公司信息
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="imei"></param>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetSubBusiInfoJson(Pagination pagination, string sName)
        {
            var data = new
            {
                rows = app.GetList(pagination, sName),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 获取Form结果的JSON
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(int sid)
        {
            var data = app.GetForm(sid);
            return Content(data.ToJson());
        }

        
        [HandlerAjaxOnly]
        public ActionResult GetGridJson()
        {
            int businessid = Convert.ToInt32(Request.Params["BUSINESSID"]);
            var data = app.GetList(businessid);
            return Content(data.ToJson());
        }

        /// <summary>
        /// 添加营销分公司
        /// </summary>
        /// <param name="roleEntity"></param>
        /// <param name="permissionIds"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(SubBusi_InfoEntity subBusiEntity, string keyValue)
        {
            app.SubmitForm(subBusiEntity, keyValue);
            return Success("操作成功。");
        }
        /// <summary>
        /// 删除营销分公司
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(int sid)
        {
            app.DeleteForm(sid);
            return Success("删除成功。");
        }

    }
}