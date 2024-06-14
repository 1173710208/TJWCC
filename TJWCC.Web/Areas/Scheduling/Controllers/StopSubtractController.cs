using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using Newtonsoft.Json.Linq;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
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
using TJWCC.Domain.ViewModel;

namespace TJWCC.Web.Areas.Scheduling.Controllers
{
    public class StopSubtractController : ControllerBase
    {
        log4net.ILog loger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        TJWCCDbContext dbcontext = new TJWCCDbContext();
        private CC_DispatchPlanApp dpApp = new CC_DispatchPlanApp();
        private GY_PumpStationInfoApp pumpSApp = new GY_PumpStationInfoApp();
        private BS_SCADA_TAG_CURRENTApp scadaCurrentApp = new BS_SCADA_TAG_CURRENTApp();
        // 水厂停（减）产: Scheduling/StopSubtract

        public ActionResult GetCalStatus(int stopType)
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
            var status = db.Database.SqlQuery<int>("SELECT StartFlag FROM [dbo].[BSOT_CalDailyInfo] WHERE Id=" + (stopType + 1)).FirstOrDefault();
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

        public ActionResult CreatePlan(string scList, string gsscList, int stopType, bool isCal = true)
        {
            var nowDate = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
            var dutyId = Convert.ToInt32(OperatorProvider.Provider.GetCurrent().DutyId);//班次
            TimeSpan time = new TimeSpan(nowDate.Hour, 0, 0);
            //int type = new CC_TheDateApp().GetTheDate(Convert.ToDateTime(DateTime.Now.ToDateString())).Type;//是否工作日
            JArray result = new JArray();
            JArray plr = new JArray();
            string[] itemNames = new string[] { "水厂ID", "水厂", "现状供水压力(MPa)", "现状供水流量(m³/h)", "方案供水压力(MPa)", "方案供水流量(m³/h)" };
            var pList = scList.Split(';');
            var gpList = gsscList.Split(';');
            string pressList = "";
            string scStatus = "";
            string outInfo = "";
            string toutInfo = "";
            string calStationList = "";
            string sqlRe = "SELECT ElementID,WMeter_ID,Meter_Name,Station_Unit,Geo_x,Geo_y,[Explain],Remark,Tag_value,Save_date,Measure_Grade FROM(SELECT ElementID,WMeter_ID,Meter_Name,ltrim(Station_Unit) Station_Unit," +
                "Geo_x,Geo_y,[Explain],[value] Remark,Tag_value,Save_date,Measure_Grade,c.ID FROM (SELECT ElementID, WMeter_ID, Meter_Name, Station_Unit, Geo_x, Geo_y,[Explain],Station_Key,Meter_Type,Remark," +
                "Measure_Grade FROM [dbo].[BSM_Meter_Info] WHERE Meter_Type =2 AND Display=1) bmi LEFT JOIN (SELECT (CASE WHEN Tag_value is null THEN 0 ELSE Tag_value END) Tag_value,GYStationId,Save_date,Tag_key " +
                "FROM BS_SCADA_TAG_CURRENT where Tag_key =2) bth ON bmi.Station_Key = bth.GYStationId and bmi.Meter_Type = bth.Tag_key LEFT JOIN (SELECT ID,CONVERT(VARCHAR(10),[value]) [value],Station_Key FROM " +
                "CC_PressTarget WHERE Selected=1 AND PlanTime='" + time + "' AND type=3) c ON c.Station_Key=WMeter_ID) a WHERE Remark IS NOT NULL ORDER BY ID";
            var pressRe = dbcontext.Database.SqlQuery<GetNewData_Result>(sqlRe).ToArray();
            foreach (var pl in pressRe)
            {
                pressList += pl.WMeter_ID + "," + pl.Tag_value + "," + pl.Remark + ";";
            }
            //scList = scList.Substring(0, scList.Length - 1);
            var fs = dbcontext.Database.SqlQuery<CC_FlowSortEntity>("SELECT * FROM(SELECT 0 ID, MAX(Save_date) Save_date,0 Sort_key, SUM(Tag_value) Sort_value,1 Type,NULL Remark FROM BS_SCADA_TAG_CURRENT WHERE GYStationId " +
                "in(SELECT Station_Key FROM [dbo].[BSM_Meter_Info] WHERE Meter_Type='9')) a WHERE Save_date IS NOT NULL");
            //var bff = dbcontext.Database.SqlQuery<BSB_ForecastFlowEntity>("SELECT * FROM [dbo].[BSB_ForecastFlow] WHERE MoreAlert=1 AND DMA=0 AND FlowTime='" + nowDate.AddHours(1).ToString("yyyy-MM-dd HH") + ":00:00'").ToList();
            var pse = pumpSApp.GetSStationList(2, 1);
            foreach (var item in pse)
            {
                var Press = scadaCurrentApp.GetStationAvgValue(item.StationId.ToString(), "117").ToDecimal(3);     //117:出口压力
                var SFlow = dbcontext.Database.SqlQuery<decimal>("SELECT SUM(Tag_value) Sort_value FROM BS_SCADA_TAG_CURRENT WHERE GYStationId " +
                "in(SELECT Station_Key FROM [dbo].[BSM_Meter_Info] WHERE Meter_Type='9' AND Station_Unit='" + item.Remark + "') AND Save_date IS NOT NULL").FirstOrDefault().ToDecimal(2);     //水厂出口流量
                calStationList += item.ID + "," + item.StationName + "," + Press + "," + SFlow + ";";
                toutInfo += item.StationName + "," + item.ID + "," + Press + "," + SFlow + "," + (Press * 1.03m).ToDecimal(3) + "," + (SFlow * 1.1m).ToDecimal(2) + ";";
            }
            calStationList = calStationList.Substring(0, calStationList.Length - 1);
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
            //decimal tmpn = bff.Select(i => i.ForecastFlow).FirstOrDefault().ToDecimal(3);
            DbCommand cmd = db.Database.Connection.CreateCommand();
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "UPDATE [dbo].[BSOT_CalDailyInfo] SET StartFlag=1,CurrentDate='" + nowDate + "',PressStation='" + pressList.Substring(0, pressList.Length - 1) + "|" +
                scList + "|"+ gsscList + "',PumpStation='" + calStationList + "',WaterFlow=" + tmpf + ",WaterFlowNext=" + tmpf + " WHERE Id=" + (stopType + 1);
            try
            {
                cmd.ExecuteNonQuery();
                db.Database.Connection.Close();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            while (isCal)
            {

                Thread.Sleep(1000);
                var count = db.Database.SqlQuery<int>("SELECT count(*) FROM [dbo].[BSOT_CalDailyInfo] WHERE Id=" + (stopType + 1) + " AND StartFlag=0").FirstOrDefault();
                if (count > 0)
                {
                    calStationList = "";
                    outInfo = db.Database.SqlQuery<string>("SELECT OUT FROM [dbo].[BSOT_CalDailyInfo] WHERE Id=" + (stopType + 1) + " AND StartFlag=0").FirstOrDefault();
                    var OutInfo = outInfo.Split(';');
                    var toInfo = toutInfo.Split(';');
                    int index = 0;
                    foreach (var item in pse)
                    {
                        var tmpoi = OutInfo[index].Split(',');
                        var tmptoInfo = toInfo[index].Split(',');
                        if (stopType == 1)
                        {
                            if (item.StationId == Convert.ToInt32(pList[0].Split(',')[0]))
                            {
                                calStationList += "-," + item.StationName + ",-,-,-,-;";
                            }
                            else
                            {
                                calStationList += item.ID + "," + item.StationName + "," + tmptoInfo[2].ToDecimal(3) + "," + tmptoInfo[3].ToDecimal(2) + "," + tmpoi[1].ToDecimal(3) + "," + tmpoi[2].ToDecimal(2) + ";";
                                //if (index == 1)
                                //    scStatus += item.StationName + ":已生成;<br/>";
                                //else
                                    scStatus += item.StationName + ":已生成;";
                            }
                        }
                        else
                        {
                            calStationList += item.ID + "," + item.StationName + "," + tmptoInfo[2].ToDecimal(3) + "," + tmptoInfo[3].ToDecimal(2) + "," + tmpoi[1].ToDecimal(3) + "," + tmpoi[2].ToDecimal(2) + ";";
                            //if (index == 1)
                            //    scStatus += item.StationName + ":已生成;<br/>";
                            //else
                                scStatus += item.StationName + ":已生成;";
                        }
                        index++;
                    }
                    calStationList = calStationList.Substring(0, calStationList.Length - 1);
                    scStatus = scStatus.Substring(0, scStatus.Length - 1);
                    break;
                }
            }
            if (!isCal)
            {
                calStationList = "";
                int index = 0;
                var toInfo = toutInfo.Split(';');
                foreach (var item in pse)
                {
                    var tmptoInfo = toInfo[index].Split(',');
                    if (stopType == 1)
                    {
                        if (item.StationId == Convert.ToInt32(pList[0].Split(',')[0]))
                        {
                            calStationList += "-," + item.StationName + ",-,-,-,-;";
                        }
                        else
                        {
                            calStationList += item.ID + "," + item.StationName + "," + tmptoInfo[2].ToDecimal(3) + "," + tmptoInfo[3].ToDecimal(2) + "," + (tmptoInfo[2].ToDecimal() * 1.2m).ToDecimal(3) + "," + (tmptoInfo[3].ToDecimal() * 1.3m).ToDecimal(2) + ";";
                            //if (index == 1)
                            //    scStatus += item.StationName + ":已生成;<br/>";
                            //else
                                scStatus += item.StationName + ":已生成;";
                        }
                    }
                    else
                    {
                        //foreach (var pl in pList)
                        //{
                        calStationList += item.ID + "," + item.StationName + "," + tmptoInfo[2].ToDecimal(3) + "," + tmptoInfo[3].ToDecimal(2) + "," + (tmptoInfo[2].ToDecimal() * 1.2m).ToDecimal(3) + "," + (tmptoInfo[3].ToDecimal() * 1.3m).ToDecimal(2) + ";";
                        //calStationList = "10,芥园水厂,0.321,10000,0.382,12000;11,凌庄水厂,0.323,10000,0.323,12000;12,津滨水厂,0.325,10000,0.325,12000;13,新开河水厂,0.327,10000,0.327,12000;";
                        //}
                        //if (index == 1)
                        //    scStatus += item.StationName + ":已生成;<br/>";
                        //else
                            scStatus += item.StationName + ":已生成;";
                    }
                    index++;
                }
                calStationList = calStationList.Substring(0, calStationList.Length - 1);
                scStatus = scStatus.Substring(0, scStatus.Length - 1);
            }
            var aui1 = dbcontext.Database.SqlQuery<AffectUserInfo>("SELECT COUNT(*) Count,CONVERT(DECIMAL(20,2),COUNT(*)*400.0/1000000.0) Area FROM BSB_LinkedTable WHERE ElementID in(SELECT ElementID FROM RESULT_JUNCTION_TJWCC " +
                "WHERE Result_Pressure_2<0.18 AND Is_Active=1)").FirstOrDefault();     //调度前影响地表情况
            var aui2 = dbcontext.Database.SqlQuery<AffectUserInfo>("SELECT COUNT(*) Count,CONVERT(DECIMAL(20,2),COUNT(*)*400.0/1000000.0) Area FROM BSB_LinkedTable WHERE ElementID in(SELECT ElementID FROM RESULT_JUNCTION_TJWCC " +
                "WHERE Result_Pressure_3<0.18 AND Is_Active=1)").FirstOrDefault();     //调度后影响地表情况
            var aui3 = dbcontext.Database.SqlQuery<BSB_LinkedTableEntity>("SELECT TOP 200 * FROM BSB_LinkedTable WHERE ElementID in(SELECT ElementID FROM RESULT_JUNCTION_TJWCC " +
                "WHERE Result_Pressure_2<0.18 AND Is_Active=1)").ToList();     //调度前影响地表信息
            var aui4 = dbcontext.Database.SqlQuery<BSB_LinkedTableEntity>("SELECT TOP 200 * FROM BSB_LinkedTable WHERE ElementID in(SELECT ElementID FROM RESULT_JUNCTION_TJWCC " +
                "WHERE Result_Pressure_3<0.18 AND Is_Active=1)").ToList();     //调度后影响地表信息
            db.Database.Connection.Close();

            string HeadStr = "";
            //byte[] buffer = Convert.FromBase64String(base64Info.Replace(" ", "+").Split(',')[1]);
            //保存到指定路径
            string fullPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\file\image\" + nowDate.ToString("yyyyMMddHHmmss") + ".png";
            //FileStream fileStream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)
            //{
            //    Position = 0L
            //};
            //fileStream.Write(buffer, 0, buffer.Length);
            //fileStream.Close();
            //string templateFilePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\file\pdf\Tmp2022.pdf";


            //PdfDocument doc = PdfSharp.Pdf.IO.PdfReader.Open(templateFilePath, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Modify);
            switch (stopType)
            {
                case 1:
                    HeadStr = "水厂停产应急方案";
                    break;
                case 2:
                    HeadStr = "水厂减产应急方案";
                    break;
                case 3:
                    HeadStr = "基于当前减产应急方案";
                    break;
            }
            //PdfPage page0 = doc.Pages[0];
            //XGraphics gfx0 = XGraphics.FromPdfPage(page0);
            //XFont font = new XFont("华文宋体", 30, XFontStyle.Bold);
            //gfx0.DrawString(HeadStr, font, XBrushes.Black, new XRect(30, 30, page0.Width - 2 * 30, 80), XStringFormats.Center);
            //font = new XFont("华文宋体", 12, XFontStyle.Regular);
            //gfx0.DrawString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), font, XBrushes.Black, new XRect(30, 120, 100, 20), XStringFormats.CenterLeft);
            //gfx0.DrawString("1001", font, XBrushes.Black, new XRect(30, 150, 100, 20), XStringFormats.CenterLeft);
            //PdfPage page1 = doc.Pages[1];
            //XGraphics gfx1 = XGraphics.FromPdfPage(page1);
            //XImage img = XImage.FromFile(fullPath);
            //XGraphics.FromImage(img);
            //gfx1.DrawImage(img, 30, 30, 800, 400);
            JArray items = new JArray();

            var ctmps = calStationList.Split(';');
            foreach (var ctmp in ctmps)
            {
                var ctmpis = ctmp.Split(',');
                JArray tmp = new JArray();
                foreach (var ctmpi in ctmpis)
                {
                    tmp.Add(ctmpi);
                }
                JArray tmp1 = new JArray();
                items.Add(tmp);
            }
            JArray items3 = new JArray();
            if (aui3 != null)
            {
                foreach (var ctmp in aui3)
                {
                    JArray tmp1 = new JArray();
                    tmp1.Add(ctmp.USERNAME);
                    tmp1.Add(ctmp.Address);
                    tmp1.Add(ctmp.Label);
                    items3.Add(tmp1);
                }
            }
            JArray items4 = new JArray();
            if (aui4 != null)
            {
                foreach (var ctmp in aui4)
                {
                    JArray tmp1 = new JArray();
                    tmp1.Add(ctmp.USERNAME);
                    tmp1.Add(ctmp.Address);
                    tmp1.Add(ctmp.Label);
                    items4.Add(tmp1);
                }
            }
            string[] itemNames1 = new string[] { "停水用户(以地表计,块)", "影响区域面积(km²)" };
            string[] itemNames3 = new string[] { "名称", "地址", "营销公司" };
            double[] with = new double[] { 1.5, 2, 3.3 };
            double[] with1 = new double[] { 4.2, 3.6 };
            double[] with2 = new double[] { 1.5, 4.2, 3.3 };
            JArray items1 = new JArray
            {
                JArray.FromObject(new string[] { aui1.Count.ToString(), aui1.Area.ToString() }),
                JArray.FromObject(new string[] { aui2.Count.ToString(), aui2.Area.ToString() })
            };
            var document = new Document();
            document.Info.Title = "SAMBO";
            document.Info.Subject = "SAMBO";
            document.Info.Author = "SAMBO";

            Code.MigraDoc.Styles.DefineStyles(document);
            Cover.DefineCover(document);
            Code.MigraDoc.Styles.DefineContentSection(document);
            document.LastSection.AddParagraph(HeadStr, "Heading1");
            Tables.DemonstrateSimpleTable(document, with, itemNames, items);
            document.LastSection.AddParagraph("处置效果对比", "Heading2");
            Tables.DemonstrateSimpleTable(document, with1, itemNames1, items1);
            var renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            //EZFontResolver fontResolver = EZFontResolver.Get;
            //GlobalFontSettings.FontResolver = fontResolver;
            //fontResolver.AddFont("微软雅黑", XFontStyle.Regular, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\Content\fonts\msyh.ttf", true, true);            // Get the predefined style Normal.
            renderer.RenderDocument();
            renderer.PdfDocument.Save(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\file\pdf\" + HeadStr + nowDate.ToString("yyyyMMddHHmmss") + ".pdf");
            string path = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/file/pdf/" + HeadStr + nowDate.ToString("yyyyMMddHHmmss") + ".pdf";
            CC_DispatchPlanEntity dpe = new CC_DispatchPlanEntity
            {
                DispatchType = 3,
                DataDate = Convert.ToDateTime(nowDate.ToString("yyyy-MM-dd HH") + ":00:00"),
                CreateDate = nowDate,
                AccordingTo = scList,
                DispatchOrder = calStationList,
                Effect = aui1.Count + "," + aui1.Area + ";" + aui2.Count + "," + aui2.Area,
                LinkedMeters = gsscList,
                Status = scStatus,
                Operator = OperatorProvider.Provider.GetCurrent().UserName,
                IsPlanBase = 0,
                Shift = dutyId
            };
            dpApp.SubmitForm(dpe, null);
            result.Add(calStationList);
            result.Add(JArray.FromObject(new string[] { aui1.Count.ToString(), aui1.Area.Value.ToString("0.##") }));
            result.Add(JArray.FromObject(new string[] { aui2.Count.ToString(), aui2.Area.Value.ToString("0.##") }));
            var ndp = dpApp.GetNewPlan(3);
            result.Add(JObject.FromObject(ndp));
            result.Add(items3);
            result.Add(items4);
            result.Add(path);
            return Content(result.ToJson());
        }

        public ActionResult CreateOrder(int? planId, string scList)
        {
            TimeSpan time = new TimeSpan(DateTime.Now.Hour, 0, 0);

            var dpe = dpApp.GetListById(planId);
            string[] pumpNames = dpe.Status.Split(';');
            string Status = "";
            foreach (var pumpName in pumpNames)
            {
                Status += pumpName.Split(':')[0] + ":已下发;";
                //if (Status.Length > 17 && Status.Length < 27) Status += "<br/>";
            }
            dpe.Status = Status.Substring(0, Status.Length - 1);
            dpe.IssueDate = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
            string HeadStr = "";
            string[] itemNames = new string[] { "水厂ID", "水厂", "现状供水压力(MPa)", "现状供水流量(m³/h)", "方案供水压力(MPa)", "方案供水流量(m³/h)" };
            //保存到指定路径
            string fullPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\file\image\" + dpe.DataDate.Value.ToString("yyyyMMddHHmmss") + ".png";
            if (dpe.DispatchOrder.IndexOf("-,-,-") > 0)
            {
                HeadStr = "水厂停产应急方案";
            }
            else
            {
                HeadStr = "水厂减产应急方案";
            }
            JArray items = new JArray();
            var ctmps = dpe.DispatchOrder.Split(';');
            foreach (var ctmp in ctmps)
            {
                var ctmpis = ctmp.Split(',');
                JArray tmp = new JArray();
                foreach (var ctmpi in ctmpis)
                {
                    tmp.Add(ctmpi);
                }
                items.Add(tmp);
            }
            string[] itemNames1 = new string[] { "停水用户（以地表计，块）", "影响区域面积（km²）" };
            ctmps = dpe.Effect.Split(';');
            JArray items1 = new JArray();
            foreach (var ctmp in ctmps)
            {
                var ctmpis = ctmp.Split(',');
                items1.Add(JArray.FromObject(new string[] { ctmpis[0], ctmpis[1] }));
            }
            double[] with = new double[] { 1.5, 2, 3.3 };
            double[] with1 = new double[] { 4.2, 3.6 };
            double[] with2 = new double[] { 1.5, 2, 3.3 };
            double[] with3 = new double[] { 1.5, 4.2, 3.3 };
            string[] itemNames2 = new string[] { "水厂ID", "水厂", "方案供水压力(MPa)" };
            JArray items2 = new JArray();
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
            ctmps = scList.Split(';');
            var ordLists = dpe.DispatchOrder.Split(';');//根据指令下发内容更新方案信息
            for (int i = 0; i < ordLists.Length; i++)
            {
                var ordsc = ordLists[i].Split(',');
                ordsc[4] = ordsc[2];
                foreach (var scs in ctmps)
                {
                    var sc = scs.Split(',');
                    if (ordsc[1] == sc[0]) ordsc[4] = sc[1];
                }
                ordLists[i] = string.Join(",", ordsc);
            }
            dpe.DispatchOrder = string.Join(";", ordLists);
            foreach (var ctmp in ctmps)
            {
                var ctmpis = ctmp.Split(',');
                JArray tmp = new JArray();
                foreach (var ctmpi in ctmpis)
                {
                    tmp.Add(ctmpi);
                    if (tmp.Count == 1)
                    {
                        switch (ctmpi)
                        {
                            case "10": tmp.Add("芥园水厂"); break;
                            case "11": tmp.Add("凌庄水厂"); break;
                            case "12": tmp.Add("新开河水厂"); break;
                            case "13": tmp.Add("津滨水厂"); break;
                        }
                    }
                }
                items2.Add(tmp);
                scmd.CommandText = "INSERT INTO [SendOrder].[dbo].[TJWCC_OrderRecords]([Title], [StateFlag], [CreateDate], [StationId], [StationName], [OrderInfo]) " +
                    "VALUES (N'" + HeadStr.Substring(0, HeadStr.Length - 4) + "|" + dpe.ID + "', 0,GETDATE(), " + tmp[0].ToString() + ", N'" + tmp[1].ToString() + "', N'将水厂出口压力设置到" + tmp[2].ToString() + "')";
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
            var document = new Document();
            document.Info.Title = "SAMBO";
            document.Info.Subject = "SAMBO";
            document.Info.Author = "SAMBO";

            Code.MigraDoc.Styles.DefineStyles(document);
            Cover.DefineCover(document);
            Code.MigraDoc.Styles.DefineContentSection(document);
            document.LastSection.AddParagraph(HeadStr, "Heading1");
            Tables.DemonstrateSimpleTable(document, with, itemNames, items);
            document.LastSection.AddParagraph("处置效果对比", "Heading2");
            Tables.DemonstrateSimpleTable(document, with1, itemNames1, items1);
            document.LastSection.AddParagraph("下发调度指令", "Heading2");
            Tables.DemonstrateSimpleTable(document, with2, itemNames2, items2);
            string filename = DateTime.Now.ToString("yyyyMMddHHmmss");
            var renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            //EZFontResolver fontResolver = EZFontResolver.Get;
            //GlobalFontSettings.FontResolver = fontResolver;
            //fontResolver.AddFont("微软雅黑", XFontStyle.Regular, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\Content\fonts\msyh.ttf", true, true);            // Get the predefined style Normal.
            renderer.RenderDocument();
            renderer.PdfDocument.Save(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\file\pdf\" + HeadStr + filename + ".pdf");
            string path = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/file/pdf/" + HeadStr + filename + ".pdf";
            return Content(path);
        }
        public ActionResult GetBeforeUserInfo(int? planId)
        {
            TimeSpan time = new TimeSpan(DateTime.Now.Hour, 0, 0);
            var aui3 = dbcontext.Database.SqlQuery<BSB_LinkedTableEntity>("SELECT * FROM BSB_LinkedTable WHERE ElementID in(SELECT ElementID FROM RESULT_JUNCTION_TJWCC " +
                "WHERE Result_Pressure_2<0.18 AND Is_Active=1)").ToList();     //调度前影响地表信息

            var dpe = dpApp.GetListById(planId);
            string HeadStr = "";
            if (dpe.DispatchOrder.IndexOf("-,-,-") > 0)
            {
                HeadStr = "水厂停产调度前影响用户";
            }
            else
            {
                HeadStr = "水厂减产调度前影响用户";
            }
            JArray items3 = new JArray();
            if (aui3 != null)
            {
                foreach (var ctmp in aui3)
                {
                    JArray tmp1 = new JArray();
                    tmp1.Add(ctmp.USERNAME);
                    tmp1.Add(ctmp.Address);
                    tmp1.Add(ctmp.Label);
                    items3.Add(tmp1);
                }
            }
            double[] with3 = new double[] { 2.5, 7.2, 3.3 };
            string[] itemNames3 = new string[] { "名称", "地址", "营销公司" };
            var document = new Document();
            document.Info.Title = "SAMBO";
            document.Info.Subject = "SAMBO";
            document.Info.Author = "SAMBO";

            Code.MigraDoc.Styles.DefineStyles(document);
            Cover.DefineCover(document);
            Code.MigraDoc.Styles.DefineContentSection(document);
            document.LastSection.AddParagraph("处置前受影响用户明细表(以地表计,共" + items3.Count + "块)", "Heading1");
            Tables.DemonstrateSimpleTable(document, with3, itemNames3, items3);
            string filename = DateTime.Now.ToString("yyyyMMddHHmmss");
            var renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            //EZFontResolver fontResolver = EZFontResolver.Get;
            //GlobalFontSettings.FontResolver = fontResolver;
            //fontResolver.AddFont("微软雅黑", XFontStyle.Regular, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\Content\fonts\msyh.ttf", true, true);            // Get the predefined style Normal.
            renderer.RenderDocument();
            renderer.PdfDocument.Save(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\file\pdf\" + HeadStr + filename + ".pdf");
            string path = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/file/pdf/" + HeadStr + filename + ".pdf";
            return Content(path);
        }
        public ActionResult GetAfterUserInfo(int? planId)
        {
            TimeSpan time = new TimeSpan(DateTime.Now.Hour, 0, 0);
            var aui4 = dbcontext.Database.SqlQuery<BSB_LinkedTableEntity>("SELECT * FROM BSB_LinkedTable WHERE ElementID in(SELECT ElementID FROM RESULT_JUNCTION_TJWCC " +
                "WHERE Result_Pressure_3<0.18 AND Is_Active=1)").ToList();     //调度后影响地表信息

            var dpe = dpApp.GetListById(planId);
            string HeadStr = "";
            if (dpe.DispatchOrder.IndexOf("-,-,-") > 0)
            {
                HeadStr = "水厂停产调度后影响用户";
            }
            else
            {
                HeadStr = "水厂减产调度后影响用户";
            }
            JArray items4 = new JArray();
            if (aui4 != null)
            {
                foreach (var ctmp in aui4)
                {
                    JArray tmp1 = new JArray();
                    tmp1.Add(ctmp.USERNAME);
                    tmp1.Add(ctmp.Address);
                    tmp1.Add(ctmp.Label);
                    items4.Add(tmp1);
                }
            }
            double[] with3 = new double[] { 2.5, 7.2, 3.3 };
            string[] itemNames3 = new string[] { "名称", "地址", "营销公司" };
            var document = new Document();
            document.Info.Title = "SAMBO";
            document.Info.Subject = "SAMBO";
            document.Info.Author = "SAMBO";

            Code.MigraDoc.Styles.DefineStyles(document);
            Cover.DefineCover(document);
            Code.MigraDoc.Styles.DefineContentSection(document);
            document.LastSection.AddParagraph("处置后受影响用户明细表(以地表计,共" + items4.Count + "块)", "Heading1");
            Tables.DemonstrateSimpleTable(document, with3, itemNames3, items4);
            string filename = DateTime.Now.ToString("yyyyMMddHHmmss");
            var renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\file\pdf\" + HeadStr + filename + ".pdf");
            string path = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/file/pdf/" + HeadStr + filename + ".pdf";
            //dpApp.SubmitForm(dpe, planId);
            //var dpList = dpApp.GetList();
            //JArray result = new JArray
            //{
            //    JArray.FromObject(dpList),
            //    path
            //};
            return Content(path);
        }
        public class AffectUserInfo
        {
            public int? Count { get; set; }
            public decimal? Area { get; set; }
        }
    }
}