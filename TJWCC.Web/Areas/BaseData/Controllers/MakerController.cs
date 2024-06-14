using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.BaseData;
using TJWCC.Code;
using TJWCC.Domain.Entity.BaseData;

namespace TJWCC.Web.Areas.BaseData.Controllers
{
    public class MakerController : ControllerBase
    {
        private MeterMaker_InfoApp app = new MeterMaker_InfoApp();
        /// <summary>
        /// 查找供应商信息
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="imei"></param>
        /// <returns></returns>
        [HandlerAjaxOnly]
        public ActionResult GetMakerInfoJson(Pagination pagination, string makerName)
        {
            var data = new
            {
                rows = app.GetList(pagination, makerName),
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
        public ActionResult GetFormJson(int makerID)
        {
            var data = app.GetForm(makerID);
            return Content(data.ToJson());
        }

        
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(string keyword)
        {
            var data = app.GetList();
            return Content(data.ToJson());
        }

        /// <summary>
        /// 添加供应商
        /// </summary>
        /// <param name="METERMAKER_INFOEntity">供应商类型实体</param> 
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(METERMAKER_INFOEntity MakerEntity, string keyValue)
        {
            app.SubmitForm(MakerEntity, keyValue);
            return Success("操作成功。");
        }
        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(int MakerID)
        {
            app.DeleteForm(MakerID);
            return Success("删除成功。");
        }
    }
}