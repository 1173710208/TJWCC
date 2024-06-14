using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using NBIOT.Code;
using NBIOT.Application.MeterManage;

namespace NBIOT.Web.Areas.MeterManage.Controllers
{
    public class MeterController : ControllerBase
    {
        private Meter_InfoApp meterInfoApp = new Meter_InfoApp();
        private Alarm_RAW_RecordApp alarmRecordApp = new Alarm_RAW_RecordApp();
        private MeterDay_RecordApp meterDayRecordApp = new MeterDay_RecordApp();
        private Meter_RecordApp meterRecordApp = new Meter_RecordApp();

        //private NASDK currsdk = new NASDK();
        private string token = new NASDK().getToken().accessToken;
        
        #region 水表档案

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetMeterInfoJson(Pagination pagination, string imei)
        {
            var data = new
            {
                rows = meterInfoApp.GetList(pagination, imei),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        #region 新增水表
        public ActionResult MeterForm()
        {
            return View();
        }
        #endregion
        #endregion

        #region 报警记录
        /// <summary>
        /// 报警记录信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Alarm()
        {
            return View();
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAlarmRecordJson(Pagination pagination, string imei)
        {
            var data = new
            {

                rows = alarmRecordApp.GetList(pagination, imei),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        #endregion

        #region 日水量记录
        /// <summary>
        /// 日水表记录信息
        /// </summary>
        /// <returns></returns>
        public ActionResult WaterDay()
        {
            return View();
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetMeterDayRecordJson(Pagination pagination, string imei)
        {
            var data = new
            {

                rows = meterDayRecordApp.GetList(pagination, imei),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }



        #endregion

        #region 水表周期记录

        /// <summary>
        /// 水表记录信息
        /// </summary>
        /// <returns></returns>
        public ActionResult WaterPeriod()
        {
            return View();
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetMeterRecordJson(Pagination pagination, string imei)
        {
            var data = new
            {

                rows = meterRecordApp.GetList(pagination, imei),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        #endregion

        #region 指令下发
        public ActionResult CommForm()
        {
            return View();
        }


        /// <summary>
        /// 指令下发
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult GetSendComm()
        {

            string imei = Request.Form["IMEI"];
            //string strPostdata = "{'msg': {'at': 1550566806931,'imei': '869976030006200','dev_id': '515845581'," +
            //       " 'controlCode': '03','tag': '03','value': [{'ID': 1,'Value': '5000'}, {'ID': 2,'Value': '1'" +
            //       "}, {'ID': 3,'Value': '64'}, {'ID': 4,'Value': '2'}],'isAes': true,'cmd_id': '5399'},	" +
            //       "'nonce': 'SMtOM!Os',	'signature': 'wiAffyWRtsqq+3zQa2qUqg=='}";

            TimeSpan ts = DateTime.Now - Convert.ToDateTime("1970-01-01");
            string result = "";
            for (int i = 0; i < 4; i++)
            {

                string msg = "{\"at\":\"" + ts.Ticks + "\",\"imei\":\"00000000000000" + i + 1 + "\",\"dev_id\":\"515845581\"," +
                            "\"controlCode\":\"30\",\"tag\":\"03\",\"value\":[{\"ID\":1,\"Value\":\"5000\"},{\"ID\":2,\"Value\":\"1\"" +
                            "},{\"ID\":3,\"Value\":\"64\"},{\"ID\":4,\"Value\":\"2\"}],\"isAes\":true,\"cmd_id\":\"1\"}";

                string signature = Signature.GenerateSignature("6543210987654321", "SMtOM!Os", msg);

                string strPostdata = "{\"msg\":{\"at\":\"" + ts.Ticks + "\",\"imei\":\"00000000000000" + i + 1 + "\",\"dev_id\":\"515845581\"," +
                            "\"controlCode\":\"30\",\"tag\":\"03\",\"value\":[{\"ID\":1,\"Value\":\"5000\"},{\"ID\":2,\"Value\":\"1\"" +
                            "},{\"ID\":3,\"Value\":\"64\"},{\"ID\":4,\"Value\":\"2\"}],\"isAes\":true,\"cmd_id\":\"1\"}," +
                            "\"nonce\":\"SMtOM!Os\",	\"signature\":\"" + signature + "\"}";

                result = HttpMethods.HttpPost("http://47.105.136.33:9090", strPostdata);

                result += " 设备" + i + "注册成功";
                System.Threading.Thread.Sleep(1000);
            }

            return Info("  结果：" + result);


        }
        #endregion

    }
}