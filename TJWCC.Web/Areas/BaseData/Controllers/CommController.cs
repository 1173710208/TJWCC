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
    public class CommController : ControllerBase
    {
        private Comm_InfoApp app = new Comm_InfoApp();
        /// <summary>
        /// 查找运营商信息
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="imei"></param>
        /// <returns></returns>
        [HandlerAjaxOnly]
        public ActionResult GetCommInfoJson(Pagination pagination, string commName)
        {
            var data = new
            {
                rows = app.GetList(pagination, commName),
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
        public ActionResult GetFormJson(int commID)
        {
            var data = app.GetForm(commID);
            return Content(data.ToJson());
        }

        
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(string keyword)
        {
            var data = app.GetList();
            return Content(data.ToJson());
        }
        /// <summary>
        /// 添加运营商
        /// </summary>
        /// <param name="roleEntity"></param>
        /// <param name="permissionIds"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(COMM_INFOEntity commEntity, string keyValue)
        {
            app.SubmitForm(commEntity, keyValue);
            return Success("操作成功。");
        }
        /// <summary>
        /// 删除运营商
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(int CommID)
        {
            app.DeleteForm(CommID);
            return Success("删除成功。");
        }

    }
}