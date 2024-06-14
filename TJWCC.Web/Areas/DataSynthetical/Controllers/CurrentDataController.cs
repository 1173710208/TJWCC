using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.BSSys;
using TJWCC.Code;
using TJWCC.Data;
using TJWCC.Domain.Entity.BSSys;
using TJWCC.Domain.ViewModel;

namespace TJWCC.Web.Areas.DataSynthetical.Controllers
{
    public class CurrentDataController : ControllerBase
    {
        private BS_SCADA_TAG_CURRENTApp bstcapp = new BS_SCADA_TAG_CURRENTApp();
        private DistrictClassApp dcApp = new DistrictClassApp();
        private BSM_Meter_InfoApp bmiapp = new BSM_Meter_InfoApp();
        TJWCCDbContext dbcontext = new TJWCCDbContext();
        int? AreaId;
        int? Type;
        int? SPoint;
        int? SWater;
        // 实时监测数据: DataSynthetical/CurrentData

        /// <summary>
        /// 获取数据类型下拉菜单列表
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
        /// 获取监测点下拉菜单列表
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetPointList(int? areaId, int? type, int? sPoint, int? sWater)
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            AreaId = areaId;
            Type = type;
            SPoint = sPoint;
            SWater = sWater;
            if (areaId != null)
            {
                var sds = bmiapp.GetList(areaId, type, sPoint, sWater).OrderBy(i => i.Meter_Name);
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
        /// <summary>
        /// 实时数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCurrentData(string sks)
        {
            List<GetNewData_Result> result = new List<GetNewData_Result>();
            if (!string.IsNullOrWhiteSpace(sks))
                foreach (var skey in sks.Split(','))
                {
                    var sskey = skey.Split('_');
                    string sql = "SELECT bmi.ElementID,bmi.WMeter_ID,bmi.Meter_Name,ltrim(bmi.Station_Unit) Station_Unit,bmi.Geo_x,bmi.Geo_y,[Explain],Remark,Tag_value,Save_date,Measure_Grade FROM " +
                        "(SELECT ElementID,CONVERT(varchar(20),WMeter_ID) WMeter_ID,Meter_Name,Station_Unit,Geo_x,Geo_y,[Explain],Station_Key,Meter_Type,Remark,Measure_Grade FROM [dbo].[BSM_Meter_Info] " +
                        "WHERE Station_Key ='" + sskey[1] + "' AND Meter_Type=" + sskey[2] + " AND DistrictAreaId=" + sskey[3] + ") bmi LEFT JOIN (SELECT (CASE WHEN Tag_value IS NULL THEN 0 ELSE Tag_value END) Tag_value,GYStationId," +
                        "Save_date,Tag_key FROM BS_SCADA_TAG_CURRENT WHERE GYStationId ='" + sskey[1] + "' AND Number=" + sskey[3] + ") bth ON bmi.Station_Key = bth.GYStationId AND Meter_Type = Tag_key " +
                        "ORDER BY Station_Unit, WMeter_ID";
                    List<GetNewData_Result> bsgnd = dbcontext.Database.SqlQuery<GetNewData_Result>(sql).ToList();
                    result.AddRange(bsgnd);
                }
            return Content(result.ToJson());
        }
        /// <summary>
        /// 余氯实时数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetClData(string factory, string district)
        {
            string jr = GetDatas("3", factory, district);
            return Content(jr);
        }
        /// <summary>
        /// 浊度实时数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTurbidityData(string factory, string district)
        {
            string jr = GetDatas("4", factory, district);
            return Content(jr);
        }
        /// <summary>
        /// 水质实时数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetQualityData(string factory, string district)
        {
            string jr = GetDatas("3,4", factory, district);
            return Content(jr);
        }
        /// <summary>
        /// 压力实时数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPressureData(string factory, string district)
        {
            string jr = GetDatas("2", factory, district);
            return Content(jr);
        }
        /// <summary>
        /// 流量实时数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMeterData(string factory, string district)
        {
            string jr = GetDatas("1,9", factory, district);
            return Content(jr);
        }
        /// <summary>
        /// 原水流量实时数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSourceData(string factory, string district)
        {
            string jr = GetDatas("8", factory, district);
            return Content(jr);
        }
        public string GetDatas(string meterType, string factory, string district)
        {
            string sql = "SELECT bmi.ElementID,bmi.WMeter_ID,bmi.Meter_Name,ltrim(bmi.Station_Unit)  Station_Unit,bmi.Geo_x,bmi.Geo_y,[Explain],Remark,Tag_value,Save_date,Measure_Grade FROM " +
                "(SELECT ElementID,CONVERT(varchar(20),WMeter_ID) WMeter_ID,Meter_Name,Station_Unit,Geo_x,Geo_y,[Explain],Station_Key,Meter_Type,Remark,Measure_Grade FROM [dbo].[BSM_Meter_Info] " +
                "WHERE Meter_Type in (" + meterType + ") AND Display=1) bmi LEFT JOIN (select (CASE WHEN Tag_value is null THEN 0 ELSE Tag_value END) Tag_value,Station_Key,Save_date,Tag_key " +
                "from BS_SCADA_TAG_CURRENT where Tag_key in(" + meterType + ")) bth ON bmi.Station_Key = bth.Station_key and bmi.Meter_Type = bth.Tag_key ORDER BY bmi.Station_Unit, bmi.WMeter_ID";
            if (!string.IsNullOrWhiteSpace(factory))
                sql = "SELECT bmi.ElementID,bmi.WMeter_ID,bmi.Meter_Name,ltrim(bmi.Station_Unit)  Station_Unit,bmi.Geo_x,bmi.Geo_y,[Explain],Remark,Tag_value,Save_date,Measure_Grade FROM " +
                    "(SELECT ElementID,CONVERT(varchar(20),WMeter_ID) WMeter_ID,Meter_Name,Station_Unit,Geo_x,Geo_y,[Explain],Station_Key,Meter_Type,Remark,Measure_Grade FROM [dbo].[BSM_Meter_Info] " +
                    "WHERE Meter_Type in (" + meterType + ") AND ElementID in(SELECT ElementID FROM RESULT_JUNCTION WHERE Result_Source_24='"+factory+ "') AND Display=1) bmi LEFT JOIN (select " +
                    "(CASE WHEN Tag_value is null THEN 0 ELSE Tag_value END) Tag_value,Station_Key,Save_date,Tag_key from BS_SCADA_TAG_CURRENT where Tag_key in(" + meterType + ")) bth ON " +
                    "bmi.Station_Key = bth.Station_key and bmi.Meter_Type = bth.Tag_key ORDER BY bmi.Station_Unit, bmi.WMeter_ID";
            if (!string.IsNullOrWhiteSpace(district))
                sql = "SELECT bmi.ElementID,bmi.WMeter_ID,bmi.Meter_Name,ltrim(bmi.Station_Unit)  Station_Unit,bmi.Geo_x,bmi.Geo_y,[Explain],Remark,Tag_value,Save_date,Measure_Grade FROM " +
                    "(SELECT ElementID,CONVERT(varchar(20),WMeter_ID) WMeter_ID,Meter_Name,Station_Unit,Geo_x,Geo_y,[Explain],Station_Key,Meter_Type,Remark,Measure_Grade FROM [dbo].[BSM_Meter_Info] " +
                    "WHERE Meter_Type in (" + meterType + ") AND DistrictAreaId="+ district + " AND Display=1) bmi LEFT JOIN (select (CASE WHEN Tag_value is null THEN 0 ELSE Tag_value END) Tag_value,Station_Key,Save_date,Tag_key " +
                    "from BS_SCADA_TAG_CURRENT where Tag_key in(" + meterType + ")) bth ON bmi.Station_Key = bth.Station_key and bmi.Meter_Type = bth.Tag_key ORDER BY Station_Unit, WMeter_ID";
            List<GetNewData_Result> bsgnd = dbcontext.Database.SqlQuery<GetNewData_Result>(sql).ToList();
            return Code.Json.ToJson(bsgnd, "yyyy-MM-dd HH:mm:ss");
        }

    }
}