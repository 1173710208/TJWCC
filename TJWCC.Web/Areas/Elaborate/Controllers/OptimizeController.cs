using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.BSSys;
using TJWCC.Application.SystemManage;
using TJWCC.Application.WCC;
using TJWCC.Code;
using TJWCC.Data;
using TJWCC.Domain.Entity.WCC;
using TJWCC.Domain.ViewModel;

namespace TJWCC.Web.Areas.Elaborate.Controllers
{
    public class OptimizeController : ControllerBase
    {
        log4net.ILog loger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        TJWCCDbContext dbcontext = new TJWCCDbContext();
        private CC_DispatchPlanApp dpApp = new CC_DispatchPlanApp();
        private BSO_PumpParamApp bppApp = new BSO_PumpParamApp();
        private BSO_PumpStationApp bpsApp = new BSO_PumpStationApp();
        private SYS_DICApp sdApp = new SYS_DICApp();
        private BS_SCADA_TAG_CURRENTApp bstcapp = new BS_SCADA_TAG_CURRENTApp();
        private BSO_Station_RecordsApp bsrapp = new BSO_Station_RecordsApp();
        private BSO_Station_CurrentApp bscapp = new BSO_Station_CurrentApp();
        private BSO_PumpOptimal_RecordsApp bprapp = new BSO_PumpOptimal_RecordsApp();
        // 泵站优化调度: Elaborate/Optimize


        public ActionResult GetPumpList()
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            var sds = sdApp.GetItemList(10);
            foreach (var sd in sds)
            {
                GetDataType_Result rdr = new GetDataType_Result()
                {
                    Name = sd.ItemName,
                    Value = sd.ItemID.ToString()
                };
                list.Add(rdr);
            }
            return Content(list.ToJson());
        }


        public ActionResult GetPumpInfo(int? psId)
        {
            JArray result = new JArray();
            var bpp = bppApp.GetList(psId);
            result.Add(JArray.FromObject(bpp));
            return Content(result.ToJson());
        }

