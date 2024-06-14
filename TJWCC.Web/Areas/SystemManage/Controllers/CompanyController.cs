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
    public class CompanyController : ControllerBase
    {
        private Company_InfoApp app = new Company_InfoApp();

        /// <summary>
        /// 查找二级公司信息
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="imei"></param>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetComapnyInfoJson(Pagination pagination, string cName)
        {
            var data = new
            {
                rows = app.GetList(pagination, cName),
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
        public ActionResult GetFormJson(int cid)
        {
            var data = app.GetForm(cid);
            return Content(data.ToJson());
        }
        
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(string keyword)
        {
            var data = app.GetList();
            return Content(data.ToJson());
        }
        /// <summary>
        /// 添加二级公司
        /// </summary>
        /// <param name="roleEntity"></param>
        /// <param name="permissionIds"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(Company_InfoEntity companyEntity, string keyValue)
        {
            app.SubmitForm(companyEntity, keyValue);
            return Success("操作成功。");
        }
        /// <summary>
        /// 删除二级公司
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(int cid)
        {
            app.DeleteForm(cid);
            return Success("删除成功。");
        }

    }
}