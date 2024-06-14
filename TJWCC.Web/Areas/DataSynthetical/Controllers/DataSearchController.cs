using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.BSSys;
using TJWCC.Application.SystemManage;
using TJWCC.Code;
using TJWCC.Data;
using TJWCC.Domain.Entity.BSSys;
using TJWCC.Domain.ViewModel;

namespace TJWCC.Web.Areas.DataSynthetical.Controllers
{
    public class DataSearchController : ControllerBase
    {
        TJWCCDbContext dbcontext = new TJWCCDbContext();
        private BSM_Meter_InfoApp bmiApp = new BSM_Meter_InfoApp();
        private BS_SCADA_TAG_HISApp bsthapp = new BS_SCADA_TAG_HISApp();
        private BS_SCADA_TAG_INFOApp scadaInfoApp = new BS_SCADA_TAG_INFOApp();
        private BS_SCADA_TAG_CURRENTApp bstcapp = new BS_SCADA_TAG_CURRENTApp();
        private SYS_DICApp sysDicApp = new SYS_DICApp();
        // 数据查询: DataSynthetical/DataSearch
        public ActionResult GetMeterInfoList(int itemId)
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            var result = bmiApp.GetListByMType(itemId);
            foreach (var bmi in result)
            {
                GetDataType_Result rdr = new GetDataType_Result()
                {
                    Name = bmi.Meter_Name,
                    Value = bmi.Station_Key.ToString()
                };
                list.Add(rdr);
            }
            return Content(list.ToJson());
        }
        public ActionResult GetSPList()
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            var sds = sysDicApp.GetItemList(9);
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
        public ActionResult GetSWaterList()
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            var sds = sysDicApp.Get5ItemList(52);
            foreach (var sd in sds)
            {
                GetDataType_Result rdr = new GetDataType_Result()
                {
                    Name = sd.ItemName,
                    Value = sd.ItemKey
                };
                list.Add(rdr);
            }
            return Content(list.ToJson());
        }
        public ActionResult GetCycleList()
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            var sds = sysDicApp.GetItemList(13);//图表分析统计周期
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
        public ActionResult GetHistoryData(Pagination pagination, string sTKey, decimal? max, decimal? min, DateTime? startDate, DateTime? endDate, TimeSpan? startTime, TimeSpan? endTime)
        {
            JArray items = new JArray();
            JArray item = new JArray();
            JArray unit = new JArray();
            List<string> name = new List<string>();
            BS_SCADA_TAG_HISEntity[] dataHiss = bsthapp.GetANDList(pagination, sTKey, max, min, startDate, endDate, startTime, endTime).ToArray();
            //foreach (var dataHis in dataHiss)
            //{
            //    JObject dataItems = new JObject();
            //    JArray dataItem = new JArray();
            //    if (!name.Contains(dataHis.Station_key))
            //    {
            //        string tmpUnit = scadaInfoApp.GetEntity(dataHis.Tag_key.ToString()).Units;
            //        unit.Add(tmpUnit);
            //        name.Add(dataHis.Station_key);
            //        if (item.Count > 0)
            //            items.Add(item);
            //        item = new JArray();
            //    }
            //    dataItem.Add(dataHis.Save_date.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            //    if ("1".Equals(dataHis.Tag_key.ToString()))
            //        dataItem.Add(dataHis.Tag_value.Value.ToString("F3"));
            //    else
            //        dataItem.Add(dataHis.Tag_value.Value.ToString("F2"));
            //    dataItems.Add("value", dataItem);
            //    item.Add(dataItems);
            //}
            //JArray result = new JArray();
            //result.Add(items);
            //result.Add(name.ToArray());
            //result.Add(unit);
            var data = new
            {
                rows = dataHiss,
                pagination.total,
                pagination.page,
                pagination.records
            };
            return Content(data.ToJson());
        }
        public ActionResult GetSumValue(string sTKey, decimal? max, decimal? min, DateTime? startDate, DateTime? endDate, TimeSpan? startTime, TimeSpan? endTime)
        {
            int type = bmiApp.GetList(sTKey).Select(i => i.Meter_Type).FirstOrDefault().ToInt();
            string value = bsthapp.GetSumValue(type, sTKey, max, min, startDate, endDate, startTime, endTime);
            return Content(value);
        }
        public ActionResult GetAvgValue(string sTKey, decimal? max, decimal? min, DateTime? startDate, DateTime? endDate, TimeSpan? startTime, TimeSpan? endTime)
        {
            int type = bmiApp.GetList(sTKey).Select(i => i.Meter_Type).FirstOrDefault().ToInt();
            string value = bsthapp.GetAvgValue(type, sTKey, max, min, startDate, endDate, startTime, endTime);
            return Content(value);
        }
        public ActionResult GetMaxValue(string sTKey, decimal? max, decimal? min, DateTime? startDate, DateTime? endDate, TimeSpan? startTime, TimeSpan? endTime)
        {
            int type = bmiApp.GetList(sTKey).Select(i => i.Meter_Type).FirstOrDefault().ToInt();
            string value = bsthapp.GetMaxValue(type, sTKey, max, min, startDate, endDate, startTime, endTime);
            return Content(value);
        }
        public ActionResult GetMinValue(string sTKey, decimal? max, decimal? min, DateTime? startDate, DateTime? endDate, TimeSpan? startTime, TimeSpan? endTime)
        {
            int type = bmiApp.GetList(sTKey).Select(i => i.Meter_Type).FirstOrDefault().ToInt();
            string value = bsthapp.GetMinValue(type, sTKey, max, min, startDate, endDate, startTime, endTime);
            return Content(value);
        }
        public ActionResult GetDownloadJson(string sTKey, decimal? max, decimal? min, DateTime? startDate, DateTime? endDate, TimeSpan? startTime, TimeSpan? endTime)
        {
            int type = bmiApp.GetList(sTKey).Select(i => i.Meter_Type).FirstOrDefault().ToInt();
            string path = bsthapp.GetANDDownload(type, sTKey, max, min, startDate, endDate, startTime, endTime);
            //StringBuilder sbScript = new StringBuilder();
            //sbScript.Append("<script type='text/javascript'>$.loading(false);</script>");
            return Content("http://" + Request.Url.Host + ":" + Request.Url.Port + path);
        }
        /// <summary>
        /// 获取数据类型下拉菜单列表
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetDataTypeList(int? areaId)
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            if (areaId != null)
            {
                var bmis = bmiApp.GetList(areaId, null);
                var sds = sysDicApp.GetItemList(14).Select(i => i.ItemID.ToString()).ToArray();
                foreach (var sd in bmis.Where(i => sds.Contains("" + i.Meter_Type)).OrderBy(i => i.Meter_Type))
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
        /// 获取监测点下拉菜单列表
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetPointList(int? areaId, int? type, int? sPoint, int? sWater)
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            if (areaId != null)
            {

                var bmis = bmiApp.GetList(areaId, type, sPoint, sWater);
                var sds = sysDicApp.GetItemList(14).Select(i => i.ItemID.ToString()).ToArray();
                foreach (var sd in bmis.Where(i => sds.Contains("" + i.Meter_Type)))
                {
                    GetDataType_Result rdr = new GetDataType_Result()
                    {
                        Name = sd.Meter_Name,
                        Value = sd.Station_Key
                    };
                    list.Add(rdr);
                }
            }
            return Content(list.ToJson());
        }
        public ActionResult GetFlowRateData(string areaId, int? type, DateTime? startDate, DateTime? endDate)
        {
            DateTime nowDate = DateTime.Now;
            int bmi = type == null ? 1 : type.Value;
            //var bmi = bmiApp.GetList(areaId).Select(i => i.Meter_Type).FirstOrDefault().ToInt();
            if (string.IsNullOrEmpty(areaId)) bmi = 1;
            JArray name = JArray.FromObject(sysDicApp.GetItemList(14).Where(i => i.ItemID == bmi).Select(i => i.ItemKey).FirstOrDefault().Split(','));
            JArray result = new JArray();
            JArray time = new JArray();
            JArray bds = new JArray();
            JArray areaIds = JArray.FromObject(areaId.Split(','));
            int tcvs = 0;
            List<ChartView> cv = new List<ChartView>();
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            var tmpdt = bstcapp.GetList().Where(t => t.Tag_key.Equals(bmi.ToString())).Max(item => item.Save_date);
            if (tmpdt != null)
            {
                dt = tmpdt.Value;
                dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
            }
            if (startDate == null)
            {
                startDate = dt.AddDays(-1);
            }
            if (endDate == null)
            {
                endDate = dt;
            }
            if (name[0].ToString().Equals("all"))
            {
                if (areaIds.Count > 5)
                {
                    int index = 0;
                    int count = 0;
                    while (count < 5)
                    {
                        index = areaId.IndexOf(',', index + 1);
                        count++;
                    }
                    areaId = areaId.Substring(0, index);
                    areaIds = JArray.FromObject(areaId.Split(','));
                }
                name = JArray.FromObject(areaId.Split(','));
                areaId = areaId.Replace(",", "','");
                string sql = "SELECT CONVERT(DECIMAL(18,2),(CASE WHEN b.co=0 THEN 0 ELSE a.cc/b.co END)*100) Rate,a.Station_Key,(SELECT TOP 1 Meter_Name FROM BSM_Meter_Info m WHERE m.Station_Key =a.Station_Key AND " +
                    "Meter_Type=" + bmi + ") Name,a.Save_date+':00:00' Save_date FROM(SELECT SUM(ABS(Tag_value)) cc,Station_key,CONVERT(VARCHAR(13),Save_date,20) Save_date FROM [dbo].[BS_SCADA_TAG_HIS] WHERE Station_key " +
                    "IN('" + areaId + "') AND Tag_key=" + bmi + " AND Save_date>='" + startDate + "' AND Save_date<'" + endDate.Value.AddDays(1) + "' GROUP BY CONVERT(VARCHAR(13),Save_date,20),Station_key) a LEFT " +
                    "JOIN (SELECT * FROM(SELECT SUM(ABS(Tag_value)) co,CONVERT(VARCHAR(13),Save_date,20) Save_date FROM [dbo].[BS_SCADA_TAG_HIS] WHERE Station_key IN('" + areaId + "') AND Tag_key=" + bmi + " AND " +
                    "Save_date>='" + startDate + "' AND Save_date<'" + endDate.Value.AddDays(1) + "' GROUP BY CONVERT(VARCHAR(13),Save_date,20)) s WHERE co!=0) b ON a.Save_date=b.Save_date ORDER BY a.Save_date";
                dbcontext.Database.CommandTimeout = int.MaxValue;
                cv = dbcontext.Database.SqlQuery<ChartView>(sql).ToList();
            }
            else
            {
                if (name.Count > 0)
                {
                    areaId = areaId.Replace(",", "','");
                    areaIds = JArray.FromObject(sysDicApp.GetItemList(14).Where(i => i.ItemID == bmi).Select(i => i.ItemKey).FirstOrDefault().Split(','));
                    string select = "SELECT CONVERT(DECIMAL(18,2),CONVERT(DECIMAL,a.Rate)/CONVERT(DECIMAL,b.co)*100) Rate,a.Station_Key,a.Save_date+':00:00' Save_date,a.Station_Key Name FROM(";
                    string sql = "";
                    areaIds.Add(areaIds[areaIds.Count - 1].ToString());
                    name.Add(name[name.Count - 1].ToString());
                    for (int i = 0; i < areaIds.Count; i++)
                    {
                        if (i == 0)
                        {
                            sql = "SELECT COUNT(*) Rate,CONVERT(VARCHAR(13),Save_date,20) Save_date,'" + areaIds[i].ToString() + "' Station_Key FROM [dbo].[BS_SCADA_TAG_HIS] WHERE " +
                                "Tag_value<" + areaIds[i].ToString() + " AND Station_key IN('" + areaId + "') AND Tag_key=" + bmi + " AND Save_date>='" + startDate + "' AND Save_date<'" + 
                                endDate.Value.AddDays(1) + "' GROUP BY CONVERT(VARCHAR(13),Save_date,20)";
                            name[i] = "＜" + areaIds[i].ToString();
                        }
                        else
                        {
                            if (i == areaIds.Count - 1)
                            {
                                sql += " UNION ALL (SELECT COUNT(*) c5,CONVERT(VARCHAR(13),Save_date,20) Save_date,'≥" + areaIds[i].ToString() + "' Station_Key FROM [dbo].[BS_SCADA_TAG_HIS] " +
                                    "WHERE Tag_value>=" + areaIds[i].ToString() + " AND Station_key IN('" + areaId + "') AND Tag_key=" + bmi + " AND Save_date>='" + startDate + "' AND Save_date<'" + 
                                    endDate.Value.AddDays(1) + "' GROUP BY CONVERT(VARCHAR(13),Save_date,20))) a LEFT JOIN (SELECT * FROM(SELECT COUNT(*) co,CONVERT(VARCHAR(13),Save_date,20) " +
                                    "Save_date FROM [dbo].[BS_SCADA_TAG_HIS] WHERE Station_key IN('" + areaId + "') AND Tag_key=" + bmi + " AND Save_date>='" + startDate + "' AND Save_date<'" + 
                                    endDate.Value.AddDays(1) + "' GROUP BY CONVERT(VARCHAR(13),Save_date,20)) s WHERE co!=0) b ON a.Save_date=b.Save_date ORDER BY a.Save_date";
                                name[i] = "≥" + areaIds[i].ToString();
                                areaIds[i] = "≥" + areaIds[i].ToString();
                            }
                            else
                            {

                                sql += " UNION ALL (SELECT COUNT(*) Rate,CONVERT(VARCHAR(13),Save_date,20) Save_date,'" + areaIds[i].ToString() + "' Station_Key FROM [dbo].[BS_SCADA_TAG_HIS] " +
                                    "WHERE Tag_value>=" + areaIds[i - 1].ToString() + " AND Tag_value<" + areaIds[i].ToString() + " AND Station_key IN('" + areaId + "') AND " +
                                    "Tag_key=" + bmi + " AND Save_date>='" + startDate + "' AND Save_date<'" + endDate.Value.AddDays(1) + "' GROUP BY CONVERT(VARCHAR(13),Save_date,20))";
                                name[i] = areaIds[i - 1].ToString() + "-" + areaIds[i].ToString();
                            }
                        }
                    }
                    dbcontext.Database.CommandTimeout = int.MaxValue;
                    cv = dbcontext.Database.SqlQuery<ChartView>(select + sql).ToList();
                }
            }
            if (cv.Count == 0 && string.IsNullOrEmpty(areaId))
            {
                name = new JArray();
                name.Add("＜0.20");
                name.Add("0.20-0.24");
                name.Add("0.24-0.30");
                name.Add("≥0.30");
            }
            JArray[] data = new JArray[name.Count];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new JArray();
            }
            if (cv.Count == 0 && string.IsNullOrEmpty(areaId))
            {
                for (int i = 0; i < 21; i++)
                {
                    JArray bd = new JArray();
                    int tol = 0;
                    for (var x = 0; x < data.Length; x++)
                    {
                        Random rd = new Random();
                        int rd1 = rd.Next(100, 700);
                        data[x].Add(rd1);
                        JObject rdo = new JObject();
                        rdo.Add("value", rd1.ToDecimal());
                        rdo.Add("name", name[x]);
                        tol += rd1;
                        System.Threading.Thread.Sleep(50);
                        bd.Add(rdo);
                    }
                    for (var x = 0; x < data.Length; x++)
                    {
                        JObject rdo = (JObject)bd[x];
                        var tmpv = (rdo.GetValue("value").ToDecimal() / tol.ToDecimal() * 100.0m).ToDecimal(2);
                        rdo["value"] = tmpv;
                        bd[x] = rdo;
                        data[x][data[x].Count - 1] = tmpv;
                    }
                    time.Add(nowDate.AddHours(-40 + i).ToDateString() + " " + nowDate.AddHours(-40 + i).Hour + ":00:00");
                    bds.Add(bd);
                }
            }
            else
            {
                var tmpcvs = cv.Select(y => y.Save_date).Distinct().ToList();
                tcvs = tmpcvs.Count;
                foreach (var tmpcv in tmpcvs)
                {
                    JArray bd = new JArray();
                    var tmcvs = cv.Where(y => y.Save_date == tmpcv).ToList();
                    for (var x = 0; x < areaIds.Count; x++)
                    {
                        var tmcv = tmcvs.Where(y => y.Station_Key.Equals(areaIds[x].ToString())).FirstOrDefault();
                        data[x].Add(tmcv?.Rate.ToDecimal());
                        if (name[x].Equals(areaIds[x]) && tmcv != null) name[x] = tmcv.Name;
                        JObject rdo = new JObject();
                        rdo.Add("value", tmcv?.Rate.ToDecimal());
                        rdo.Add("name", tmcv == null ? null : name[x]);
                        bd.Add(rdo);
                    }
                    time.Add(tmpcv);
                    bds.Add(bd);
                }
            }
            JArray tmp = new JArray();
            tmp.Add(data);
            result.Add(tmp);
            result.Add(time);
            result.Add(name);
            result.Add(bds);
            result.Add(tcvs == 0 ? 100 : (30.0 / tcvs.ToDouble() * 100.0).ToInt());
            return Content(result.ToJson());
        }
        public ActionResult GetFlowData(string stationKey, int? type, DateTime? startDate, DateTime? endDate)
        {
            DateTime nowDate = DateTime.Now;
            int mType = 8;
            if (!string.IsNullOrWhiteSpace(stationKey))
            {
                mType = stationKey.Split(',')[0].Split('_')[2].ToInt();
            }
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            var tmpdt = bstcapp.GetList().Where(t => t.Tag_key.Equals(mType.ToString())).Max(item => item.Save_date);
            if (tmpdt != null)
            {
                dt = tmpdt.Value;
                dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
            }
            if (startDate == null)
            {
                startDate = dt.AddDays(-30);
            }
            if (endDate == null)
            {
                endDate = dt;
            }
            var ssi = scadaInfoApp.GetEntity(mType.ToString());
            JArray name = JArray.FromObject(new string[] { "前年同期(" + ssi.Units + ")", "去年同期(" + ssi.Units + ")", ssi.Tag_name + "(" + ssi.Units + ")" });
            JArray result = new JArray();
            JArray data1 = new JArray();
            JArray data2 = new JArray();
            JArray data3 = new JArray();
            JArray time = new JArray();
            int tcvs = 0;
            string where = "";//水表数据过滤条件
            string where1 = "";//水表数据过滤条件
            string where2 = "";//水表数据过滤条件
            if (!string.IsNullOrWhiteSpace(stationKey))
            {
                var skeys = stationKey.Split(',');
                for (var i = 0; i < skeys.Length; i++)
                {
                    if (skeys[i].Length > 0)
                    {
                        var sskey = skeys[i].Split('_');
                        if (i == 0)
                        {
                            if (skeys.Length == 1)
                                where += " AND (Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                            else
                                where += " AND ((Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        }
                        else if (i == skeys.Length - 1)
                        {
                            where += " OR (Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + "))";
                        }
                        else
                        {
                            where += " OR (Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        }
                    }
                }
                where1 = where;
                where2 = where;
            }
            string sql = "";
            switch (type)
            {
                case 1:
                    where += " AND Save_date>='" + startDate + "' AND Save_date<'" + endDate.Value.AddDays(1) + "'";
                    where1 += " AND Save_date>='" + startDate.Value.AddYears(-1) + "' AND Save_date<'" + endDate.Value.AddDays(1).AddYears(-1) + "'";
                    where2 += " AND Save_date>='" + startDate.Value.AddYears(-2) + "' AND Save_date<'" + endDate.Value.AddDays(1).AddYears(-2) + "'";
                    if (mType == 1 || mType == 8 || mType == 9)
                        sql = "SELECT a.Save_date ,a.Tag_value Value1,b.Tag_value Value2,c.Tag_value Value3 FROM(SELECT Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10)," +
                        "Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) a1 GROUP BY " +
                        "Save_date) a LEFT JOIN (SELECT Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM " +
                        "BS_SCADA_TAG_HIS x WHERE 1=1" + where1 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) b1 GROUP BY Save_date) b ON RIGHT(a.Save_date,5)=RIGHT(b.Save_date,5) " +
                        "LEFT JOIN (SELECT Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM BS_SCADA_TAG_HIS x WHERE " +
                        "1=1" + where2 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) c1 GROUP BY Save_date) c ON RIGHT(a.Save_date,5)=RIGHT(c.Save_date,5) ORDER BY a.Save_date";
                    else
                        sql = "SELECT a.Save_date ,a.Tag_value Value1,b.Tag_value Value2,c.Tag_value Value3 FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(Tag_value) Tag_value" +
                        " FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where + " GROUP BY CONVERT(VARCHAR(10),Save_date,20)) a LEFT JOIN (SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date," +
                        "AVG(Tag_value) Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where1 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20)) b ON RIGHT(a.Save_date,5)=RIGHT(b.Save_date,5) " +
                        "LEFT JOIN (SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(Tag_value) Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where2 + " GROUP BY CONVERT(VARCHAR(10)," +
                        "Save_date,20)) c ON RIGHT(a.Save_date,5)=RIGHT(c.Save_date,5) ORDER BY a.Save_date";
                    break;
                case 2:
                    where += " AND Save_date>='" + startDate + "' AND Save_date<'" + endDate.Value.AddMonths(1) + "'";
                    where1 += " AND Save_date>='" + startDate.Value.AddYears(-1) + "' AND Save_date<'" + endDate.Value.AddMonths(1).AddYears(-1) + "'";
                    where2 += " AND Save_date>='" + startDate.Value.AddYears(-2) + "' AND Save_date<'" + endDate.Value.AddMonths(1).AddYears(-2) + "'";
                    if (mType == 1 || mType == 8 || mType == 9)
                        sql = "SELECT a.Save_date ,a.Tag_value Value1,b.Tag_value Value2,c.Tag_value Value3 FROM(SELECT LEFT(Save_date,7) Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10)," +
                        "Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) a1 GROUP BY " +
                        "LEFT(Save_date,7)) a LEFT JOIN (SELECT LEFT(Save_date,7) Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM " +
                        "BS_SCADA_TAG_HIS x WHERE 1=1" + where1 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) b1 GROUP BY LEFT(Save_date,7)) b ON RIGHT(a.Save_date,2)=RIGHT(b.Save_date,2) " +
                        "LEFT JOIN (SELECT LEFT(Save_date,7) Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM BS_SCADA_TAG_HIS x WHERE " +
                        "1=1" + where2 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) c1 GROUP BY LEFT(Save_date,7)) c ON RIGHT(a.Save_date,2)=RIGHT(c.Save_date,2) ORDER BY a.Save_date";
                    else
                        sql = "SELECT a.Save_date ,a.Tag_value Value1,b.Tag_value Value2,c.Tag_value Value3 FROM(SELECT CONVERT(VARCHAR(7),Save_date,20) Save_date,AVG(Tag_value) Tag_value " +
                        "FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where + " GROUP BY CONVERT(VARCHAR(7),Save_date,20)) a LEFT JOIN (SELECT CONVERT(VARCHAR(7),Save_date,20) Save_date,AVG(Tag_value) " +
                        "Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where1 + " GROUP BY CONVERT(VARCHAR(7),Save_date,20)) b ON RIGHT(a.Save_date,2)=RIGHT(b.Save_date,2) LEFT JOIN (SELECT " +
                        "CONVERT(VARCHAR(7),Save_date,20) Save_date,AVG(Tag_value) Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where2 + " GROUP BY CONVERT(VARCHAR(7),Save_date,20)) c ON " +
                        "RIGHT(a.Save_date,2)=RIGHT(c.Save_date,2) ORDER BY a.Save_date";
                    break;
                case 3:
                    where += " AND Save_date>='" + startDate + "' AND Save_date<'" + endDate.Value.AddMonths(1) + "'";
                    where1 += " AND Save_date>='" + startDate.Value.AddYears(-1) + "' AND Save_date<'" + endDate.Value.AddMonths(1).AddYears(-1) + "'";
                    where2 += " AND Save_date>='" + startDate.Value.AddYears(-2) + "' AND Save_date<'" + endDate.Value.AddMonths(1).AddYears(-2) + "'";
                    if (mType == 1 || mType == 8 || mType == 9)
                        sql = "SELECT CONCAT(LEFT(Save_date,4),'年第',CEILING((RIGHT(Save_date,2)+2) /3),'季度') Save_date, SUM(v1) Value1,SUM(v2) Value2,SUM(v3) Value3 FROM (SELECT a.Save_date ," +
                        "a.Tag_value v1,b.Tag_value v2,c.Tag_value v3 FROM(FROM(SELECT LEFT(Save_date,7) Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10)," +
                        "Save_date,20) Save_date,AVG(Tag_value)*24 Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) a1 GROUP BY " +
                        "LEFT(Save_date,7)) a LEFT JOIN (SELECT LEFT(Save_date,7) Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(Tag_value)*24 Tag_value FROM " +
                        "BS_SCADA_TAG_HIS x WHERE 1=1" + where1 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) b1 GROUP BY LEFT(Save_date,7)) b ON RIGHT(a.Save_date,2)=RIGHT(b.Save_date,2) " +
                        "LEFT JOIN (SELECT LEFT(Save_date,7) Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(Tag_value)*24 Tag_value FROM BS_SCADA_TAG_HIS x WHERE " +
                        "1=1" + where2 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) c1 GROUP BY LEFT(Save_date,7)) c ON RIGHT(a.Save_date,2)=RIGHT(c.Save_date,2)) d " +
                        "GROUP BY CONCAT(LEFT(Save_date,4),'年第',CEILING((RIGHT(Save_date,2)+2) /3),'季度') ORDER BY Save_date";
                    else
                        sql = "SELECT CONCAT(LEFT(Save_date,4),'年第',CEILING((RIGHT(Save_date,2)+2) /3),'季度')Save_date, AVG(v1) Value1,AVG(v2) Value2,AVG(v3) Value3 FROM (SELECT a.Save_date ," +
                        "a.Tag_value v1,b.Tag_value v2,c.Tag_value v3 FROM(FROM(SELECT CONVERT(VARCHAR(7),Save_date,20) Save_date,AVG(ABS(Tag_value)) Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" +
                        where + " GROUP BY CONVERT(VARCHAR(7),Save_date,20)) a LEFT JOIN (SELECT CONVERT(VARCHAR(7),Save_date,20) Save_date,AVG(ABS(Tag_value)) Tag_value FROM BS_SCADA_TAG_HIS x WHERE " +
                        "1=1" + where1 + " GROUP BY CONVERT(VARCHAR(7),Save_date,20)) b ON RIGHT(a.Save_date,2)=RIGHT(b.Save_date,2) LEFT JOIN (SELECT CONVERT(VARCHAR(7),Save_date,20) Save_date," +
                        "AVG(ABS(Tag_value)) Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where2 + " GROUP BY CONVERT(VARCHAR(7),Save_date,20)) c ON RIGHT(a.Save_date,2)=RIGHT(c.Save_date,2)) d " +
                        "GROUP BY CONCAT(LEFT(Save_date,4),'年第',CEILING((RIGHT(Save_date,2)+2) /3),'季度') ORDER BY Save_date";
                    break;
                default:
                    where += " AND Save_date>='" + startDate + "' AND Save_date<'" + endDate.Value.AddDays(1) + "'";
                    where1 += " AND Save_date>='" + startDate.Value.AddYears(-1) + "' AND Save_date<'" + endDate.Value.AddDays(1).AddYears(-1) + "'";
                    where2 += " AND Save_date>='" + startDate.Value.AddYears(-2) + "' AND Save_date<'" + endDate.Value.AddDays(1).AddYears(-2) + "'";
                    if (mType == 1 || mType == 8 || mType == 9)
                        sql = "SELECT a.Save_date,a.Tag_value Value1,b.Tag_value Value2,c.Tag_value Value3 FROM(SELECT Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10)," +
                        "Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) a1 GROUP BY " +
                        "Save_date) a LEFT JOIN (SELECT Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM " +
                        "BS_SCADA_TAG_HIS x WHERE 1=1" + where1 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) b1 GROUP BY Save_date) b ON RIGHT(a.Save_date,5)=RIGHT(b.Save_date,5) " +
                        "LEFT JOIN (SELECT Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM BS_SCADA_TAG_HIS x WHERE " +
                        "1=1" + where2 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) c1 GROUP BY Save_date) c ON RIGHT(a.Save_date,5)=RIGHT(c.Save_date,5) ORDER BY a.Save_date";
                    else
                        sql = "SELECT a.Save_date,a.Tag_value Value1,b.Tag_value Value2,c.Tag_value Value3 FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(Tag_value) Tag_value " +
                            "FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where + " GROUP BY CONVERT(VARCHAR(10),Save_date,20)) a LEFT JOIN (SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date," +
                            "AVG(Tag_value) Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where1 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20)) b ON RIGHT(a.Save_date,5)=RIGHT(b.Save_date,5) " +
                        "LEFT JOIN (SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(Tag_value) Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where2 + " GROUP BY CONVERT(VARCHAR(10)," +
                        "Save_date,20)) c ON RIGHT(a.Save_date,5)=RIGHT(c.Save_date,5) ORDER BY a.Save_date";
                    break;
            }
            dbcontext.Database.CommandTimeout = int.MaxValue;
            var dataHiss = dbcontext.Database.SqlQuery<ChartFlowView>(sql).ToList();
            if (string.IsNullOrWhiteSpace(stationKey) || startDate == null)
            {
                for (int i = 0; i < 51; i++)
                {
                    mType = 8;
                    Random rd = new Random();
                    int rd1 = rd.Next(21500, 37650);
                    int rd2 = rd.Next(28330, 38440);
                    int rd3 = rd.Next(29330, 39440);
                    System.Threading.Thread.Sleep(50);
                    data1.Add(rd1);
                    data2.Add(rd2);
                    data3.Add(rd3);
                    time.Add(nowDate.AddDays(-50 + i).ToDateString());
                }
            }
            else
            {
                switch (type)
                {
                    case 1:
                        for (var i = 0; Convert.ToDateTime(startDate.Value.AddDays(i).ToDateString()) <= Convert.ToDateTime(endDate.ToDateString()); i++)
                        {
                            var dH11 = dataHiss.Where(a => a.Save_date.Equals(startDate.Value.AddDays(i).ToDateString())).FirstOrDefault();
                            if (dH11 != null)
                            {
                                if (mType == 1 || mType == 8 || mType == 9)
                                {
                                    data1.Add(dH11.Value1.ToDecimal(2));
                                    data2.Add(dH11.Value2.ToDecimal(2));
                                    data3.Add(dH11.Value3.ToDecimal(2));
                                }
                                else
                                {
                                    data1.Add(dH11.Value1.ToDecimal(3));
                                    data2.Add(dH11.Value2.ToDecimal(3));
                                    data3.Add(dH11.Value3.ToDecimal(3));
                                }
                            }
                            else
                            {
                                data1.Add(null);
                                data2.Add(null);
                                data3.Add(null);
                            }
                            time.Add(startDate.Value.AddDays(i).ToDateString());
                            tcvs = i + 1;
                        }
                        break;
                    case 2:
                        for (var i = 0; Convert.ToDateTime(startDate.Value.AddMonths(i).ToString("yyyy-MM") + "-01") <= Convert.ToDateTime(endDate.Value.ToString("yyyy-MM") + "-01"); i++)
                        {
                            var dH11 = dataHiss.Where(a => a.Save_date.Equals(startDate.Value.AddMonths(i).ToString("yyyy-MM"))).FirstOrDefault();
                            if (dH11 != null)
                            {
                                if (mType == 1 || mType == 8 || mType == 9)
                                {
                                    data1.Add(dH11.Value1.ToDecimal(2));
                                    data2.Add(dH11.Value2.ToDecimal(2));
                                    data3.Add(dH11.Value3.ToDecimal(2));
                                }
                                else
                                {
                                    data1.Add(dH11.Value1.ToDecimal(3));
                                    data2.Add(dH11.Value2.ToDecimal(3));
                                    data3.Add(dH11.Value3.ToDecimal(3));
                                }
                            }
                            else
                            {
                                data1.Add(null);
                                data2.Add(null);
                                data3.Add(null);
                            }
                            time.Add(startDate.Value.AddMonths(i).ToString("yyyy-MM"));
                            tcvs = i + 1;
                        }
                        break;
                    case 3:
                        for (var i = 0; i < dataHiss.Count; i++)
                        {
                            var dH11 = dataHiss[i];
                            if (dH11 != null)
                            {
                                if (mType == 1 || mType == 8 || mType == 9)
                                {
                                    data1.Add(dH11.Value1.ToDecimal(2));
                                    data2.Add(dH11.Value2.ToDecimal(2));
                                    data3.Add(dH11.Value3.ToDecimal(2));
                                }
                                else
                                {
                                    data1.Add(dH11.Value1.ToDecimal(3));
                                    data2.Add(dH11.Value2.ToDecimal(3));
                                    data3.Add(dH11.Value3.ToDecimal(3));
                                }
                            }
                            time.Add(dH11.Save_date);
                            tcvs = i + 1;
                        }
                        break;
                    default:
                        for (var i = 0; Convert.ToDateTime(startDate.Value.AddDays(i).ToDateString()) <= Convert.ToDateTime(endDate.ToDateString()); i++)
                        {
                            var dH11 = dataHiss.Where(a => a.Save_date.Equals(startDate.Value.AddDays(i).ToDateString())).FirstOrDefault();
                            if (dH11 != null)
                            {
                                if (mType == 1 || mType == 8 || mType == 9)
                                {
                                    data1.Add(dH11.Value1.ToDecimal(2));
                                    data2.Add(dH11.Value2.ToDecimal(2));
                                    data3.Add(dH11.Value3.ToDecimal(2));
                                }
                                else
                                {
                                    data1.Add(dH11.Value1.ToDecimal(3));
                                    data2.Add(dH11.Value2.ToDecimal(3));
                                    data3.Add(dH11.Value3.ToDecimal(3));
                                }
                            }
                            else
                            {
                                data1.Add(null);
                                data2.Add(null);
                                data3.Add(null);
                            }
                            time.Add(startDate.Value.AddDays(i).ToDateString());
                            tcvs = i + 1;
                        }
                        break;
                }
            }
            result.Add(data3);
            result.Add(data2);
            result.Add(data1);
            result.Add(time);
            result.Add(name);
            if (ssi == null)
            {
                result.Add("");
                result.Add(0);
            }
            else
            {
                result.Add(ssi.Tag_name + "(" + ssi.Units + ")");
                result.Add(tcvs == 0 ? 50 : (25.0 / tcvs.ToDouble() * 100.0).ToInt());
            }
            return Content(result.ToJson());
        }
        public ActionResult GetMaxMinData(string stationKey, int? type, DateTime? startDate, DateTime? endDate)
        {
            DateTime nowDate = DateTime.Now;
            int mType = 8;
            if (!string.IsNullOrWhiteSpace(stationKey))
            {
                mType = stationKey.Split(',')[0].Split('_')[2].ToInt();
            }
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            var tmpdt = bstcapp.GetList().Where(t => t.Tag_key.Equals(mType.ToString())).Max(item => item.Save_date);
            if (tmpdt != null)
            {
                dt = tmpdt.Value;
                dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
            }
            if (startDate == null)
            {
                startDate = dt.AddDays(-30);
            }
            if (endDate == null)
            {
                endDate = dt;
            }
            var ssi = scadaInfoApp.GetEntity(mType.ToString());
            JArray name = JArray.FromObject(new string[] { "最大值(" + ssi.Units + ")", "最小值(" + ssi.Units + ")" });
            JArray result = new JArray();
            JArray data1 = new JArray();
            JArray data2 = new JArray();
            JArray time = new JArray();
            int tcvs = 0;
            string where = "";//水表数据过滤条件
            if (!string.IsNullOrWhiteSpace(stationKey))
            {
                var skeys = stationKey.Split(',');
                for (var i = 0; i < skeys.Length; i++)
                {
                    if (skeys[i].Length > 0)
                    {
                        var sskey = skeys[i].Split('_');
                        if (i == 0)
                        {
                            if (skeys.Length == 1)
                                where += " AND (Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                            else
                                where += " AND ((Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        }
                        else if (i == skeys.Length - 1)
                        {
                            where += " OR (Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + "))";
                        }
                        else
                        {
                            where += " OR (Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        }
                    }
                }
            }
            if (startDate != null || endDate != null)
            {
                if (startDate == null && endDate != null)
                {
                    where += " AND Save_date<='" + endDate + "'";
                }
                else if (startDate != null && endDate == null)
                {
                    where += " AND Save_date>='" + startDate + "'";
                }
                else
                {
                    where += " AND Save_date>='" + startDate + "' AND Save_date<='" + endDate + "'";
                }
            }
            string sql = "SELECT CONVERT(VARCHAR(13),Save_date,20) Save_date,MAX(Tag_value) Max,MIN(Tag_value) Min FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where +
                        " GROUP BY CONVERT(VARCHAR(13),Save_date,20) ORDER BY CONVERT(VARCHAR(13),Save_date,20)";
            dbcontext.Database.CommandTimeout = int.MaxValue;
            var dataHiss = dbcontext.Database.SqlQuery<ChartMaxView>(sql).ToList();
            if (string.IsNullOrWhiteSpace(stationKey) || startDate == null)
            {
                mType = 8;
                for (int i = 0; i < 41; i++)
                {
                    Random rd = new Random();
                    int rd1 = rd.Next(500, 650);
                    int rd2 = rd.Next(330, 440);
                    System.Threading.Thread.Sleep(50);
                    data1.Add(rd1);
                    data2.Add(rd2);
                    time.Add(nowDate.AddHours(-40 + i).ToDateString() + " " + nowDate.AddHours(-40 + i).Hour);
                }
            }
            else
            {
                for (var i = 0; Convert.ToDateTime(startDate.Value.AddHours(i).ToString("yyyy-MM-dd HH") + ":00:00") <= Convert.ToDateTime(endDate.Value.ToString("yyyy-MM-dd HH") + ":00:00"); i++)
                {
                    var dH11 = dataHiss.Where(a => a.Save_date.Equals(startDate.Value.AddHours(i).ToString("yyyy-MM-dd HH"))).FirstOrDefault();
                    if (dH11 != null)
                    {
                        if (mType == 1 || mType == 8 || mType == 9)
                        {
                            data1.Add(dH11.Max.ToDecimal(2));
                            data2.Add(dH11.Min.ToDecimal(2));
                        }
                        else
                        {
                            data1.Add(dH11.Max.ToDecimal(3));
                            data2.Add(dH11.Min.ToDecimal(3));
                        }
                        time.Add(startDate.Value.AddHours(i).ToString("yyyy-MM-dd HH"));
                    }
                    tcvs = i + 1;
                }
            }
            JArray tmp = new JArray();
            tmp.Add(data1);
            tmp.Add(data2);
            result.Add(tmp);
            result.Add(time);
            result.Add(name);
            if (ssi == null)
            {
                result.Add("");
                result.Add(0);
            }
            else
            {
                result.Add(ssi.Tag_name + "(" + ssi.Units + ")");
                result.Add(tcvs == 0 ? 50 : (25.0 / tcvs.ToDouble() * 100.0).ToInt());
            }
            return Content(result.ToJson());
        }
        public ActionResult GetFlowRateDowdload(string stationKey, int? type, DateTime? startDate, DateTime? endDate)
        {
            //int mType = bmiApp.GetList(stationKey).Select(i => i.Meter_Type).FirstOrDefault().ToInt();
            int mType = type == null ? 1 : type.Value;
            string path = bsthapp.GetFlowRateDowdload(stationKey, mType, startDate, endDate);
            //StringBuilder sbScript = new StringBuilder();
            //sbScript.Append("<script type='text/javascript'>$.loading(false);</script>");
            return Content("http://" + Request.Url.Host + ":" + Request.Url.Port + path);
        }
        public ActionResult GetFlowDowdload(string stationKey, int? type, DateTime? startDate, DateTime? endDate)
        {
            int mType = 0;
            if (!string.IsNullOrWhiteSpace(stationKey))
            {
                mType = stationKey.Split(',')[0].Split('_')[2].ToInt();
            }
            string path = bsthapp.GetFlowDowdload(stationKey, mType, type, startDate, endDate);
            //StringBuilder sbScript = new StringBuilder();
            //sbScript.Append("<script type='text/javascript'>$.loading(false);</script>");
            return Content("http://" + Request.Url.Host + ":" + Request.Url.Port + path);
        }
        public ActionResult GetMaxMinDowdload(string stationKey, int? type, DateTime? startDate, DateTime? endDate)
        {
            int mType = 0;
            if (!string.IsNullOrWhiteSpace(stationKey))
            {
                mType = stationKey.Split(',')[0].Split('_')[2].ToInt();
            }
            string path = bsthapp.GetMaxMinDowdload(stationKey, mType, type, startDate, endDate);
            //StringBuilder sbScript = new StringBuilder();
            //sbScript.Append("<script type='text/javascript'>$.loading(false);</script>");
            return Content("http://" + Request.Url.Host + ":" + Request.Url.Port + path);
        }
        public class ChartView
        {
            public decimal? Rate { get; set; }
            public string Station_Key { get; set; }
            public string Name { get; set; }
            public string Save_date { get; set; }
        }
        public class ChartMaxView
        {
            public decimal Max { get; set; }
            public decimal Min { get; set; }
            public string Save_date { get; set; }
        }
        public class ChartFlowView
        {
            public decimal? Value1 { get; set; }
            public decimal? Value2 { get; set; }
            public decimal? Value3 { get; set; }
            public string Save_date { get; set; }
        }
    }
}