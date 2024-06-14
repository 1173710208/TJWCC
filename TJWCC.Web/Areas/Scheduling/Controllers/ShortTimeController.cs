using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using Newtonsoft.Json.Linq;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.BSSys;
using TJWCC.Application.SystemManage;
using TJWCC.Code;
using TJWCC.Code.MigraDoc;
using TJWCC.Data;
using TJWCC.Domain.ViewModel;

namespace TJWCC.Web.Areas.Scheduling.Controllers
{
    public class ShortTimeController : ControllerBase
    {
        TJWCCDbContext dbcontext = new TJWCCDbContext();
        private BS_SCADA_TAG_CURRENTApp bstcapp = new BS_SCADA_TAG_CURRENTApp();
        private DistrictClassApp DistrictClass = new DistrictClassApp();
        private SYS_DICApp sysDicApp = new SYS_DICApp();
        // 短期预测: Scheduling/ShortTime

        public ActionResult GetAreaList()
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            GetDataType_Result rdr = new GetDataType_Result()
            {
                Name = "市区",
                Value = "0"
            };
            list.Add(rdr);
            var sds = sysDicApp.GetItemList(15);
            foreach (var sd in sds)
            {
                rdr = new GetDataType_Result()
                {
                    Name = sd.ItemName,
                    Value = sd.ItemID.ToString()
                };
                list.Add(rdr);
            }
            var dists = DistrictClass.GetList().OrderBy(item => item.ID).ToList();
            foreach (var dist in dists)
            {
                GetDataType_Result tmp = new GetDataType_Result()
                {
                    Name = dist.District,
                    Value = dist.ID.ToString()
                };
                list.Add(tmp);
            }
            return Content(list.ToJson());
        }
        
