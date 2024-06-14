using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.SystemManage;
using TJWCC.Application.WCC;
using TJWCC.Code;
using TJWCC.Data;
using TJWCC.Domain.Entity.WCC;
using TJWCC.Domain.Entity.BSSys;
using System.Threading;
using TJWCC.Application.BSSys;
using System.Data.SqlClient;

namespace TJWCC.Web.Areas.Scheduling.Controllers
{
    public class DailyController : ControllerBase
    {
        log4net.ILog loger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        TJWCCDbContext dbcontext = new TJWCCDbContext();
        private CC_DispatchPlanApp dpApp = new CC_DispatchPlanApp();
        private GY_PumpStationInfoApp pumpSApp = new GY_PumpStationInfoApp();
        private BS_SCADA_TAG_CURRENTApp scadaCurrentApp = new BS_SCADA_TAG_CURRENTApp();

        // 日常调度方案: Scheduling/Daily

        /// <summary>
        /// 获取日常调度方案中监测点实时数据
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetRealtimeData()
        {
            JArray msList = dpApp.GetRealtimeList();
            return Content(msList.ToJson());
        }
        /// <summary>
        /// 获取日常调度方案中调度数据
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetData()
        {
            JArray msList = dpApp.GetList();
            return Content(msList.ToJson());
        }
        public ActionResult GetCalStatus()
        {
            TJWCCDbContext db = new TJWCCDbContext();
            try
            {
                db.Database.Connection.Open();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            var status = db.Database.SqlQuery<int>("SELECT StartFlag FROM [dbo].[BSOT_CalDailyInfo] WHERE Id=1").FirstOrDefault();
            try
            {
                db.Database.Connection.Close();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            switch (status)
            {
                case 1:
                case 2:
                    return Content("1");
                default:
                    return Content("0");
            }
        }
        /// <summary>
        /// 生成日常调度方案
        /// </summary>
        ///<param name="typeId">水厂类型</param> 
        /// <returns></returns>
        public ActionResult CreateDailyPlan(DateTime dataDate, string pressList, bool isCal = true)
        {
            var nowDate = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
            var dutyId = Convert.ToInt32(OperatorProvider.Provider.GetCurrent().DutyId);//班次
            TimeSpan time = new TimeSpan(nowDate.Hour, 0, 0);
            int type = new CC_TheDateApp().GetTheDate(Convert.ToDateTime(DateTime.Now.ToDateString())).Type;//是否工作日
            JArray plr = new JArray();
            TJWCCDbContext db = new TJWCCDbContext();
            try
            {
                db.Database.Connection.Open();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            DbCommand cmd = db.Database.Connection.CreateCommand();
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            var pList = pressList.Split(';');
            pressList = "";
            string calPressList = "";
            string calStationList = "";
            string outInfo = "";
            string toutInfo = "";
            if (pList.Length > 0)
            {
                cmd.CommandText = "UPDATE [dbo].[CC_PressTarget] SET Selected=0 WHERE PlanTime='" + time + "' AND type=" + type;
                cmd.ExecuteNonQuery();
            }
            foreach (var pl in pList)
            {
                JArray tmppls = JArray.FromObject(pl.Split(','));
                tmppls.Add("");
                tmppls.Add("");
                plr.Add(tmppls);
                pressList += string.Join(",", tmppls.ToObject<List<string>>().ToArray()) + ";";
                JArray tmpcpls = JArray.FromObject(pl.Split(','));
                calPressList += tmpcpls[0].ToString() + "," + tmpcpls[2].ToString() + "," + tmpcpls[3].ToString() + ";";
                try
                {
                    var count = dbcontext.Database.SqlQuery<int>("SELECT COUNT(*) FROM CC_PressTarget WHERE Station_key='" + tmpcpls[0].ToString() + "' AND PlanTime='" + time + "' AND type=" + type).FirstOrDefault();
                    if (count > 0)
                        cmd.CommandText = "UPDATE [dbo].[CC_PressTarget] SET [value]=" + tmpcpls[3].ToString() + ",Selected=1 WHERE Station_key='" + tmpcpls[0].ToString() + "' AND PlanTime='" + time + "' AND type=" + type;
                    else
                        cmd.CommandText = "INSERT INTO [dbo].[CC_PressTarget](Station_key,PlanTime,[value],type,Selected) VALUES ('" + tmpcpls[0].ToString() + "','" + time + "'," + tmpcpls[3].ToString() + "," + type + ",1)";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    loger.Error(ex.Message);
                    loger.Error(ex.Source + " |" + ex.StackTrace);
                }
            }
            pressList = pressList.Substring(0, pressList.Length - 1);
            calPressList = calPressList.Substring(0, calPressList.Length - 1);
            var fs = dbcontext.Database.SqlQuery<CC_FlowSortEntity>("SELECT * FROM(SELECT 0 ID, MAX(Save_date) Save_date,0 Sort_key, SUM(Tag_value) Sort_value,1 Type,NULL Remark FROM BS_SCADA_TAG_CURRENT WHERE Station_key " +
                "in(SELECT Station_Key FROM [dbo].[BSM_Meter_Info] WHERE Meter_Type='9')) a WHERE Save_date IS NOT NULL");
            var bff = dbcontext.Database.SqlQuery<BSB_ForecastFlowEntity>("SELECT * FROM [dbo].[BSB_ForecastFlow] WHERE MoreAlert=1 AND DMA=0 AND FlowTime='" + dataDate.AddHours(1).ToString("yyyy-MM-dd HH") + ":00:00'").ToList();
            var pse = pumpSApp.GetSStationList(2, 1);
            foreach (var item in pse)
            {
                var Press = scadaCurrentApp.GetStationAvgValue(item.StationId.ToString(), "117");     //117:出口压力
                var SFlow = dbcontext.Database.SqlQuery<decimal>("SELECT SUM(Tag_value) Sort_value FROM BS_SCADA_TAG_CURRENT WHERE Station_key " +
                "in(SELECT Station_Key FROM [dbo].[BSM_Meter_Info] WHERE Meter_Type='9' AND Station_Unit='" + item.Remark + "') AND Save_date IS NOT NULL").FirstOrDefault();     //水厂出口流量
                calStationList += item.ID + "," + item.StationName + "," + Press.ToDecimal(3) + "," + SFlow.ToDecimal(3) + ";";
                toutInfo += item.StationName + "," + item.ID + "," + Press.ToDecimal(3) + "," + SFlow.ToDecimal(3) + "," + (Press * 1.03m).ToDecimal(3) + "," + (SFlow * 1.1m).ToDecimal(3) + ";";
            }
            calStationList = calStationList.Substring(0, calStationList.Length - 1);
            decimal tmpf = (fs.Select(i => i.Sort_value).FirstOrDefault() == null ? 0m : fs.Select(i => i.Sort_value).FirstOrDefault().ToDecimal(3));
            decimal tmpn = bff.Select(i => i.ForecastFlow).FirstOrDefault().ToDecimal(3);
            cmd.CommandText = "UPDATE [dbo].[BSOT_CalDailyInfo] SET StartFlag=1,CurrentDate='" + nowDate + "',PressStation='" + calPressList + "',PumpStation='" + calStationList + "',WaterFlow=" +
                tmpf + ",WaterFlowNext=" + (tmpn == 0m ? tmpf : tmpn) + " WHERE Id=1";
            try
            {
                cmd.ExecuteNonQuery();
                //db.Database.Connection.Close();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            while (isCal)
            {

                var count = db.Database.SqlQuery<int>("SELECT count(*) FROM [dbo].[BSOT_CalDailyInfo] WHERE Id=1 AND StartFlag=0").FirstOrDefault();
                if (count > 0)
                {
                    outInfo = db.Database.SqlQuery<string>("SELECT OUT FROM [dbo].[BSOT_CalDailyInfo] WHERE Id=1 AND StartFlag=0").FirstOrDefault();
                    var OutInfo = outInfo.Split(';');
                    var toInfo = toutInfo.Split(';');
                    int index = 0;
                    outInfo = "";
                    foreach (var item in pse)
                    {
                        var tmpoi = OutInfo[index].Split(',');
                        var tmptoInfo = toInfo[index].Split(',');
                        outInfo += item.StationName + "," + item.ID + "," + tmptoInfo[2].ToDecimal(3) + "," + tmptoInfo[3].ToDecimal(3) + "," + tmpoi[1].ToDecimal(3) + "," + tmpoi[2].ToDecimal(3) + ";";
                        index++;
                        switch (item.ID)
                        {
                            case 10:
                                cmd.CommandText = "UPDATE [dbo].[BSO_Station_Current] SET Pressure1=" + tmpoi[1].ToDecimal() * 100m + ",Pressure2=" + tmpoi[1].ToDecimal() * 100m + ",Pressure3=" + tmpoi[1].ToDecimal() * 100m +
                                    ",Flow1=" + tmpoi[2].ToDecimal() + " WHERE StationId=2";
                                break;
                            case 11:
                                cmd.CommandText = "UPDATE [dbo].[BSO_Station_Current] SET Pressure1=" + tmpoi[1].ToDecimal() * 100m + ",Pressure2=" + tmpoi[1].ToDecimal() * 100m + ",Pressure3=" + tmpoi[1].ToDecimal() * 100m +
                                    ",Pressure4=" + tmpoi[1].ToDecimal() * 100m + ",Pressure5=" + tmpoi[1].ToDecimal() * 100m + ",Pressure6=" + tmpoi[1].ToDecimal() * 100m + ",Flow1 =" + tmpoi[2].ToDecimal() + " WHERE StationId=3";
                                break;
                            case 12:
                                cmd.CommandText = "UPDATE [dbo].[BSO_Station_Current] SET Pressure1=" + tmpoi[1].ToDecimal() * 100m + ",Pressure2=" + tmpoi[1].ToDecimal() * 100m + ",Pressure3=" + tmpoi[1].ToDecimal() * 100m +
                                    ",Pressure4=" + tmpoi[1].ToDecimal() * 100m + ",Pressure5=" + tmpoi[1].ToDecimal() * 100m + ",Flow1 =" + tmpoi[2].ToDecimal() + " WHERE StationId=4";
                                break;
                            default:
                                cmd.CommandText = "UPDATE [dbo].[BSO_Station_Current] SET Pressure1=" + tmpoi[1].ToDecimal() * 100m + ",Pressure2=" + tmpoi[1].ToDecimal() * 100m + ",Pressure3=" + tmpoi[1].ToDecimal() * 100m +
                                    ",Pressure4=" + tmpoi[1].ToDecimal() * 100m + ",Pressure5=" + tmpoi[1].ToDecimal() * 100m + ",Pressure6=" + tmpoi[1].ToDecimal() * 100m + ",Flow1 =" + tmpoi[2].ToDecimal() + " WHERE StationId=5";
                                break;
                        }
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            loger.Error(ex.Message);
                            loger.Error(ex.Source + " |" + ex.StackTrace);
                        }
                    }
                    outInfo = outInfo.Substring(0, outInfo.Length - 1);
                    try
                    {
                        db.Database.Connection.Close();
                    }
                    catch (Exception ex)
                    {
                        loger.Error(ex.Message);
                        loger.Error(ex.Source + " |" + ex.StackTrace);
                    }
                    break;
                }
                Thread.Sleep(1000);
            }
            if (!isCal) outInfo = toutInfo.Substring(0, toutInfo.Length - 1);
            db.Database.Connection.Close();
            CC_DispatchPlanEntity dpe = new CC_DispatchPlanEntity
            {
                DispatchType = 1,
                DataDate = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, nowDate.Hour, 0, 0),
                CreateDate = nowDate,
                AccordingTo = pressList,
                DispatchOrder = outInfo,
                Status = "芥园水厂:已生成;凌庄水厂:已生成;新开河水厂:已生成;津滨水厂:已生成",
                Operator = OperatorProvider.Provider.GetCurrent().UserName,
                IsPlanBase = 0,
                Shift = dutyId
            };
            dpApp.SubmitForm(dpe, null);
            JArray dpList = dpApp.GetList();
            return Content(dpList.ToJson());
        }
        /// <summary>
        /// 生成日常调度指令
        /// </summary>
        ///<param name="planId">方案ID</param> 
        /// <returns></returns>
        
        public ActionResult CreateDailyOrder(int? planId, string scList)
        {
            TimeSpan time = new TimeSpan(DateTime.Now.Hour, 0, 0);
            var dpe = dpApp.GetListById(planId);
            dpe.Status = "芥园水厂:已下发;凌庄水厂:已下发;新开河水厂:已下发;津滨水厂:已下发";
            dpe.IssueDate = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
            string DC_SCconstr = @" Data Source=10.14.0.16;Initial Catalog=SendOrder;Persist Security Info=True;User ID=sa;pwd=sambo@123;";
            SqlConnection msqlConnnect = new SqlConnection(DC_SCconstr);
            try
            {
                msqlConnnect.Open();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            SqlCommand scmd = msqlConnnect.CreateCommand();
            scmd.CommandTimeout = int.MaxValue;
            var scLists = scList.Split(';');
            var ordLists = dpe.DispatchOrder.Split(';');//根据指令下发内容更新方案信息
            for(int i=0;i<ordLists.Length;i++)
            {
                var ordsc = ordLists[i].Split(',');
                ordsc[4] = ordsc[2];
                foreach (var scs in scLists)
                {
                    var sc = scs.Split(',');
                    if (ordsc[1] == sc[0]) ordsc[4] = sc[3];
                }
                ordLists[i] = string.Join(",", ordsc);
            }
            dpe.DispatchOrder = string.Join(";", ordLists);
            foreach (var scs in scLists)
            {
                var sc = scs.Split(',');
                scmd.CommandText = "INSERT INTO [SendOrder].[dbo].[TJWCC_OrderRecords]([Title], [StateFlag], [CreateDate], [StationId], [StationName], [OrderInfo]) " +
                    "VALUES (N'日常调度指令|" + dpe.ID + "', 0, GETDATE(), " + sc[0] + ", N'" + sc[1] + "', N'将水厂出口压力从" + sc[2] + "调整到" + sc[3] + "')";
                try
                {
                    scmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    loger.Error(ex.Message);
                    loger.Error(ex.Source + " |" + ex.StackTrace);
                }
            }
            try
            {
                msqlConnnect.Close();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            dpApp.SubmitForm(dpe, planId);
            var dpList = dpApp.GetList();
            return Content(dpList.ToJson());
        }
    }
}