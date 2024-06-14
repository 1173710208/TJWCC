using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TJWCC.Code;
using TJWCC.Application.MeterManage;
using TJWCC.Domain.Entity.DataDisplay;
using TJWCC.Domain.Entity.BaseData;

namespace TJWCC.Web.Areas.MeterManage.Controllers
{
    public class MeterController : ControllerBase
    {
        private Meter_InfoApp meterInfoApp = new Meter_InfoApp();
        private Alarm_RAW_RecordApp alarmRecordApp = new Alarm_RAW_RecordApp();
        private MeterDay_RecordApp meterDayRecordApp = new MeterDay_RecordApp();
        private Meter_RecordApp meterRecordApp = new Meter_RecordApp();

        //private NASDK currsdk = new NASDK();
        //private string token = new NASDK().getToken().accessToken;
        
        #region 水表档案

        
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
        #endregion

        #region NB水表注册
        public ActionResult MeterForm()
        {
            return View();
        }

        /// <summary>
        /// NB水表注册、修改表信息、写入MeterInfo表中
        /// </summary>
        /// <param name="METER_INFOEntity">水表信息类型实体</param> 
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(METER_INFOEntity MeterEntity, string keyValue)
        {
            //string imei = Request.Form["IMEI"];
            MeterEntity.ISACTIVE = "1";

            NASDK currsdk = new NASDK();
            string token = currsdk.getToken().accessToken;
            string verifycode = MeterEntity.IMEI;
            string nodeid = MeterEntity.IMEI;
            //string enduserid = MeterEntity.IMSI;
            string enduserid = null;
            string psk = null;
            int timeout = 0;

            RegDeviceResult rdr = currsdk.regDevice(token, verifycode, nodeid, enduserid, psk, timeout);
            if (rdr == null)
            {
                return Info("新装水表注册失败！请重新注册。"); ;
            }
            else
            {
                MeterEntity.EQUIPID = rdr.deviceId;
                MeterEntity.PSK = rdr.psk;
                MeterEntity.IMEI = rdr.verifyCode;
            }

            string DeviceID = MeterEntity.EQUIPID;
            string model = "NB01";

            //完善水表平台信息
            string result = currsdk.modifyDevice(token, verifycode, DeviceID, model);
            if (result == null)
            {

                return Info("完善水表平台信息失败，请看日志");

            }
            //else
            //{
            //    this.txtModifyResult.Text = result;
            //}

            MeterEntity.CREATEDATE = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
            MeterEntity.CREATERID = OperatorProvider.Provider.GetCurrent().UserId;    
            meterInfoApp.SubmitForm(MeterEntity, keyValue);
            return Success("平台注册、数据库写入操作成功。");

        }


        ///// <summary>
        ///// NB水表注册、修改表信息、写入MeterInfo表中
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[HandlerAjaxOnly]
        //public ActionResult RegisteredMeter()
        //{
        //    string token = new NASDK().getToken().accessToken;
        //    string imei = Request.Form["IMEI"]; 
        //    string imei1 = Request.Form["FULLNAME"];

        //    //return Info("  结果：");
        //    return Success("操作成功。");
        //}

        #endregion