        public JsonResult DemandPrediction(int areaId, string type,int? value)
        {
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            if (false)
            {
                //dt = new DateTime(_now.Year, _now.Month, _now.Day, _now.Hour, 0, 0);
                var tmpdt = bstcapp.GetList().Where(item => new string[] { "8", "9" }.Contains(item.Tag_key)).Max(item => item.Save_date);
                if (tmpdt != null)
                {
                    dt = tmpdt.Value;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                }
            }
            DateTime startDatetime = dt;
            DateTime endDatetime = dt;

            decimal _maxJll = decimal.MinValue, _minJll = decimal.MaxValue;
            double _maxErr = double.MinValue, _minErr = double.MaxValue;
            ArrayList _obDateValue = new ArrayList();
            ArrayList _ycDateValue = new ArrayList();//预测水量
            ArrayList ycDateValue = new ArrayList();//预测未来水量
            ArrayList _ycErr = new ArrayList();//预测误差率
            ArrayList result = new ArrayList();
            switch (type)
            {
                case "1":
                    startDatetime = new DateTime(dt.AddDays(-2).Year, dt.AddDays(-2).Month, dt.AddDays(-2).Day, dt.AddDays(-2).Hour, 0, 0);
                    endDatetime = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                    break;
                case "2":
                    startDatetime = new DateTime(dt.AddDays(-15).Year, dt.AddDays(-15).Month, dt.AddDays(-15).Day, 0, 0, 0);
                    endDatetime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0).AddSeconds(-1);
                    break;
                case "3":
                    startDatetime = new DateTime(dt.AddDays(-150).Year, dt.AddDays(-150).Month, dt.AddDays(-150).Day, 0, 0, 0);
                    dt = dt.AddDays(0 - Convert.ToInt16(dt.DayOfWeek));
                    endDatetime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0).AddSeconds(-1);
                    break;
                case "4":
                    startDatetime = new DateTime(dt.AddDays(-185).Year, dt.AddDays(-185).Month, dt.AddDays(-185).Day, 0, 0, 0);
                    endDatetime = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0).AddSeconds(-1);
                    break;
                case "5":
                    startDatetime = new DateTime(dt.AddDays(-150).Year, dt.AddDays(-150).Month, dt.AddDays(-150).Day, 0, 0, 0);
                    endDatetime = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0).AddSeconds(-1);
                    break;
                case "6":
                    startDatetime = new DateTime(dt.AddYears(-5).Year, dt.AddYears(-5).Month, 1, 0, 0, 0);
                    endDatetime = new DateTime(dt.Year, 1, 1, 0, 0, 0).AddSeconds(-1);
                    break;
            }
            string sql = "SELECT FlowTime Save_date,DMA Sort_key,Sort_value,ForecastFlow,DMAGrade,Type FROM (SELECT * FROM [dbo].[BSB_ForecastFlow] WHERE DMA=" + areaId + " AND MoreAlert=" + type
                 + " AND FlowTime>'" + startDatetime + "' AND FlowTime<='" + endDatetime + "') b LEFT JOIN (SELECT * FROM [dbo].[CC_FlowSort] WHERE Sort_key=" + areaId + " AND Type=" + type
                + " AND Save_date>='" + startDatetime + "' AND Save_date<'" + endDatetime + "') a ON a.Sort_key=b.DMA AND a.Save_date=b.FlowTime";
            List<GetForecastFlow_Result> arrlist = dbcontext.Database.SqlQuery<GetForecastFlow_Result>(sql).OrderBy(i=>i.Save_date).ToList();
            for (int i = 0; i < arrlist.Count; i++)
            {
                ArrayList _obtmpDV = new ArrayList();
                decimal? tmpJll = arrlist[i].Sort_value;
                if (type.Equals("1"))
                    _obtmpDV.Add(arrlist[i].Save_date.Value.ToString("MM-dd HH:mm"));//date
                else
                    _obtmpDV.Add(arrlist[i].Save_date.Value.ToString("MM-dd"));//date
                _obtmpDV.Add(tmpJll == null ? null : tmpJll.Value.ToString("#0.00"));//value
                _obDateValue.Add(_obtmpDV);
                if (i == 0 && tmpJll != null)
                {
                    _maxJll = tmpJll.Value;
                    _minJll = tmpJll.Value;
                }
                if (tmpJll > _maxJll && tmpJll != null)
                {
                    _maxJll = tmpJll.Value;
                }
                if (tmpJll < _minJll && tmpJll != null)
                {
                    _minJll = tmpJll.Value;
                }

                _obtmpDV = new ArrayList();
                tmpJll = arrlist[i].ForecastFlow;
                if (type.Equals("1"))
                    _obtmpDV.Add(arrlist[i].Save_date.Value.ToString("MM-dd HH:mm"));//date
                else
                    _obtmpDV.Add(arrlist[i].Save_date.Value.ToString("MM-dd"));//date
                _obtmpDV.Add(tmpJll == null ? null : tmpJll.Value.ToString("#0.00"));//value
                _ycDateValue.Add(_obtmpDV);
                if (tmpJll > _maxJll && tmpJll != null)
                {
                    _maxJll = tmpJll.Value;
                }
                if (tmpJll < _minJll && tmpJll != null)
                {
                    _minJll = tmpJll.Value;
                }
                double tmpErr = 0;
                for (int j = 0; j < _obDateValue.Count; j++)
                {
                    string tmpdata = "";
                    if (type.Equals("1"))
                        tmpdata = arrlist[i].Save_date.Value.ToString("MM-dd HH:mm");//date
                    else
                        tmpdata = arrlist[i].Save_date.Value.ToString("MM-dd");//date
                    if (tmpdata.Equals((_obDateValue[j] as ArrayList)[0].ToString()))
                    {
                        if (Convert.ToDouble((_obDateValue[j] as ArrayList)[1]) == 0) tmpErr = 0;
                        else tmpErr = Math.Abs(Convert.ToDouble(tmpJll) - Convert.ToDouble((_obDateValue[j] as ArrayList)[1])) / Math.Abs(Convert.ToDouble((_obDateValue[j] as ArrayList)[1])) * 100;
                        ArrayList _yctmpErr = new ArrayList();
                        if (type.Equals("1"))
                            _yctmpErr.Add(arrlist[i].Save_date.Value.ToString("MM-dd HH:mm"));//date
                        else
                            _yctmpErr.Add(arrlist[i].Save_date.Value.ToString("MM-dd"));//date
                        _yctmpErr.Add(Math.Round(tmpErr, 2));//value
                        _ycErr.Add(_yctmpErr);
                    }
                }
                if (tmpErr > _maxErr)
                {
                    _maxErr = tmpErr;
                }
                if (tmpErr < _minErr)
                {
                    _minErr = tmpErr;
                }
            }
            if (type.Equals("2"))
                sql = "SELECT FlowTime Save_date,DMA Sort_key,ForecastFlow,DMAGrade,MoreAlert Type FROM [dbo].[BSB_ForecastFlow] WHERE DMA=" + areaId
                    + " AND MoreAlert=" + type + " AND FlowTime>'" + endDatetime + "' AND FlowTime<'" + endDatetime.AddDays(value.Value) + "'";
            else
                sql = "SELECT FlowTime Save_date,DMA Sort_key,ForecastFlow,DMAGrade,MoreAlert Type FROM [dbo].[BSB_ForecastFlow] WHERE DMA=" + areaId
                    + " AND MoreAlert=" + type + " AND FlowTime>'" + endDatetime + "'";
            arrlist = dbcontext.Database.SqlQuery<GetForecastFlow_Result>(sql).OrderBy(i => i.Save_date).ToList();
            for (int i = 0; i < arrlist.Count; i++)
            {
                ArrayList _obtmpDV = new ArrayList();
                decimal? tmpJll = arrlist[i].ForecastFlow;
                if (type.Equals("1"))
                    _obtmpDV.Add(arrlist[i].Save_date.Value.ToString("MM-dd HH:mm"));//date
                else
                    _obtmpDV.Add(arrlist[i].Save_date.Value.ToString("MM-dd"));//date
                _obtmpDV.Add(tmpJll == null ? null : tmpJll.Value.ToString("#0.00"));//value
                ycDateValue.Add(_obtmpDV);
                if (tmpJll > _maxJll && tmpJll != null)
                {
                    _maxJll = tmpJll.Value;
                }
                if (tmpJll < _minJll && tmpJll != null)
                {
                    _minJll = tmpJll.Value;
                }
            }
            //for (int i = 0; i < arrlist.Count; i++)
            //{
            //    ArrayList _obtmpDV = new ArrayList();
            //    //_obDate.Add(nowday_h1.Ticks / 10000);//date
            //}
            result.Add(_obDateValue);
            result.Add(_ycDateValue);
            result.Add(_ycErr);
            result.Add(ycDateValue);
            result.Add(Convert.ToInt64(_maxJll == decimal.MinValue ? 0m : (_maxJll + _maxJll * 0.01m)));//最大流量
            //result.Add(Convert.ToInt64(_minJll == decimal.MaxValue ? 0m : (_minJll - _minJll * 0.1m)));//最小流量
            result.Add(0m);//最小流量
            result.Add(Math.Round(_maxErr == double.MinValue ? 0.0 : (_maxErr * 2.5), 2));//最大误差
            result.Add(Math.Round(_minErr == double.MaxValue ? 0.0 : _minErr, 2));//最小误差

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = ((time.Ticks - startTime.Ticks) / 10000) + 28800000;   //除10000调整为13位      
            return t;
        }
        public string CreatePDF(int areaId, string name, int type,double? gRate, string base64Info)
        {
            DateTime nowDate = DateTime.Now;
            DateTime dt = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, nowDate.Hour, 0, 0);
            if (!false)
            {
                //dt = new DateTime(_now.Year, _now.Month, _now.Day, _now.Hour, 0, 0);
                var tmpdt = bstcapp.GetList().Where(item => new string[] { "8", "9" }.Contains(item.Tag_key)).Max(item => item.Save_date);
                if (tmpdt != null)
                {
                    dt = tmpdt.Value;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                }
            }
            DateTime startDatetime = dt;
            DateTime endDatetime = dt;
            string HeadStr = "";
            string sqldate = "";
            byte[] buffer = Convert.FromBase64String(base64Info.Replace(" ", "+").Split(',')[1]);
            //保存到指定路径
            string fullPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\file\image\" + nowDate.ToString("yyyyMMddHHmmss") + ".png";
            FileStream fileStream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)
            {
                Position = 0L
            };
            fileStream.Write(buffer, 0, buffer.Length);
            fileStream.Close();

            switch (type)
            {
                case 1:
                    HeadStr = name + "时需水量预测报告";
                    sqldate = "CONVERT(VARCHAR(20),FlowTime,20)";
                    startDatetime = new DateTime(dt.AddDays(-2).Year, dt.AddDays(-2).Month, dt.AddDays(-2).Day, dt.AddDays(-2).Hour, 0, 0);
                    endDatetime = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                    break;
                case 2:
                    HeadStr = name + "日需水量预测报告";
                    sqldate = "CONVERT(VARCHAR(10),FlowTime,20)";
                    startDatetime = new DateTime(dt.AddDays(-15).Year, dt.AddDays(-15).Month, dt.AddDays(-15).Day, 0, 0, 0);
                    endDatetime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0).AddSeconds(-1);
                    break;
                case 3:
                    HeadStr = name + "周需水量预测报告";
                    sqldate = "CONVERT(VARCHAR(10),FlowTime,20)";
                    startDatetime = new DateTime(dt.AddDays(-150).Year, dt.AddDays(-150).Month, dt.AddDays(-150).Day, 0, 0, 0);
                    dt = dt.AddDays(0 - Convert.ToInt16(dt.DayOfWeek));
                    endDatetime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0).AddSeconds(-1);
                    break;
                case 4:
                    HeadStr = name + "月需水量预测报告";
                    sqldate = "CONVERT(VARCHAR(7),FlowTime,20)";
                    startDatetime = new DateTime(dt.AddDays(-185).Year, dt.AddDays(-185).Month, dt.AddDays(-185).Day, 0, 0, 0);
                    endDatetime = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0).AddSeconds(-1);
                    break;
                case 5:
                    HeadStr = name + "季度需水量预测报告";
                    sqldate = "CONVERT(VARCHAR(7),FlowTime,20)";
                    startDatetime = new DateTime(dt.AddMonths(-7).Year, dt.AddMonths(-7).Month, dt.AddMonths(-7).Day, 0, 0, 0);
                    endDatetime = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0).AddSeconds(-1);
                    break;
                case 6:
                    HeadStr = name + "年需水量预测报告";
                    sqldate = "CONVERT(VARCHAR(4),FlowTime,20)+'年'";
                    startDatetime = new DateTime(dt.AddYears(-5).Year, dt.AddYears(-5).Month, 1, 0, 0, 0);
                    endDatetime = new DateTime(dt.Year, 1, 1, 0, 0, 0).AddSeconds(-1);
                    break;
                default:
                    HeadStr = name + "日需水量预测报告";
                    sqldate = "CONVERT(VARCHAR(10),FlowTime,20)";
                    startDatetime = new DateTime(dt.AddDays(-15).Year, dt.AddDays(-15).Month, dt.AddDays(-15).Day, 0, 0, 0);
                    endDatetime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0).AddSeconds(-1);
                    break;
            }
            string sql = "SELECT " + sqldate + " '时间',(CASE WHEN DMA=0 THEN '全市' ELSE (CASE WHEN DMA<20 THEN (SELECT District FROM DistrictClass WHERE ID=DMA) ELSE (SELECT Label FROM RESERVOIR " +
                "WHERE ElementId=DMA) END) END) '区域',CONVERT(int,Sort_value) '实际水量(m³)',CONVERT(int,ForecastFlow) '预测水量(m³)' FROM (SELECT FlowTime,DMA,ForecastFlow FROM [BSB_ForecastFlow] " +
                "WHERE DMA=" + areaId + " AND MoreAlert=" + type + " AND FlowTime>'" + startDatetime + "') b LEFT JOIN (SELECT * FROM [dbo].[CC_FlowSort] WHERE Sort_key=" + areaId + " " +
                "AND Type=" + type + " AND Save_date>='" + startDatetime + "' AND Save_date<'" + endDatetime + "') a ON a.Sort_key=b.DMA AND a.Save_date=b.FlowTime ORDER BY FlowTime";
            DbCommand cmd = dbcontext.Database.Connection.CreateCommand();
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            //转成sqlcommand正常查询语句。
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd as SqlCommand;
            DataTable tmp1 = new DataTable();
            da.Fill(tmp1);
            double[] with = new double[] { 3.5, 2, 3 };
            double[] with1 = new double[] { 3.5, 2.7, 2.7 };
            double[] with2 = new double[] { 3.2, 3.2 };

            var document = new Document();
            document.Info.Title = "SAMBO";
            document.Info.Subject = "SAMBO";
            document.Info.Author = "SAMBO";

            Code.MigraDoc.Styles.DefineStyles(document);
            Cover.DefineCover(document);
            Code.MigraDoc.Styles.DefineContentSection(document);
            document.LastSection.AddParagraph(HeadStr, "Heading1");
            var image = document.LastSection.AddImage(fullPath);
            image.LockAspectRatio = true;
            image.Width = 630;
            image.Left = -85;
            document.LastSection.AddParagraph("数据明细", "Heading2");
            Tables.DemonstrateSimpleTable(document, with, tmp1);
            if (type == 6)
            {
                double flow1 = 0;
                double flow2 = 0;
                if (gRate == null)
                {
                    flow1 = tmp1.Rows[tmp1.Rows.Count - 2][3].ToDouble();
                    flow2 = tmp1.Rows[tmp1.Rows.Count - 1][3].ToDouble();
                }
                else
                {
                    string[] itemNames = new string[] { DateTime.Now.Year + "年水量(m³)", (DateTime.Now.Year+1) + "年水量(m³)" };
                    flow1 = tmp1.Rows[tmp1.Rows.Count - 3][2].ToDouble() * (1 + gRate.Value / 100.0);
                    flow2 = flow1 * (1 + gRate.Value / 100.0);
                    document.LastSection.AddParagraph("修正水量", "Heading2");
                    Tables.DemonstrateSimpleTable(document, with2, itemNames, new JArray { new JArray { flow1.ToInt() , flow2.ToInt() } });
                }
                JArray tmp2 = new JArray();
                JArray tmp3 = new JArray();
                switch (areaId)
                {
                    case 0:
                        {
                            string[] itemNames = new string[] { "月份", "芥园水量(m³)", "凌庄水量(m³)", "新开河水量(m³)", "津滨水量(m³)" };
                            string[] month = new string[] { "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月" };
                            double[] rate = new double[] { 21.79, 24.00, 33.16, 21.04 };
                            double[] jyrate = new double[] { 8.54, 7.29, 8.46, 8.22, 8.61, 8.33, 8.53, 8.52, 8.33, 8.67, 8.23, 8.27 };
                            double[] lzrate = new double[] { 8.6, 7.05, 8.34, 8.05, 8.9, 8.78, 8.5, 8.54, 8.53, 8.21, 8.18, 8.32 };
                            double[] xkhrate = new double[] { 8.56, 7, 8.29, 8.09, 8.68, 8.66, 8.5, 8.58, 8.3, 8.46, 8.27, 8.61 };
                            double[] jbrate = new double[] { 8.41, 6.98, 8.49, 8.29, 9.34, 8.98, 8.79, 8.78, 8.06, 8.33, 7.74, 7.81 };
                            for (int i = 0; i < month.Length; i++)
                            {
                                JArray ss = new JArray();
                                ss.Add(month[i]);
                                ss.Add((flow1 * (rate[0] / 100.0) * (jyrate[i] / 100.0)).ToInt());
                                ss.Add((flow1 * (rate[1] / 100.0) * (lzrate[i] / 100.0)).ToInt());
                                ss.Add((flow1 * (rate[2] / 100.0) * (xkhrate[i] / 100.0)).ToInt());
                                ss.Add((flow1 * (rate[3] / 100.0) * (jbrate[i] / 100.0)).ToInt());
                                tmp2.Add(ss);
                                ss = new JArray();
                                ss.Add(month[i]);
                                ss.Add((flow2 * (rate[0] / 100.0) * (jyrate[i] / 100.0)).ToInt());
                                ss.Add((flow2 * (rate[1] / 100.0) * (lzrate[i] / 100.0)).ToInt());
                                ss.Add((flow2 * (rate[2] / 100.0) * (xkhrate[i] / 100.0)).ToInt());
                                ss.Add((flow2 * (rate[3] / 100.0) * (jbrate[i] / 100.0)).ToInt());
                                tmp3.Add(ss);
                            }
                            document.LastSection.AddParagraph(DateTime.Now.Year + "年预测水量", "Heading2");
                            Tables.DemonstrateSimpleTable(document, with1, itemNames, tmp2);
                            document.LastSection.AddParagraph((DateTime.Now.Year + 1) + "年测水量", "Heading2");
                            Tables.DemonstrateSimpleTable(document, with1, itemNames, tmp3);
                        }
                        break;
                    case 512538:
                        {
                            string[] itemNames = new string[] { "月份", "芥园水量(m³)" };
                            string[] month = new string[] { "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月" };
                            double[] rate = new double[] { 21.79, 24.00, 33.16, 21.04 };
                            double[] jyrate = new double[] { 8.54, 7.29, 8.46, 8.22, 8.61, 8.33, 8.53, 8.52, 8.33, 8.67, 8.23, 8.27 };
                            for (int i = 0; i < month.Length; i++)
                            {
                                JArray ss = new JArray();
                                ss.Add(month[i]);
                                ss.Add((flow1 * (jyrate[i] / 100.0)).ToInt());
                                tmp2.Add(ss);
                                ss = new JArray();
                                ss.Add(month[i]);
                                ss.Add((flow2 * (jyrate[i] / 100.0)).ToInt());
                                tmp3.Add(ss);
                            }
                            document.LastSection.AddParagraph(DateTime.Now.Year + "年预测水量", "Heading2");
                            Tables.DemonstrateSimpleTable(document, with1, itemNames, tmp2);
                            document.LastSection.AddParagraph((DateTime.Now.Year + 1) + "年测水量", "Heading2");
                            Tables.DemonstrateSimpleTable(document, with1, itemNames, tmp3);
                        }
                        break;
                    case 511686:
                        {
                            string[] itemNames = new string[] { "月份", "凌庄水量(m³)" };
                            string[] month = new string[] { "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月" };
                            double[] lzrate = new double[] { 8.6, 7.05, 8.34, 8.05, 8.9, 8.78, 8.5, 8.54, 8.53, 8.21, 8.18, 8.32 };
                            for (int i = 0; i < month.Length; i++)
                            {
                                JArray ss = new JArray();
                                ss.Add(month[i]);
                                ss.Add((flow1 * (lzrate[i] / 100.0)).ToInt());
                                tmp2.Add(ss);
                                ss = new JArray();
                                ss.Add(month[i]);
                                ss.Add((flow2 * (lzrate[i] / 100.0)).ToInt());
                                tmp3.Add(ss);
                            }
                            document.LastSection.AddParagraph(DateTime.Now.Year + "年预测水量", "Heading2");
                            Tables.DemonstrateSimpleTable(document, with1, itemNames, tmp2);
                            document.LastSection.AddParagraph((DateTime.Now.Year + 1) + "年测水量", "Heading2");
                            Tables.DemonstrateSimpleTable(document, with1, itemNames, tmp3);
                        }
                        break;
                    case 511707:
                        {
                            string[] itemNames = new string[] { "月份",  "新开河水量(m³)"};
                            string[] month = new string[] { "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月" };
                            double[] xkhrate = new double[] { 8.56, 7, 8.29, 8.09, 8.68, 8.66, 8.5, 8.58, 8.3, 8.46, 8.27, 8.61 };
                            for (int i = 0; i < month.Length; i++)
                            {
                                JArray ss = new JArray();
                                ss.Add(month[i]);
                                ss.Add((flow1 * (xkhrate[i] / 100.0)).ToInt());
                                tmp2.Add(ss);
                                ss = new JArray();
                                ss.Add(month[i]);
                                ss.Add((flow2 * (xkhrate[i] / 100.0)).ToInt());
                                tmp3.Add(ss);
                            }
                            document.LastSection.AddParagraph(DateTime.Now.Year + "年预测水量", "Heading2");
                            Tables.DemonstrateSimpleTable(document, with1, itemNames, tmp2);
                            document.LastSection.AddParagraph((DateTime.Now.Year + 1) + "年测水量", "Heading2");
                            Tables.DemonstrateSimpleTable(document, with1, itemNames, tmp3);
                        }
                        break;
                    case 511215:
                        {
                            string[] itemNames = new string[] { "月份",  "津滨水量(m³)" };
                            string[] month = new string[] { "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月" };
                            double[] rate = new double[] { 21.79, 24.00, 33.16, 21.04 };
                            double[] jbrate = new double[] { 8.41, 6.98, 8.49, 8.29, 9.34, 8.98, 8.79, 8.78, 8.06, 8.33, 7.74, 7.81 };
                            for (int i = 0; i < month.Length; i++)
                            {
                                JArray ss = new JArray();
                                ss.Add(month[i]);
                                ss.Add((flow1 * (jbrate[i] / 100.0)).ToInt());
                                tmp2.Add(ss);
                                ss = new JArray();
                                ss.Add(month[i]);
                                ss.Add((flow2 * (jbrate[i] / 100.0)).ToInt());
                                tmp3.Add(ss);
                            }
                            document.LastSection.AddParagraph(DateTime.Now.Year + "年预测水量", "Heading2");
                            Tables.DemonstrateSimpleTable(document, with1, itemNames, tmp2);
                            document.LastSection.AddParagraph((DateTime.Now.Year + 1) + "年测水量", "Heading2");
                            Tables.DemonstrateSimpleTable(document, with1, itemNames, tmp3);
                        }
                        break;
                }
            }
            var renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\file\pdf\yc" + nowDate.ToString("yyyyMMddHHmmss") + ".pdf");
            string path = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/file/pdf/yc" + nowDate.ToString("yyyyMMddHHmmss") + ".pdf";
            return path;
            //byte[] fileContents = null;
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    doc.Save(stream, true);
            //    fileContents = stream.ToArray();
            //}
        }
    }
}