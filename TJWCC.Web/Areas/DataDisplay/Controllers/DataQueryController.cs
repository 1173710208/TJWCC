using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.BSSys;
using TJWCC.Code;
using TJWCC.Data;
using TJWCC.Domain.ViewModel;

namespace TJWCC.Web.Areas.DataDisplay.Controllers
{
    public class DataQueryController : ControllerBase
    {
        private BS_SCADA_TAG_CURRENTApp bstcapp = new BS_SCADA_TAG_CURRENTApp();
        private BS_SCADA_TAG_HISApp bsthapp = new BS_SCADA_TAG_HISApp();
        TJWCCDbContext dbcontext = new TJWCCDbContext();
        // GET: DataDisplay/DataQuery
        /// <summary>
        /// 所以监测点实时数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllData(string keyword)
        {
            string jr = GetDatas("1,2,3,4,9", keyword);
            return Content(jr);
        }
        /// <summary>
        /// 余氯实时数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetClData(string keyword)
        {
            string jr = GetDatas("3", keyword);
            return Content(jr);
        }
        /// <summary>
        /// 浊度实时数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTurbidityData(string keyword)
        {
            string jr = GetDatas("4", keyword);
            return Content(jr);
        }
        /// <summary>
        /// 水质实时数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetQualityData(string keyword)
        {
            string jr = GetDatas("3,4", keyword);
            return Content(jr);
        }
        /// <summary>
        /// 压力实时数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPressureData(string keyword)
        {
            string jr = GetDatas("2", keyword);
            return Content(jr);
        }
        /// <summary>
        /// 流量实时数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMeterData(string keyword)
        {
            string jr = GetDatas("1,9", keyword);
            return Content(jr);
        }
        /// <summary>
        /// 原水流量实时数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSourceData(string keyword)
        {
            string jr = GetDatas("8", keyword);
            return Content(jr);
        }
        public string GetDatas(string meterType, string keyword)
        {
            string sql = "SELECT ElementID,WMeter_ID,Meter_Name,ltrim(Station_Unit) Station_Unit,Geo_x,Geo_y,[Explain],bmi.Remark,Tag_value,Save_date,Measure_Grade FROM " +
                "(SELECT ElementID,CONVERT(varchar(20),WMeter_ID) WMeter_ID,Meter_Name,Station_Unit,Geo_x,Geo_y,[Explain],Station_Key,Meter_Type,Remark,Measure_Grade FROM [dbo].[BSM_Meter_Info] " +
                "WHERE Meter_Type in ("+ meterType + ") AND Display=1) bmi LEFT JOIN (select (CASE WHEN Tag_value is null THEN 0 ELSE Tag_value END) Tag_value,Station_Key,Save_date,Tag_key " +
                "FROM BS_SCADA_TAG_CURRENT WHERE Tag_key in(" + meterType+ ")) bth ON bmi.Station_Key = bth.Station_key AND bmi.Meter_Type = bth.Tag_key ORDER BY Station_Unit, WMeter_ID";
            if (!string.IsNullOrEmpty(keyword))
            {
                sql = "SELECT ElementID,WMeter_ID,Meter_Name,ltrim(Station_Unit) Station_Unit,Geo_x,Geo_y,[Explain],Remark,Tag_value,Save_date,Measure_Grade FROM " +
                "(SELECT ElementID,CONVERT(varchar(20),WMeter_ID) WMeter_ID,Meter_Name,Station_Unit,Geo_x,Geo_y,[Explain],Station_Key,Meter_Type,Remark,Measure_Grade FROM [dbo].[BSM_Meter_Info] " +
                "WHERE Meter_Type in (" + meterType + ") AND Display=1 AND Meter_Name LIKE'%" + keyword + "%') bmi LEFT JOIN (select (CASE WHEN Tag_value is null THEN 0 ELSE Tag_value END) Tag_value," +
                "Station_Key,Save_date,Tag_key FROM BS_SCADA_TAG_CURRENT WHERE Tag_key in(" + meterType + ")) bth ON bmi.Station_Key = bth.Station_key AND Meter_Type = Tag_key " +
                "ORDER BY Station_Unit, WMeter_ID";
            }
            else if (keyword != null)
            {
                return new List<GetNewData_Result>().ToJson();
            }
            List<GetNewData_Result> bsgnd = dbcontext.Database.SqlQuery<GetNewData_Result>(sql).ToList();
            return bsgnd.ToJson();
        }
        public JsonResult GetHistoryData()
        {
            ArrayList result = new ArrayList();
            ArrayList listValue = new ArrayList();
            ArrayList listOValue = new ArrayList();
            ArrayList listDate = new ArrayList();
            double _max = new double();
            double _min = new double();
            var id = Request["id"];
            var num = Request["num"];
            var type = Request["type"];
            //1压力、2水质、4流量
            string[] tagKey;
            string title = "";
            string unit = "";
            if (num == "1")
            {
                tagKey = new string[] { "2", "4", "8" };
                title = "压力";
                unit = ArrayDataTableHelper.PressUnit;
            }
            else if (num == "2")
            {
                tagKey = new string[] { "3" };
                title = "余氯";
                unit = ArrayDataTableHelper.ClUnit;
            }
            else if (num == "4")
            {
                tagKey = new string[] { "1", "8", "9" };
                title = "流量";
                unit = ArrayDataTableHelper.FlowUnit;
            }
            else
            {
                var tmpName = num.Split('-');
                tagKey = new string[] { "11", "12", "13", "14", "15", "16" };
                title = tmpName[0];
                unit = tmpName[1];
            }
            if (num == "4")
            {
                DateTime dt = DateTime.Now;
                    if (bstcapp.GetList().Where(item => new string[] { "1", "8" }.Contains(item.Tag_key)).Count() > 0)
                        dt = (DateTime)bstcapp.GetList().Where(item => new string[] { "1", "8" }.Contains(item.Tag_key)).Max(item => item.Save_date);
                DateTime dt2 = dt.AddDays(-1);
                switch (type)
                {
                    case "day":
                        dt2 = dt.AddDays(-1);
                        break;
                    case "week":
                        dt2 = dt.AddDays(-7);
                        break;
                    case "month":
                        dt2 = dt.AddMonths(-1);
                        break;
                    case "year":
                        dt2 = dt.AddYears(-1);
                        break;
                }
                DateTime dt3 = dt2.AddYears(-1);
                DateTime dt4 = dt.AddYears(-1);
                //取当天数据。
                var listHis = (from ss in bsthapp.GetList()
                               where ss.Save_date >= dt2 && ss.Station_key == id && tagKey.Contains( ss.Tag_key )
                               orderby ss.Save_date
                               select ss).ToList();
                //取一年前当天的数据。
                var listHisy = (from sss in bsthapp.GetList()
                                where sss.Save_date >= dt3 && sss.Save_date <= dt4 && sss.Station_key == id && tagKey.Contains( sss.Tag_key )
                                orderby sss.Save_date
                                select sss).ToList();
                for (int i = 0; i < listHis.Count; i++)
                {
                    listValue.Add(listHis[i].Tag_value_ss);
                    listDate.Add(listHis[i].Save_date.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    int tmpid = 0;
                    for (int j = i; j < listHisy.Count; j++)
                    {
                        string datetimestr1 = listHis[i].Save_date.Value.ToString("HH:mm:ss");
                        string datetimestr2 = listHisy[j].Save_date.Value.ToString("HH:mm:ss");
                        if (datetimestr1 == datetimestr2)
                        {
                            listOValue.Add(listHisy[j].Tag_value_ss);
                            tmpid = j;
                            break;
                        }
                    }
                    if (i == 0)
                    {
                        if (tmpid != 0)//如果历史数据中找到对应listOValue值就比较listHis和listHisy中的值
                        {
                            _max = Convert.ToDouble(listHis[i].Tag_value_ss.Value < listHisy[tmpid].Tag_value_ss.Value ? listHisy[tmpid].Tag_value_ss.Value : listHis[i].Tag_value_ss.Value);
                            _min = Convert.ToDouble(listHis[i].Tag_value_ss.Value > listHisy[tmpid].Tag_value_ss.Value ? listHisy[tmpid].Tag_value_ss.Value : listHis[i].Tag_value_ss.Value);
                        }
                        else
                        {
                            _max = Convert.ToDouble(listHis[i].Tag_value_ss.Value);
                            _min = Convert.ToDouble(listHis[i].Tag_value_ss.Value);
                        }
                    }
                    else
                    {
                        if (tmpid != 0)//如果历史数据中找到对应listOValue值就比较listHis和listHisy中的值
                        {
                            double tmpValue = Convert.ToDouble(listHis[i].Tag_value_ss.Value);
                            if (tmpValue > _max) _max = tmpValue;
                            if (tmpValue < _min) _min = tmpValue;
                            tmpValue = Convert.ToDouble(listHisy[tmpid].Tag_value_ss.Value);
                            if (tmpValue > _max) _max = tmpValue;
                            if (tmpValue < _min) _min = tmpValue;
                        }
                        else
                        {
                            double tmpValue = Convert.ToDouble(listHis[i].Tag_value_ss.Value);
                            if (tmpValue > _max) _max = tmpValue;
                            if (tmpValue < _min) _min = tmpValue;
                        }
                    }
                }
            }
            else
            {
                DateTime dt = DateTime.Now;
                    if (bstcapp.GetList().Where(item => item.Station_key == id).Select(item => item.Save_date).Count() > 0)
                        dt = (DateTime)bstcapp.GetList().Where(item => item.Station_key == id).Select(item => item.Save_date).FirstOrDefault();
                DateTime dt2 = dt.AddDays(-1);
                switch (type)
                {
                    case "day":
                        dt2 = dt.AddDays(-1);
                        break;
                    case "week":
                        dt2 = dt.AddDays(-7);
                        break;
                    case "month":
                        dt2 = dt.AddMonths(-1);
                        break;
                    case "year":
                        dt2 = dt.AddYears(-1);
                        break;
                }
                DateTime dt3 = dt2.AddYears(-1);
                DateTime dt4 = dt.AddYears(-1);
                //取当天数据。
                var listHis = (from ss in bsthapp.GetList()
                               where ss.Save_date >= dt2 && ss.Station_key == id && tagKey.Contains(ss.Tag_key)
                               orderby ss.Save_date
                               select ss).ToList();
                //取一年前当天的数据。
                var listHisy = (from sss in bsthapp.GetList()
                                where sss.Save_date >= dt3 && sss.Save_date <= dt4 && sss.Station_key == id && tagKey.Contains(sss.Tag_key)
                                orderby sss.Save_date
                                select sss).ToList();
                for (int i = 0; i < listHis.Count; i++)
                {
                    listValue.Add(listHis[i].Tag_value);
                    listDate.Add(listHis[i].Save_date.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    int tmpid = 0;
                    for (int j = i; j < listHisy.Count; j++)
                    {
                        string datetimestr1 = listHis[i].Save_date.Value.ToString("HH:mm:ss");
                        string datetimestr2 = listHisy[j].Save_date.Value.ToString("HH:mm:ss");
                        if (datetimestr1 == datetimestr2)
                        {
                            listOValue.Add(listHisy[j].Tag_value);
                            tmpid = j;
                            break;
                        }
                    }
                    if (i == 0)
                    {
                        if (tmpid != 0)//如果历史数据中找到对应listOValue值就比较listHis和listHisy中的值
                        {
                            _max = Convert.ToDouble(listHis[i].Tag_value.Value < listHisy[tmpid].Tag_value.Value ? listHisy[tmpid].Tag_value.Value : listHis[i].Tag_value.Value);
                            _min = Convert.ToDouble(listHis[i].Tag_value.Value > listHisy[tmpid].Tag_value.Value ? listHisy[tmpid].Tag_value.Value : listHis[i].Tag_value.Value);
                        }
                        else
                        {
                            _max = Convert.ToDouble(listHis[i].Tag_value.Value);
                            _min = Convert.ToDouble(listHis[i].Tag_value.Value);
                        }
                    }
                    else
                    {
                        if (tmpid != 0)//如果历史数据中找到对应listOValue值就比较listHis和listHisy中的值
                        {
                            double tmpValue = Convert.ToDouble(listHis[i].Tag_value.Value);
                            if (tmpValue > _max) _max = tmpValue;
                            if (tmpValue < _min) _min = tmpValue;
                            tmpValue = Convert.ToDouble(listHisy[tmpid].Tag_value.Value);
                            if (tmpValue > _max) _max = tmpValue;
                            if (tmpValue < _min) _min = tmpValue;
                        }
                        else
                        {
                            double tmpValue = Convert.ToDouble(listHis[i].Tag_value.Value);
                            if (tmpValue > _max) _max = tmpValue;
                            if (tmpValue < _min) _min = tmpValue;
                        }
                    }
                }
            }
            result.Add(listValue);
            result.Add(listOValue);
            result.Add(listDate);
            result.Add(title);
            result.Add(unit);
            result.Add(_max);
            result.Add(_min);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}