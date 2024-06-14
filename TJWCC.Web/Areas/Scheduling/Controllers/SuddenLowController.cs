using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.BSSys;
using TJWCC.Application.WCC;
using TJWCC.Code;
using TJWCC.Code.MigraDoc;
using TJWCC.Domain.Entity.WCC;

namespace TJWCC.Web.Areas.Scheduling.Controllers
{
    public class SuddenLowController : ControllerBase
    {
        private BSB_Warn_RecordsApp bwbApp = new BSB_Warn_RecordsApp();
        private CC_DispatchPlanApp dpApp = new CC_DispatchPlanApp();
        // 压力突降: Scheduling/SuddenLow


        [HandlerAjaxOnly]
        public ActionResult GetAlarmGridJson()
        {
            var data = bwbApp.GetSuddLow(null);
            return Content(data.ToJson());
        }
        public JsonResult GetPointArea(int elementId, int? aId, string cnbValves)
        {
            var nowDate = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
            CC_DispatchPlanEntity cdp = null;
            string calStationList = "";
            string[] accoTo = new string[] { null, "0" };
            if (aId != null)
            {
                var dorder = bwbApp.GetSuddLow(aId).FirstOrDefault();
                cdp = dpApp.GetLowPlan(dorder.ElementId + "," + dorder.StationKey, dorder.StartTime.Value);
                if (cdp != null)
                {
                    accoTo = cdp.AccordingTo.Split('|');//故障监测点信息和扩大计算次数
                    calStationList = accoTo[0];
                    if (string.IsNullOrWhiteSpace(cnbValves))
                    {
                        cnbValves = accoTo[1] + "|" + cdp.Effect;
                    }
                    else
                    {
                        var tcnbValve = cnbValves.Split('|');
                        cdp.Effect = tcnbValve[1] + "|" + tcnbValve[2];
                        cdp.AccordingTo = accoTo[0] + "|" + tcnbValve[0];
                        dpApp.SubmitForm(cdp, aId);
                        accoTo[1] = tcnbValve[0];
                    }
                }
            }
            ArrayList tmpValves = new ArrayList();
            if (!string.IsNullOrWhiteSpace(cnbValves))
            {
                if (!string.IsNullOrWhiteSpace(cnbValves.Split('|')[1]))
                    tmpValves = new ArrayList(Array.ConvertAll(cnbValves.Split('|')[1].Split(','), int.Parse));
            }
            TubeBurstAnalysisApp tba = new TubeBurstAnalysisApp();
            ArrayList result = tba.IsolateNode(elementId, 55, tmpValves);
            result.Add(accoTo[1]);

            string[] itemNames = new string[] { "编号","名称","设备状态","报警时间","营销公司" };
            JArray items = new JArray();
            var ctmps = calStationList.Split(';');
            foreach (var ctmp in ctmps)
            {
                var ctmpis = ctmp.Split(',');
                JArray tmp = new JArray();
                int index = 0;
                foreach (var ctmpi in ctmpis)
                {
                    if(index>0) tmp.Add(ctmpi);
                    index++;
                }
                JArray tmp1 = new JArray();
                items.Add(tmp);
            }
            string[] itemNames1 = new string[] { "序号","地址", "口径(mm)" };
            double[] with = new double[] { 1.5, 2, 3.3 };
            double[] with1 = new double[] { 1.3, 8, 3.3 };
            var tmpre = JArray.FromObject(((result[4] as ArrayList)[1] as ArrayList)[2].ToString().ToJson());
            JArray items1 = new JArray();
            for (int i = 0; i < tmpre.Count; i++)
            {
                var tmp = JObject.FromObject(tmpre[i]);
                JArray tmpitem = new JArray
                {
                    i+1,
                    tmp.GetValue("Physical_Address").ToString(),
                    tmp.GetValue("Physical_PipeDiameter").ToInt()
                };
                items1.Add(tmpitem);
            }
            var document = new Document();
            document.Info.Title = "SAMBO";
            document.Info.Subject = "SAMBO";
            document.Info.Author = "SAMBO";

            Code.MigraDoc.Styles.DefineStyles(document);
            Cover.DefineCover(document);
            Code.MigraDoc.Styles.DefineContentSection(document);
            document.LastSection.AddParagraph("事件监测点信息", "Heading1");
            Tables.DemonstrateSimpleTable(document, with, itemNames, items);
            if (accoTo[1].Equals("0"))
                document.LastSection.AddParagraph("排查区域：首次计算", "Heading2");
            else
                document.LastSection.AddParagraph("排查区域：第" + accoTo[1] + "次扩大计算", "Heading2");
            document.LastSection.AddParagraph("区域管道明细", "Heading2");
            Tables.DemonstrateSimpleTable(document, with1, itemNames1, items1);
            var renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            //EZFontResolver fontResolver = EZFontResolver.Get;
            //GlobalFontSettings.FontResolver = fontResolver;
            //fontResolver.AddFont("微软雅黑", XFontStyle.Regular, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\Content\fonts\msyh.ttf", true, true);            // Get the predefined style Normal.
            renderer.RenderDocument();
            renderer.PdfDocument.Save(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\file\pdf\压力突降" + nowDate.ToString("yyyyMMddHHmmss") + ".pdf");
            string path = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/file/pdf/压力突降" + nowDate.ToString("yyyyMMddHHmmss") + ".pdf";
            result.Add(path);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult PointCloseValve(int eId)
        {
            PointCloseValveApp pcv = new PointCloseValveApp();
            pcv.IsolateNode(eId, 55);

            return Success("计算完成！");
        }
    }
}