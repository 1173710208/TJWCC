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
    public class MeterTypeController : ControllerBase
    {
        private MeterType_InfoApp app = new MeterType_InfoApp();
        /// <summary>
        /// 查找水表类型
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="imei"></param>
        /// <returns></returns>
        
        [HandlerAjaxOnly]
        public ActionResult GetMeterTypeInfoJson(Pagination pagination, string meterTypeName)
        {
            var data = new
            {
                rows = app.GetList(pagination, meterTypeName),
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
        public ActionResult GetFormJson(int TypeID)
        {
            var data = app.GetForm(TypeID);
            return Content(data.ToJson());
        }

        
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(string keyword)
        {
            var data = app.GetList();
            return Content(data.ToJson());
        }

        /// <summary>
        /// 添加水表类型
        /// </summary>
        /// <param name="METERTYPE_INFOEntity">水表类型实体</param> 
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(METERTYPE_INFOEntity MakerEntity, string keyValue)
        {
            app.SubmitForm(MakerEntity, keyValue);
            return Success("操作成功。");
        }
        /// <summary>
        /// 删除水表类型
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(int TypeID)
        {
            app.DeleteForm(TypeID);
            return Success("删除成功。");
        }
    }
}