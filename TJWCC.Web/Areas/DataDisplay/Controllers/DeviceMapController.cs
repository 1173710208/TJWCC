using System;
using System.Collections.Generic;
using TJWCC.Code;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.DataDisplay;
using TJWCC.Domain.Entity.DataDisplay;

namespace TJWCC.Web.Areas.DataDisplay.Controllers
{
    public class DeviceMapController : ControllerBase
    {
        private Meter_InfoApp app = new Meter_InfoApp();
        //private User_InfoApp uapp = new User_InfoApp();

        
        [HandlerAjaxOnly]
        public ActionResult GetWaterMeter()
        {
            return Content(app.GetSingleMeter(Request["mid"]));
        }
        public ActionResult GetWaterMeterlist()
        {
            return Content(app.GetMeterList());
        }
        #region 修改坐标
        public ActionResult DMEditForm()
        {
            return View();
        }
        #endregion
        
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(decimal keyValue)
        {
            var data = app.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult SubmitForm(double xx, double yy, decimal keyValue)
        {
            METER_INFOEntity meter_infoEntity = app.GetForm(keyValue);
            var x = xx * 20037508.34 / 180;
            var y = Math.Log(Math.Tan((90 + yy) * Math.PI / 360)) / (Math.PI / 180);
            y = y * 20037508.34 / 180;
            meter_infoEntity.X = Convert.ToInt32(x);
            meter_infoEntity.Y = Convert.ToInt32(y);
            app.SubmitForm(meter_infoEntity);
            return Success("操作成功。");
        }
    }
}