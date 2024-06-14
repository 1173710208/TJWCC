using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using Newtonsoft.Json.Linq;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.BSSys;
using TJWCC.Application.SystemManage;
using TJWCC.Application.WCC;
using TJWCC.Code;
using TJWCC.Code.MigraDoc;
using TJWCC.Data;
using TJWCC.Domain.Entity.BSSys;
using TJWCC.Domain.Entity.WCC;

namespace TJWCC.Web.Areas.Scheduling.Controllers
{
    public class PollutionController : ControllerBase
    {
        log4net.ILog loger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private CC_DispatchPlanApp dpApp = new CC_DispatchPlanApp();
        private GY_PumpStationInfoApp pumpSApp = new GY_PumpStationInfoApp();
        private BS_SCADA_TAG_CURRENTApp scadaCurrentApp = new BS_SCADA_TAG_CURRENTApp();
        private BSM_Meter_InfoApp bmiApp = new BSM_Meter_InfoApp();
        TJWCCDbContext dbcontext = new TJWCCDbContext();
        // 水质污染事件: Scheduling/Pollution

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
            var status = db.Database.SqlQuery<int>("SELECT StartFlag FROM [dbo].[BSOT_CalDailyInfo] WHERE Id=5").FirstOrDefault();
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
        [HttpPost]
        public JsonResult PollutionAnalysis(int elementId, int nodeType, string cnbValves)
        {
            var nowDate = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
            ArrayList tmpValves = new ArrayList();
            if (cnbValves != null && cnbValves != "")
                tmpValves = new ArrayList(Array.ConvertAll(cnbValves.Split(','), int.Parse));
            TubeBurstAnalysisApp tba = new TubeBurstAnalysisApp();
            ArrayList result = new ArrayList();
            if (nodeType == 69)
                result = tba.IsolatePipe(elementId, tmpValves);
            else
                result = tba.IsolateNode(elementId, nodeType, tmpValves);

            string foundNodes = string.Join(",", ((ArrayList)result[0]).ToArray());//关键节点arraylist数据转成数组数据
            string foundPipes = string.Join(",", ((ArrayList)result[2]).ToArray());//关键管道arraylist数据转成数组数据
            string foundValves = string.Join(",", ((ArrayList)result[3]).ToArray());//关键阀门arraylist数据转成数组数据
            //Process p = new Process();
            //string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Pollution.py";// 获得python文件的绝对路径（将文件放在c#的debug文件夹中可以这样操作）
            //path = @"D:\sambo\Pollution.py";//(因为我没放debug下，所以直接写的绝对路径,替换掉上面的路径了)
            //p.StartInfo.FileName = @"C:\Users\Administrator\AppData\Local\Programs\Python\Python39\python.exe";//没有配环境变量的话，可以像我这样写python.exe的绝对路径。如果配了，直接写"python.exe"即可
            //string sArguments = path;
            //sArguments += " " + foundValves + " " + foundNodes + " " + foundPipes;//传递参数
            //sArguments += " -u";
            //p.StartInfo.Arguments = sArguments;
            //p.StartInfo.UseShellExecute = false;
            //p.StartInfo.RedirectStandardOutput = true;
            //p.StartInfo.RedirectStandardInput = true;
            //p.StartInfo.RedirectStandardError = true;
            //p.StartInfo.CreateNoWindow = true;

            //p.Start();
            //p.BeginOutputReadLine();
            ////p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
            ////Console.ReadLine();
            //p.WaitForExit();
            var fs = dbcontext.Database.SqlQuery<CC_FlowSortEntity>("SELECT * FROM(SELECT 0 ID, MAX(Save_date) Save_date,0 Sort_key, SUM(Tag_value) Sort_value,1 Type,NULL Remark FROM BS_SCADA_TAG_CURRENT WHERE Station_key " +
                "in(SELECT Station_Key FROM [dbo].[BSM_Meter_Info] WHERE Meter_Type='9')) a WHERE Save_date IS NOT NULL");
            var bff = dbcontext.Database.SqlQuery<BSB_ForecastFlowEntity>("SELECT * FROM [dbo].[BSB_ForecastFlow] WHERE MoreAlert=1 AND DMA=0 AND FlowTime='" + nowDate.AddHours(1).ToString("yyyy-MM-dd HH") + ":00:00'").ToList();
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
            decimal tmpf = (fs.Select(i => i.Sort_value).FirstOrDefault() == null ? 0m : fs.Select(i => i.Sort_value).FirstOrDefault().ToDecimal(3));
            decimal tmpn = bff.Select(i => i.ForecastFlow).FirstOrDefault().ToDecimal(3);
            DbCommand cmd = db.Database.Connection.CreateCommand();
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "UPDATE [dbo].[BSOT_CalDailyInfo] SET StartFlag=1,CurrentDate='" + nowDate + "',PressStation='" + foundValves 
                 + "',WaterFlow=" + tmpf + ",WaterFlowNext=" + (tmpn == 0m ? tmpf : tmpn) + " WHERE Id=5";
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            while (true)
            {
                var count = db.Database.SqlQuery<int>("SELECT count(*) FROM [dbo].[BSOT_CalDailyInfo] WHERE Id=5 AND StartFlag=0").FirstOrDefault();
                if (count > 0)
                {
                    cmd.CommandText = "UPDATE [dbo].[RESULT_PIPE_TJWCC] SET Result_Flow_7=0 WHERE ElementID in(SELECT ElementID FROM PIPE WHERE StartNodeID IN(SELECT ElementID " +
                        "FROM RESULT_JUNCTION_TJWCC WHERE Result_Pressure_7=0)) OR ElementID in(SELECT ElementID FROM PIPE WHERE EndNodeID IN(SELECT ElementID FROM RESULT_JUNCTION_TJWCC " +
                        "WHERE Result_Pressure_7=0))";
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        loger.Error(ex.Message);
                        loger.Error(ex.Source + " |" + ex.StackTrace);
                    }
                    cmd.CommandText = "UPDATE [dbo].[RESULT_PIPE_TJWCC] SET Result_Flow_7=0 WHERE ElementID in(SELECT ElementID FROM PIPE WHERE StartNodeID IN("+ foundValves + ")) OR " +
                        "ElementID in(SELECT ElementID FROM PIPE WHERE EndNodeID IN(" + foundValves + "))";
                    try
                    {
                        cmd.ExecuteNonQuery();
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
            try
            {
                db.Database.Connection.Close();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SupplyWaterWay(int elementId,int nodeType,bool swwType)
        {
            //var ElementId = int.Parse(Request["id"]);
            //var nodeType = int.Parse(Request["nodetype"]);
            //var swwType = bool.Parse(Request["swwtype"]);
            SupplyWaterWayApp sww = new SupplyWaterWayApp();
            ArrayList result = null;
            result = sww.FindSupplyway(elementId, nodeType, swwType, 24);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreatePlan(string valveList, string base64Info)
        {
            var nowDate = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
            JArray result = new JArray();
            var vl = valveList.Split('|');
            string HeadStr = "管网水质污染应急方案";
            byte[] buffer = Convert.FromBase64String(base64Info.Replace(" ", "+").Split(',')[1]);
            ////保存到指定路径
            string fullPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\file\image\" + nowDate.ToString("yyyyMMddHHmmss") + ".png";
            //FileStream fileStream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)
            //{
            //    Position = 0L
            //};
            //fileStream.Write(buffer, 0, buffer.Length);
            //fileStream.Close();
            double[] with = new double[] { 1.5, 4.5, 2, 2, 1.4, 2, 2.3 };
            string[] itemNames = new string[] { "ID", "所在管道", "X坐标", "Y坐标", "管径", "管材", "营销公司" };
            double[] with1 = new double[] { 1.5, 4.5, 3.6, 2.4 };
            string[] itemNames1 = new string[] { "ID", "地址", "GID", "口径(mm)" };
            double[] with2 = new double[] { 4.2, 3.6 };
            string[] itemNames2 = new string[] { "停水用户(以地表计,块)", "影响区域面积(km²)" };
            string[] itemNames3 = new string[] { "管道排空时间(h)" };
            string[] itemNames4 = new string[] { "名称", "地址", "营销公司" };
            JArray items = new JArray();
            var ctmps = vl[0].Split(',');
            JArray tmp = new JArray();
            for (var i = 0; i < ctmps.Length; i++)
            {
                if (i != 0) tmp.Add(ctmps[i]);
            }
            items.Add(tmp);
            JArray items1 = new JArray();
            ctmps = vl[1].Split(';');
            foreach (var ctmp in ctmps)
            {
                var ctmpis = ctmp.Split(',');
                JArray tmp1 = new JArray();
                foreach (var ctmpi in ctmpis)
                {
                    tmp1.Add(ctmpi);
                }
                items1.Add(tmp1);
            }
            ctmps = vl[2].Split(',');
            JArray items2 = new JArray
            {
                JArray.FromObject(ctmps)
            };
            ctmps = vl[3].Split(',');
            JArray items3 = new JArray
            {
                JArray.FromObject(ctmps)
            };
            JArray items4 = new JArray();
            if (vl[4].Length > 0)
            {
                ctmps = vl[4].Split(';');
                foreach (var ctmp in ctmps)
                {
                    var ctmpis = ctmp.Split(',');
                    JArray tmp1 = new JArray();
                    foreach (var ctmpi in ctmpis)
                    {
                        tmp1.Add(ctmpi);
                    }
                    items4.Add(tmp1);
                }
            }
            var document = new Document();
            document.Info.Title = "SAMBO";
            document.Info.Subject = "SAMBO";
            document.Info.Author = "SAMBO";

            Code.MigraDoc.Styles.DefineStyles(document);
            Cover.DefineCover(document);
            Code.MigraDoc.Styles.DefineContentSection(document);
            document.LastSection.AddParagraph(HeadStr, "Heading1");
            Tables.DemonstrateSimpleTable(document, with, itemNames, items);
            document.LastSection.AddParagraph("关阀明细", "Heading2");
            Tables.DemonstrateSimpleTable(document, with1, itemNames1, items1);
            document.LastSection.AddParagraph("关阀后影响", "Heading2");
            Tables.DemonstrateSimpleTable(document, with2, itemNames2, items2);
            document.LastSection.AddParagraph("排空时间", "Heading2");
            Tables.DemonstrateSimpleTable(document, with2, itemNames3, items3);
            document.LastSection.AddParagraph("关阀后受影响用户明细表(以地表计,共" + items4.Count + "块)", "Heading2");
            Tables.DemonstrateSimpleTable(document, with1, itemNames4, items4);
            var renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\file\pdf\" + HeadStr + nowDate.ToString("yyyyMMddHHmmss") + ".pdf");
            string path = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/file/pdf/" + HeadStr + nowDate.ToString("yyyyMMddHHmmss") + ".pdf";
            var dutyId = Convert.ToInt32(OperatorProvider.Provider.GetCurrent().DutyId);//班次
            //CC_DispatchPlanEntity dpe = new CC_DispatchPlanEntity
            //{
            //    DispatchType = 5,
            //    DataDate = nowDate,
            //    CreateDate = nowDate,
            //    AccordingTo = vl[0],
            //    DispatchOrder = vl[1],
            //    Status = 1,
            //    Operator = OperatorProvider.Provider.GetCurrent().UserName,
            //    IsPlanBase = 0,
            //    Shift = dutyId
            //};
            //dpApp.SubmitForm(dpe, null);
            var ndp = dpApp.GetNewPlan(5);
            //result.Add(JObject.FromObject(ndp));
            result.Add(path);
            result.Add(path);
            return Content(result.ToJson());
        }
        public ActionResult CreatePolOrder(int? planId)
        {
            TimeSpan time = new TimeSpan(DateTime.Now.Hour, 0, 0);
            var dpe = dpApp.GetListById(planId);
            dpe.IssueDate = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
            dpApp.SubmitForm(dpe, planId);
            return Success("下发成功！");
        }
        public void ValveSWCalButton()//阀门供水路径预计算
        {
            SupplyWaterAllWayApp swaw = new SupplyWaterAllWayApp();
            List<int> tmpValve = new TCVApp().GetList().Select(item => item.ElementId.Value).ToList();
            for (int i = 0; i < tmpValve.Count; i++)
                swaw.FindSupplyway(tmpValve[i], 61, 24);
            tmpValve.Clear();
        }
        public void HydrantSWCalButton()//消火栓供水路径预计算
        {
            SupplyWaterAllWayApp swaw = new SupplyWaterAllWayApp();
            List<int> tmpHydrant = new HYDRANTApp().GetList().Select(item => item.ElementId.Value).ToList();
            for (int i = 0; i < tmpHydrant.Count; i++)
                swaw.FindSupplyway(tmpHydrant[i], 54, 24);
            tmpHydrant.Clear();
        }
        public void JunctionSWCalButton()//节点供水路径预计算
        {
            SupplyWaterAllWayApp swaw = new SupplyWaterAllWayApp();
            List<int> tmpJunction = new JUNCTIONApp().GetList().Select(item => item.ElementId.Value).ToList();
            for (int i = 0; i < tmpJunction.Count; i++)
                swaw.FindSupplyway(tmpJunction[i], 55, 24);
            tmpJunction.Clear();
        }
    }
}