        #region 水表换表
        /// <summary>
        /// 水表换表
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangeMeterIndex()
        {
            return View();
        }
        public ActionResult ChangeMeterForm()
        {
            return View();
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeMeterSubmitForm(METER_INFOEntity MeterEntity, string keyValue)
        {
            string oldimei = Request.Form["OLDIMEI"];
            List<METER_INFOEntity> lmeterEntity = meterInfoApp.GetList(oldimei);
            if(lmeterEntity.Count==0)
            {
                return Info("旧水表IMEI不存在，请修改后重试！");
            }

            string deviceId = lmeterEntity[0].EQUIPID;
            MeterEntity.ISACTIVE = "1";

            NASDK currsdk = new NASDK();
            string token = currsdk.getToken().accessToken;
            string verifycode = MeterEntity.IMEI;
            string nodeid = MeterEntity.IMEI;
            string enduserid = MeterEntity.IMSI;
            string psk = null;
            int timeout = 0;

            //从Iot平台中删除旧水表
            string result = currsdk.deleteDevice(token, deviceId);
            if (result != "204")
            {
                return Info("从Iot平台中删除旧水表失败，请看日志");
            }
            meterInfoApp.DeleteForm(oldimei);   //从meterinfo表中删除旧水表

            RegDeviceResult rdr = currsdk.regDevice(token, verifycode, nodeid, enduserid, psk, timeout);
            if (rdr == null)
            {
                return Info("新水表注册失败！请重新注册。"); ;
            }
            else
            {
                MeterEntity.EQUIPID = rdr.deviceId;
                MeterEntity.PSK = rdr.psk;
                MeterEntity.IMEI = rdr.verifyCode;
            }

            string DeviceID = MeterEntity.EQUIPID;
            string model = "NB01";

            //完善水表平台信息
            string result1 = currsdk.modifyDevice(token, verifycode, DeviceID, model);
            if (result1 != "204")
            {
                return Info("完善水表平台信息失败，请看日志");
            }
            //else
            //{
            //    this.txtModifyResult.Text = result;
            //}

            MeterEntity.CREATEDATE = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
            MeterEntity.CREATERID = OperatorProvider.Provider.GetCurrent().UserId;
            meterInfoApp.SubmitForm(MeterEntity, keyValue);
            return Success("平台注册、数据库写入操作成功。");

        }
        #endregion

        #region 水表拆除
        /// <summary>
        /// 水表拆除
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteMeterIndex()
        {
            return View();
        }
        public ActionResult DeleteMeterForm()
        {
            return View();
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMeterSubmitForm(METER_INFOEntity MeterEntity, string keyValue)
        {
            string oldimei = Request.Form["OLDIMEI"];
            List<METER_INFOEntity> listmeterEntity = meterInfoApp.GetList(oldimei);
            if (listmeterEntity.Count == 0)
            {
                return Info("拆除的水表IMEI不存在，请修改后重试！");
            }

            string olddeviceId = listmeterEntity[0].EQUIPID;

            NASDK currsdk = new NASDK();
            string token = currsdk.getToken().accessToken;

            //从Iot平台中删除旧水表
            string result = currsdk.deleteDevice(token, olddeviceId);
            if (result != "204")
            {
                return Info("从Iot平台中删除旧水表失败，请看日志");
            }

            try
            {
                meterInfoApp.DeleteForm(oldimei);   //从meterinfo表中删除旧水表
            }
            catch (Exception)
            {
                return Info("水表从数据库删除时失败！");
            }

            return Success("水表从IoT平台和数据库中删除成功。");

        }
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
         public ActionResult CommandIndex()
        {
            return View();
        }

        public ActionResult QueryCommandForm()
        {
            return View();
        }

        public ActionResult SetCommandForm()
        {
            return View();
        }

        /// <summary>
        /// 指令下发
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult QueryCommandSubmitForm(OperatType_InfoEntity otiEntity)
        {
            //命令JSON体
            //{
            //    "deviceId": "beef1e38-2cef-4514-a8c4-13041438f6c2",
            //    "command": {
            //       "serviceId": "ServiceMessage",
            //    "method": "SET_DEVICE",
            //    "paras": {
            //            "c1":"68011904290911220300000900050B0002000100000A16"
            //        }
            //    },
            //    "callbackUrl": "",
            //    "expireTime":86400,
            //    "maxRetransmit":1
            //}

            NASDK currsdk = new NASDK();
            string token = currsdk.getToken().accessToken;
            string imei = Request.Form["IMEI"];
            string commandCode = otiEntity.FIXEDCONTENT;
            List<METER_INFOEntity> listmeterEntity = meterInfoApp.GetList(imei);
            if (listmeterEntity.Count == 0)
            {
                return Info("要查询的水表IMEI不存在，请修改后重试！");
            }
            string deviceId = listmeterEntity[0].EQUIPID;

            List<CommandPara> lsCmdPars = new List<CommandPara>();

            CommandPara currCmdPara = new CommandPara();
            currCmdPara.isNum = false;
            currCmdPara.paraName = "c1";

            //--查询命令
            //68011904290911220300000900050B0002000100000A16
            //取得命令字符串及校验码
            string paraValue1 = "6801" + DateTime.Now.ToString("yyMMddHHmmss") + "0300000900050B0002" + commandCode + "0000";
            Byte[] bytesParaValue1 = AESUtil.HexStringToByte(paraValue1);
            string checkCode = AESUtil.GetCheckCode(bytesParaValue1);   //二进制算术累加的校验码
            currCmdPara.paraValue = paraValue1 + "" + checkCode + "16";
            lsCmdPars.Add(currCmdPara);

            string result = currsdk.sendCommand(token, deviceId, lsCmdPars);
            if (result != "201")
            {
                return Info("查询命令发送到IoT平台失败，请看日志。");
            }
            return Success("查询命令已成功发送到IoT平台。");
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SetCommandSubmitForm(METER_INFOEntity meter_InfoEntity)
        {
            NASDK currsdk = new NASDK();
            string token = currsdk.getToken().accessToken;
            string imei = Request.Form["IMEI"];
            //string commandCode = otiEntity.FIXEDCONTENT;
            List<METER_INFOEntity> listmeterEntity = meterInfoApp.GetList(imei);
            if (listmeterEntity.Count == 0)
            {
                return Info("要设置的水表IMEI不存在，请修改后重试！");
            }
            string deviceId = listmeterEntity[0].EQUIPID;

            List<CommandPara> lsCmdPars = new List<CommandPara>();

            CommandPara currCmdPara = new CommandPara();
            currCmdPara.isNum = false;
            currCmdPara.paraName = "c1";

            //--查询命令
            //68011904290911220300000900050B0002000100000A16
            //取得TLV（tag、length、value）
            string strtlv ="";
            string strtemp = "";
            if (meter_InfoEntity.OVERFLOW!=null)
            {
                strtemp = string.Format("{0:X00}", meter_InfoEntity.OVERFLOW.ToInt()).PadLeft(8, '0');
                if (strtemp.Length==8)
                {
                    strtlv += "00"+ strtemp;
                }else
                {
                    return Info("输入参数有误，请重新输入！");
                }
            }
            if(meter_InfoEntity.OVERTIME != null)
            {
                strtemp = string.Format("{0:X00}", meter_InfoEntity.OVERTIME.ToInt()).PadLeft(2, '0');
                if (strtemp.Length == 2)
                {
                    strtlv += "01" + strtemp;
                }
                else
                {
                    return Info("输入参数有误，请重新输入！");
                }
            }
            if (meter_InfoEntity.OPPOFLOW != null)
            {
                strtemp = string.Format("{0:X00}", meter_InfoEntity.OPPOFLOW.ToInt()).PadLeft(8, '0');
                if (strtemp.Length == 8)
                {
                    strtlv += "02" + strtemp;
                }
                else
                {
                    return Info("输入参数有误，请重新输入！");
                }
            }
            if (meter_InfoEntity.OPPOTIME != null)
            {
                strtemp = string.Format("{0:X00}", meter_InfoEntity.OPPOTIME.ToInt()).PadLeft(2, '0');
                if (strtemp.Length == 2)
                {
                    strtlv += "03" + strtemp;
                }
                else
                {
                    return Info("输入参数有误，请重新输入！");
                }
            }
            if (meter_InfoEntity.CYCSTIME != null)
            {
                strtemp = meter_InfoEntity.CYCSTIME.PadLeft(6, '0');
                if (strtemp.Length == 6)
                {
                    strtlv += "07" + "000101" + strtemp;
                }
                else
                {
                    return Info("输入参数有误，请重新输入！");
                }
            }
            if (meter_InfoEntity.CYCETIME != null)
            {
                strtemp = meter_InfoEntity.CYCETIME.PadLeft(6, '0');
                if (strtemp.Length == 6)
                {
                    strtlv += "000101"+strtemp;
                }
                else
                {
                    return Info("输入参数有误，请重新输入！");
                }
            }
            if (meter_InfoEntity.TIMELENGTH  != null)
            {
                strtemp = string.Format("{0:X00}", meter_InfoEntity.TIMELENGTH.ToInt()).PadLeft(2, '0');
                if (strtemp.Length == 2)
                {
                    strtlv += strtemp;
                }
                else
                {
                    return Info("输入参数有误，请重新输入！");
                }
            }
            if (meter_InfoEntity.CYCRATE != null)
            {
                strtemp = string.Format("{0:X00}", meter_InfoEntity.CYCRATE.ToInt()).PadLeft(2, '0');
                if (strtemp.Length == 2)
                {
                    strtlv += "08" + strtemp;
                }
                else
                {
                    return Info("输入参数有误，请重新输入！");
                }
            }
            if (meter_InfoEntity.RETRYTIMES != null)
            {
                strtemp = string.Format("{0:X00}", meter_InfoEntity.RETRYTIMES.ToInt()).PadLeft(2, '0');
                if (strtemp.Length == 2)
                {
                    strtlv += "09" + strtemp;
                }
                else
                {
                    return Info("输入参数有误，请重新输入！");
                }
            }
            if (meter_InfoEntity.VOLTALARM != null)
            {
                strtemp = string.Format("{0:X00}", meter_InfoEntity.VOLTALARM*100).PadLeft(4, '0');
                if (strtemp.Length == 4)
                {
                    strtlv += "0A" + strtemp;
                }
                else
                {
                    return Info("输入参数有误，请重新输入！");
                }
            }
            if (meter_InfoEntity.ISTARTHOUR != null)
            {
                strtemp = string.Format("{0:X00}", meter_InfoEntity.ISTARTHOUR.ToInt()).PadLeft(2, '0');
                if (strtemp.Length == 2)
                {
                    strtlv += "0B" + strtemp;
                }
                else
                {
                    return Info("输入参数有误，请重新输入！");
                }
            }
            if (meter_InfoEntity.VALVESTATUS != null)
            {
                strtemp = string.Format("{0:X00}", meter_InfoEntity.VALVESTATUS).PadLeft(2, '0');
                if (strtemp.Length == 2)
                {
                    strtlv += "0C" + strtemp;
                }
                else
                {
                    return Info("输入参数有误，请重新输入！");
                }
            }
            if (meter_InfoEntity.ISSEC != null)
            {
                strtemp = string.Format("{0:X00}", meter_InfoEntity.ISSEC.ToInt()).PadLeft(2, '0');
                if (strtemp.Length == 2)
                {
                    strtlv += "0D" + strtemp;
                }
                else
                {
                    return Info("输入参数有误，请重新输入！");
                }
            }
            if(strtlv.Length==0)
            {
                return Info("至少有一个要设置的参数，请重新输入！");
            }
            strtlv = "03" + string.Format("{0:X00}", strtlv.Length/2).PadLeft(4, '0') + strtlv;

            //取得命令字符串及校验码
            string paraValue1 = "6801" + DateTime.Now.ToString("yyMMddHHmmss") + "0100" + string.Format("{0:X00}", strtlv.Length/2+4).PadLeft(4, '0') + string.Format("{0:X00}", strtlv.Length/2).PadLeft(4, '0') + strtlv+"0000";
            Byte[] bytesParaValue1 = AESUtil.HexStringToByte(paraValue1);
            string checkCode = AESUtil.GetCheckCode(bytesParaValue1);   //二进制算术累加的校验码
            currCmdPara.paraValue = paraValue1 + "" + checkCode + "16";
            lsCmdPars.Add(currCmdPara);

            string result = currsdk.sendCommand(token, deviceId, lsCmdPars);
            if (result != "201")
            {
                return Info("设置命令发送到IoT平台失败，请看日志。");
            }
            return Success("设置命令已成功发送到IoT平台。");
        }
        #endregion


    }
}