        public ActionResult GetPumpParam(int? psId)
        {
            JArray result = new JArray();
            JArray tmp = new JArray();
            string sql = "SELECT a.*,b.IsActive FROM BSO_PumpCurrent a,BSO_PumpParam b WHERE a.PumpId=b.PumpId AND StationId=" + psId + " ORDER BY a.LocalID";
            var bsc = dbcontext.Database.SqlQuery<PumpCurrent_Result>(sql).ToList();
            result.Add(JArray.FromObject(bsc));
            switch (psId)
            {
                case 2://芥园泵房
                    if (true)
                    {
                        var bss = bstcapp.GetList("10");
                        //result.Add(bss.Select(i=>i.GYStationId.Equals("10")));
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("401") && i.Number == 3).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//7#清水库
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("411") && i.Number == 2).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//南吸水井液位
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("411") && i.Number == 1).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//北吸水井液位
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("117") && i.Number == 1).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//一干压力
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 1).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//一干流量
                        tmp.Add(null);//一干累计流量
                        tmp.Add(null);//二干压力
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 2).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//二干流量
                        tmp.Add(null);//一干累计流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("117") && i.Number == 2).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//三干压力
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 3).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//三干流量
                        tmp.Add(null);//三干累计流量
                    }
                    break;
                case 3://凌庄泵房
                    if (true)
                    {
                        var bss = bstcapp.GetList("11");
                        //result.Add(bss.Select(i=>i.GYStationId.Equals("10")));
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("401") && i.Number == 2).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//液位
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("401") && i.Number == 3).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//液位
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("401") && i.Number == 4).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//液位
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("401") && i.Number == 5).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//液位
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 5).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//五干流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("112") && i.Number == 5).Select(i => i.Tag_value).FirstOrDefault().ToDecimal(0));//五干累计流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 6).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//六干流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("112") && i.Number == 6).Select(i => i.Tag_value).FirstOrDefault().ToDecimal(0));//六干累计流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 4).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//四干流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("112") && i.Number == 4).Select(i => i.Tag_value).FirstOrDefault().ToDecimal(0));//四干累计流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 3).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//三干流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("112") && i.Number == 3).Select(i => i.Tag_value).FirstOrDefault().ToDecimal(0));//三干累计流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 2).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//二干流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("112") && i.Number == 2).Select(i => i.Tag_value).FirstOrDefault().ToDecimal(0));//二干累计流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 1).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//一干流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("112") && i.Number == 1).Select(i => i.Tag_value).FirstOrDefault().ToDecimal(0));//一干累计流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("117") && i.Number == 1).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//1压力
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("117") && i.Number == 2).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//2压力
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("117") && i.Number == 3).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//3压力
                    }
                    break;
                case 4://新开河泵房
                    if (true)
                    {
                        var bss = bstcapp.GetList("12");
                        //result.Add(bss.Select(i=>i.GYStationId.Equals("10")));
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("401") && i.Number == 1).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//液位
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("401") && i.Number == 2).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//液位
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("411") && i.Number == 3).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//液位
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("411") && i.Number == 4).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//液位
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("117") && i.Number == 3).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//一干压力
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 2).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//一干流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("112") && i.Number == 14).Select(i => i.Tag_value).FirstOrDefault().ToDecimal(0));//一干累计流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("117") && i.Number == 2).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//二干压力
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 3).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//二干流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("112") && i.Number == 15).Select(i => i.Tag_value).FirstOrDefault().ToDecimal(0));//二干累计流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("117") && i.Number == 4).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//三干压力
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 4).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//三干流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("112") && i.Number == 16).Select(i => i.Tag_value).FirstOrDefault().ToDecimal(0));//三干累计流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("131") && i.Number == 14).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//连通管流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("133") && i.Number == 1).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//连通管正向流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("134") && i.Number == 1).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//连通管反向流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("117") && i.Number == 1).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//四干压力
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 1).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//四干流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("112") && i.Number == 17).Select(i => i.Tag_value).FirstOrDefault().ToDecimal(0));//四干累计流量
                    }
                    break;
                case 5://津滨泵房
                    if (true)
                    {
                        var bss = bstcapp.GetList("13");
                        //result.Add(bss.Select(i=>i.GYStationId.Equals("10")));
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("400") && i.Number == 4).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//吸水井液位2
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("400") && i.Number == 3).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//吸水井液位1
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("117") && i.Number == 6).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//六干压力
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 1).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//六干流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("112") && i.Number == 1).Select(i => i.Tag_value).FirstOrDefault().ToDecimal(0));//六干累计流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("117") && i.Number == 5).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//五干压力
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 2).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//五干流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("112") && i.Number == 2).Select(i => i.Tag_value).FirstOrDefault().ToDecimal(0));//五干累计流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("117") && i.Number == 4).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//四干压力
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 3).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//四干流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("112") && i.Number == 3).Select(i => i.Tag_value).FirstOrDefault().ToDecimal(0));//四干累计流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("117") && i.Number == 3).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//三干压力
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 2).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//三干流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("112") && i.Number == 2).Select(i => i.Tag_value).FirstOrDefault().ToDecimal(0));//三干累计流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("117") && i.Number == 2).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//二干压力
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 5).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//二干流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("112") && i.Number == 5).Select(i => i.Tag_value).FirstOrDefault().ToDecimal(0));//二干累计流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("117") && i.Number == 1).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//一干压力
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("111") && i.Number == 6).Select(i => i.Tag_value).FirstOrDefault().ToDecimal());//一干流量
                        tmp.Add(bss.Where(i => i.Tag_key.Equals("112") && i.Number == 6).Select(i => i.Tag_value).FirstOrDefault().ToDecimal(0));//一干累计流量
                    }
                    break;
            }
            result.Add(tmp);
            Random rd = new Random();
            result.Add(Convert.ToDecimal(rd.Next(160, 310)));
            result.Add(Convert.ToDecimal(rd.Next(170, 320)));
            return Content(result.ToJson());
        }
        /// <summary>
        /// 修改泵是否可用
        /// </summary>
        /// <param name="pumpId"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>

        public ActionResult SetPumpActive(int? pumpId, bool isActive)
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
            DbCommand cmd = db.Database.Connection.CreateCommand();
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "UPDATE [dbo].[BSO_PumpParam] SET IsActive="+ (isActive ? 1 : 0) + " WHERE PumpId=" + pumpId;
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
            return Success("修改成功");
        }


        public ActionResult CreateOptimizePlan(int? psId)
        {
            var nowDate = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
            var dutyId = Convert.ToInt32(OperatorProvider.Provider.GetCurrent().DutyId);//班次
            var bscdata = dpApp.GetNewPlan(1).DataDate.Value.AddHours(1);
            string error = "";
            //using (Process proc = new Process())
            //{
            //    proc.StartInfo.WorkingDirectory = "";
            //    proc.StartInfo.FileName = @"D:\sambo\PumpMain.exe";
            //    proc.StartInfo.Arguments = "";
            //    proc.StartInfo.RedirectStandardError = true;
            //    proc.StartInfo.UseShellExecute = false;
            //    proc.StartInfo.CreateNoWindow = false;
            //    proc.Start();
            //    //error = proc.StandardError.ReadToEnd();
            //    proc.WaitForExit();
            //}
            string pathvar = Environment.GetEnvironmentVariable("PATH");
            try
            {
                Process p = new Process();
                p.StartInfo.EnvironmentVariables["PATH"] = @"%SystemRoot%\system32;%SystemRoot%;%SystemRoot%\System32\Wbem;%SYSTEMROOT%\System32\WindowsPowerShell\v1.0\;C:\Program Files (x86)\Microsoft SQL Server\110\Tools\Binn\ManagementStudio\;C:\Program Files (x86)\Microsoft SQL Server\110\Tools\Binn\;C:\Program Files\Microsoft SQL Server\110\Tools\Binn\;C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\PrivateAssemblies\;C:\Program Files (x86)\Microsoft SQL Server\110\DTS\Binn\;C:\Program Files\Microsoft SQL Server\110\DTS\Binn\;C:\Users\Administrator\AppData\Local\Programs\Python\Python39\Scripts;C:\Users\Administrator\AppData\Local\Programs\Python\Python39;C:\Program Files\MATLAB\R2014a\runtime\win64;C:\Program Files\MATLAB\R2014a\bin;C:\Program Files\MATLAB\R2014a\polyspace\bin;C:\Program Files\MATLAB\MATLAB Compiler Runtime\v84\runtime\win64";
                //p.StartInfo.EnvironmentVariables["RAYPATH"] = "test";
                //p.StartInfo.FileName = Server.MapPath("~/bin/PumpMain.exe").Replace("\\", "/");
                p.StartInfo.FileName = @"D:\sambo\PumpMain.exe";
                p.StartInfo.Arguments = "";
                p.StartInfo.UseShellExecute = false;
                //p.StartInfo.RedirectStandardOutput = true;
                //p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardError = true;
                //p.StartInfo.CreateNoWindow = true;
                p.Start();
                error = p.StandardError.ReadToEnd();
                loger.Error(error);
                //p.BeginOutputReadLine();
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            var newOptimize = bsrapp.GetNew(psId.Value);
            JArray result = new JArray();
            JArray plr = new JArray();
            string pumpList = "";
            //var pList = bppApp.GetCreateList(psId);
            string sql = "SELECT * FROM BSO_PumpCurrent WHERE PumpId in(SELECT PumpId FROM BSO_PumpParam WHERE StationId=" + psId + " AND IsActive=1) ORDER BY LocalID";
            var pList = dbcontext.Database.SqlQuery<PumpCurrent_Result>(sql).ToList();
            var press = newOptimize.Pressure1;
            var newbprs = bprapp.GetNew(psId.Value);
            var newbprdate = newbprs.Max(i => i.CreateTime);
            foreach (var pl in pList)
            {
                var newbpr = newbprs.Where(i => i.LocalId == pl.LocalID && i.CreateTime == newbprdate).FirstOrDefault();
                //if (psId == 5)
                //{
                var open = newbpr != null ? newbpr.OpenClose.Value > 0 : false;
                var IsChange = !(open == pl.IsOpen.Value);
                //var flow = newOptimize.GetType().GetProperty("Flow" + pl.LocalID.ToString()).GetValue(newOptimize, null).ToDecimalOrNull();
                PumpCurrent_Result pcr = new PumpCurrent_Result
                {
                    PumpId = pl.PumpId,
                    PumpName = pl.PumpName,
                    IsOpen = open,
                    SpeedRatio = open ? newbpr.SpeedRate.ToDecimalOrNull(2) : null,
                    Press = open ? (press / 100m).ToDecimalOrNull(3) : null,
                    Flow = open ? newbpr.Flow.ToDecimalOrNull(0) : null,
                    Frequency = open ? newbpr.Efficiency * 100m : null,
                    IsChange = IsChange
                };
                plr.Add(JObject.FromObject(pcr));
                JArray tmppls = new JArray
                        {
                            pcr.PumpId,
                            pcr.PumpName,
                            pcr.IsOpen,
                            pcr.SpeedRatio,
                            pcr.Press,
                            pcr.Flow,
                            pcr.Frequency,
                            IsChange
                        };
                pumpList += string.Join(",", tmppls.ToObject<List<string>>().ToArray()) + ";";
                //}
                //else
                //{
                //    Random rd = new Random();
                //    PumpCurrent_Result pcr = new PumpCurrent_Result
                //    {
                //        PumpId = pl.PumpId,
                //        PumpName = pl.PumpName,
                //        IsOpen = Convert.ToBoolean(rd.Next(0, 2) > 0),
                //        SpeedRatio = Convert.ToDecimal(rd.Next(600, 950)) / 1000m,
                //        Press = Convert.ToDecimal(rd.Next(260, 310)) / 1000m,
                //        Flow = Convert.ToDecimal(rd.Next(800, 3950)),
                //        Frequency = Convert.ToDecimal(rd.Next(600, 950)) / 10m
                //    };
                //    plr.Add(JObject.FromObject(pcr));
                //    System.Threading.Thread.Sleep(100);
                //    JArray tmppls = new JArray
                //{
                //    pcr.PumpId,
                //    pcr.PumpName,
                //    pcr.IsOpen,
                //    pcr.SpeedRatio,
                //    pcr.Press,
                //    pcr.Flow,
                //    pcr.Frequency
                //};
                //    pumpList += string.Join(",", tmppls.ToObject<List<string>>().ToArray()) + ";";
                //}
            }
            pumpList = pumpList.Substring(0, pumpList.Length - 1);
            string stationName = bpsApp.GetSingleFeeRecord(psId.Value).StationName;
            decimal bscPress = (bscapp.GetPress(psId) / 100m).ToDecimal(3);
            decimal bscFlow = bscapp.GetFlow(psId).ToDecimal(0);
            CC_DispatchPlanEntity dpe = new CC_DispatchPlanEntity
            {
                DispatchType = 7,
                DataDate = nowDate,
                CreateDate = nowDate,
                AccordingTo = pumpList,
                DispatchOrder = pumpList,
                Status = stationName + ":已生成",
                Effect = bscPress.ToString() + ";" + bscFlow.ToString() + ";" + bscdata.ToDateTimeString(),
                Operator = OperatorProvider.Provider.GetCurrent().UserName,
                IsPlanBase = 0,
                Shift = dutyId
            };
            dpApp.SubmitForm(dpe, null);
            JArray tmpbsc = new JArray();
            tmpbsc.Add(bscPress);
            tmpbsc.Add(bscFlow);
            tmpbsc.Add(bscdata.ToDateTimeString());
            result.Add(plr);
            result.Add(JObject.FromObject(dpApp.GetNewPlan(7)));
            result.Add(tmpbsc);
            return Content(result.ToJson());
        }


        public ActionResult CreatePumpOrder(int? planId)
        {
            TimeSpan time = new TimeSpan(DateTime.Now.Hour, 0, 0);
            var dpe = dpApp.GetListById(planId);
            string stationName = dpe.Status.Split(':')[0];
            dpe.IssueDate = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
            dpe.Status = stationName + ":已下发";
            dpApp.SubmitForm(dpe, planId);
            var dpList = dpApp.GetList();
            return Success("下发成功");
        }
    }
}