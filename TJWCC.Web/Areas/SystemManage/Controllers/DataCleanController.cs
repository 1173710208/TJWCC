using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.BSSys;
using TJWCC.Application.SystemManage;
using TJWCC.Application.WCC;
using TJWCC.Code;
using TJWCC.Data;
using TJWCC.Domain.Entity.WCC;
using TJWCC.Domain.ViewModel;

namespace TJWCC.Web.Areas.SystemManage.Controllers
{
    public class DataCleanController : ControllerBase
    {
        private SYS_DICApp sysDicApp = new SYS_DICApp();
        private CC_CleanStatisticsApp app = new CC_CleanStatisticsApp();
        private BS_SCADA_TAG_CURRENTApp bstcapp = new BS_SCADA_TAG_CURRENTApp();
        private BS_SCADA_TAG_HISApp bsthapp = new BS_SCADA_TAG_HISApp();
        private BS_SCADA_TAG_INFOApp scadaInfoApp = new BS_SCADA_TAG_INFOApp();
        private BSM_Meter_InfoApp bmiapp = new BSM_Meter_InfoApp();
        // 数据清洗: SystemManage/DataClean
        
        public string DataClean(string keyword)
        {
            string error = "";
            using (Process proc = new Process())
            {
                //proc.StartInfo.WorkingDirectory = "";
                //proc.StartInfo.FileName = "E:/调度升级/TJWCC/TJWCC.Web/bin/data_clean.exe";
                proc.StartInfo.WorkingDirectory = "";
                proc.StartInfo.FileName = Server.MapPath("~/bin/data_clean.exe").Replace("\\", "/");
                proc.StartInfo.Arguments = "";
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                error = proc.StandardError.ReadToEnd();
                proc.WaitForExit();
            }
            return error;
        }
        /// <summary>
        /// 获取数据清洗中统计周期下拉菜单列表
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetCycleList()
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            var sds = sysDicApp.GetItemList(8);
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
        /// <summary>
        /// 获取数据清洗中数据类型下拉菜单列表
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetDataTypeList(int? areaId)
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            if (areaId != null)
            {
                var sds = bmiapp.GetList(areaId, null).OrderBy(i => i.Explain);
                foreach (var sd in sds)
                {
                    GetDataType_Result rdr = new GetDataType_Result()
                    {
                        Name = sd.Explain,
                        Value = sd.Meter_Type.ToString()
                    };
                    list.Add(rdr);
                }
            }
            return Content(list.ToJson());
        }
        /// <summary>
        /// 获取数据清洗中监测点下拉菜单列表
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetPointList(int? areaId, int? type)
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            if (areaId != null)
            {
                var sds = bmiapp.GetList(areaId, type, null, null).OrderBy(i => i.Meter_Name);
                foreach (var sd in sds)
                {
                    GetDataType_Result rdr = new GetDataType_Result()
                    {
                        Name = sd.Meter_Name,
                        Value = areaId + "_" + sd.Station_Key + "_" + type + "_" + sd.DistrictAreaId
                    };
                    list.Add(rdr);
                }
            }
            return Content(list.ToJson());
        }
        public ActionResult GetCleanStatistics(int? type, int? cycle, DateTime? sDate, DateTime? eDate)
        {
            JArray result = new JArray();
            if (type == null && cycle == null && sDate == null && eDate == null)
            {
                sDate = Convert.ToDateTime(DateTime.Now.ToDateString()).AddSeconds(-1);
                JArray zong = new JArray();
                JArray qing = new JArray();
                JArray time = new JArray();
                var lists = app.GetList(0, 1, sDate, eDate).OrderBy(i => i.CountTime);
                var tmp = lists.GroupBy(i => i.CountTime.ToString("HH:mm")).Select(i => new { CountTime = i.Key, Totality = i.Sum(j => j.Totality), Clean = i.Sum(j => j.Clean) }).ToArray();
                foreach (var li in tmp)
                {
                    zong.Add(li.Totality);
                    qing.Add(li.Clean);
                    time.Add(li.CountTime);
                }
                if (lists.Count() == 0)//没有数据时生成随机数
                {
                    int sz = 0, sq = 0;
                    for(int i = 0; i < DateTime.Now.Hour; i++)
                    {
                        Random rd = new Random();
                        int tsz = rd.Next(500, 700);
                        int tsq = rd.Next(20, 70);
                        System.Threading.Thread.Sleep(100);
                        sz += tsz;
                        sq += tsq;
                        zong.Add(tsz);
                        qing.Add(tsq);
                        time.Add((i+1)+":00");

                    }
                    result.Add(zong);
                    result.Add(qing);
                    result.Add(time);
                    result.Add(sz);
                    result.Add(sq);
                    JArray tmps = new JArray();
                    int tmpt = Convert.ToInt32(sz * 0.54);
                    int tmpc = Convert.ToInt32(sq * 0.54);
                    tmps.Add(JObject.FromObject(new { TypeName = "原水", Totality = tmpt, Clean = tmpc }));
                    tmpt = Convert.ToInt32(sz * 0.16);
                    tmpc = Convert.ToInt32(sq * 0.17);
                    tmps.Add(JObject.FromObject(new { TypeName = "水厂", Totality = tmpt, Clean = tmpc }));
                    tmpt = Convert.ToInt32(sz * 0.3);
                    tmpc = Convert.ToInt32(sq * 0.29);
                    tmps.Add(JObject.FromObject(new { TypeName = "管网", Totality = tmpt, Clean = tmpc }));
                    result.Add(JArray.FromObject(tmps));
                    return Content(result.ToJson());
                }
                result.Add(zong);
                result.Add(qing);
                result.Add(time);
                result.Add(lists.Select(i => i.Totality).Sum());
                result.Add(lists.Select(i => i.Clean).Sum());
                var ss = lists.GroupBy(i => i.TypeName).Select(i => new { TypeName = i.Key, Totality = i.Sum(j => j.Totality), Clean = i.Sum(j => j.Clean) }).ToArray();
                result.Add(JArray.FromObject(ss));
                return Content(result.ToJson());
            }
            else
            {
                JArray zong = new JArray();
                JArray ztmp1 = new JArray();
                JArray ztmp2 = new JArray();
                JArray ztmp3 = new JArray();
                JArray time = new JArray();
                int index = -1;
                if(eDate!=null)
                    switch (cycle)
                    {
                        case 1:
                            eDate = eDate.Value.AddDays(1).AddSeconds(-1);
                            break;
                        case 2:
                            eDate = eDate.Value.AddMonths(1).AddSeconds(-1);
                            break;
                    }
                var lists = app.GetList(type, cycle, sDate, eDate).OrderBy(i => i.CountTime);
                List<CC_CleanStatisticsEntity> lists1 = null;
                if (sDate != null && eDate != null)
                {
                    lists1 = app.GetList(type, cycle, sDate.Value.AddYears(-1), eDate.Value.AddYears(-1)).OrderBy(i => i.CountTime).ToList();
                }
                var tmp = lists.GroupBy(i => i.CountTime).Select(i => new { CountTime = i.Key, Totality = i.Sum(j => j.Totality), Normal = i.Sum(j => j.Normal) }).ToArray();
                foreach (var li in tmp)
                {
                    ztmp1.Add((Convert.ToDouble(li.Normal) / Convert.ToDouble(li.Totality) * 100.0).ToString("0.##"));
                    var tmpct = li.CountTime.AddYears(-1);
                    var tmp1 = lists1 == null ? null : lists1.Where(i => i.CreateTime == tmpct).GroupBy(i => i.CountTime).Select(i => new { CountTime = i.Key, Totality = i.Sum(j => j.Totality), Normal = i.Sum(j => j.Normal) }).FirstOrDefault();
                    if (tmp1 != null)
                    {
                        double ab1 = Convert.ToDouble((Convert.ToDouble(li.Normal) / Convert.ToDouble(li.Totality) * 100.0).ToString("0.##"));
                        double ab2 = Convert.ToDouble((Convert.ToDouble(tmp1.Normal) / Convert.ToDouble(tmp1.Totality) * 100.0).ToString("0.##"));
                        ztmp2.Add(((ab1 - ab2)/ ab2 * 100.0).ToString("0.##"));
                    }
                    else ztmp2.Add(null);
                    if (index < tmp.Length && index >= 0)
                    {
                        var tmli = tmp[index];
                        double aa1 = Convert.ToDouble((Convert.ToDouble(li.Normal) / Convert.ToDouble(li.Totality) * 100.0).ToString("0.##"));
                        double aa2 = Convert.ToDouble((Convert.ToDouble(tmli.Normal) / Convert.ToDouble(tmli.Totality) * 100.0).ToString("0.##"));
                        ztmp3.Add(((aa1 - aa2)/ aa2 * 100.0).ToString("0.##"));
                    }
                    else ztmp3.Add(null);
                    switch (cycle)
                    {
                        case 1:
                            time.Add(li.CountTime.ToDateTimeString());
                            break;
                        case 2:
                            time.Add(li.CountTime.ToDateString());
                            break;
                        case 3:
                            time.Add(li.CountTime.ToString("yyyy-MM"));
                            break;
                    }
                    index++;
                }
                zong.Add(ztmp1);
                zong.Add(ztmp2);
                zong.Add(ztmp3);
                result.Add(zong);
                result.Add(time);
                var ss = lists.GroupBy(i => i.TypeName).Select(i => new { TypeName = i.Key, Totality = i.Sum(j => j.Totality), Clean = i.Sum(j => j.Clean) }).ToArray();
                result.Add(JArray.FromObject(ss));
                result.Add(lists.Select(i => i.Totality).Sum());
                result.Add(lists.Select(i => i.Normal).Sum());
                result.Add(lists.Select(i => i.Clean).Sum());
                return Content(result.ToJson());
            }
        }
        
        public ActionResult GetCleanData(Pagination pagination, string sks, DateTime? sDate, DateTime? eDate)
        {
            var data = new
            {
                rows = bsthapp.GetList(pagination, sks, sDate, eDate),
                pagination.total,
                pagination.page,
                pagination.records
            };
            return Content(data.ToJson());
        }
        
        public ActionResult GetPointData(string sk, DateTime saveDate)
        {
            JArray result = new JArray();
            JArray yuan = new JArray();
            JArray qing = new JArray();
            JArray time = new JArray();
            DateTime sDate = saveDate.AddDays(-2);
            DateTime eDate = saveDate.AddDays(2);
            //string bmi = sk.Split(',')[0].Split('_')[2].ToString();
            var bmi = bmiapp.GetList(sk).Select(i => i.Meter_Type).FirstOrDefault().ToString();
            var rows = bsthapp.GetList(sk, sDate, eDate);
            foreach (var row in rows)
            {
                yuan.Add(row.Tag_value);
                qing.Add(row.CleanedValue == null ? row.Tag_value : row.CleanedValue);
                time.Add(row.Save_date.ToDateTimeString(true));
            }
            int tcvs = rows.Count;
            var ssi = scadaInfoApp.GetEntity(bmi);
            result.Add(yuan);
            result.Add(qing);
            result.Add(time);
            result.Add(ssi.Tag_name + "(" + ssi.Units + ")");
            result.Add(tcvs == 0 ? 50 : (60.0 / tcvs.ToDouble() * 100.0).ToInt());
            return Content(result.ToJson());
        }
        
        public ActionResult GetDownloadJson(string sks, DateTime? sDate, DateTime? eDate)
        {
            string path = bsthapp.GetDownload(sks, sDate, eDate);
            //StringBuilder sbScript = new StringBuilder();
            //sbScript.Append("<script type='text/javascript'>$.loading(false);</script>");
            return Content("http://" + Request.Url.Host + ":" + Request.Url.Port + path);
        }
    }
}