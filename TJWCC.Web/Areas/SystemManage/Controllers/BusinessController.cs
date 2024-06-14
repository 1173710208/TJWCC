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
    public class BusinessController : ControllerBase
    {
        private Business_InfoApp app = new Business_InfoApp();

        /// <summary>
        /// 查找营销分公司信息
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="imei"></param>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetBusinessInfoJson(Pagination pagination, string bName)
        {
            var data = new
            {
                rows = app.GetList(pagination, bName),
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
        public ActionResult GetFormJson(int bid)
        {
            var data = app.GetForm(bid);
            return Content(data.ToJson());
        }

        
        [HandlerAjaxOnly]
        public ActionResult GetGridJson()
        {
            int companyid = Convert.ToInt32(Request.Params["COMPANYID"]);
            var data = app.GetList(companyid);
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
        public ActionResult SubmitForm(Business_InfoEntity businessEntity, string keyValue)
        {
            app.SubmitForm(businessEntity, keyValue);
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
        public ActionResult DeleteForm(int bid)
        {
            app.DeleteForm(bid);
            return Success("删除成功。");
        }

    }
}