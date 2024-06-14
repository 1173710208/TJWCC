using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.BSSys;
using TJWCC.Application.SystemManage;
using TJWCC.Application.WCC;
using TJWCC.Code;
using TJWCC.Data;
using TJWCC.Domain.ViewModel;

namespace TJWCC.Web.Areas.Scheduling.Controllers
{
    public class LongTimeController : ControllerBase
    {
        log4net.ILog loger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        TJWCCDbContext dbcontext = new TJWCCDbContext();
        private BS_SCADA_TAG_CURRENTApp bstcapp = new BS_SCADA_TAG_CURRENTApp();
        private CC_FlowSortApp cfsapp = new CC_FlowSortApp();
        private DistrictClassApp DistrictClass = new DistrictClassApp();
        private SYS_DICApp sysDicApp = new SYS_DICApp();
        // 中长期预测: Scheduling/LongTime

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
            //var dists = DistrictClass.GetList().OrderBy(item => item.ID).ToList();
            //foreach (var dist in dists)
            //{
            //    GetDataType_Result tmp = new GetDataType_Result()
            //    {
            //        Name = dist.District,
            //        Value = dist.ID.ToString()
            //    };
            //    list.Add(tmp);
            //}
            return Content(list.ToJson());
        }

        public ActionResult DemandPYear(decimal people, decimal gdp)
        {
            try
            {
                dbcontext.Database.Connection.Open();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            DbCommand cmd = dbcontext.Database.Connection.CreateCommand();
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "UPDATE [dbo].[CC_FlowSort] SET Sort_value=" + gdp + ",Remark=null WHERE Save_date='" + DateTime.Now.AddYears(-1).Year + "-01-01 00:00:00' AND Type=7;" +
                "UPDATE [dbo].[CC_FlowSort] SET Sort_value=" + people + ",Remark=null WHERE Save_date='" + DateTime.Now.AddYears(-1).Year + "-01-01 00:00:00' AND Type=8;";
            try
            {
                cmd.ExecuteNonQuery();
                dbcontext.Database.Connection.Close();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            dbcontext.Database.Connection.Close();
            return Success("操作成功。");
        }
        
        public ActionResult DemandPredictionBut(int areaId, decimal pRate, decimal gRate)
        {
            try
            {
                dbcontext.Database.Connection.Open();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            DbCommand cmd = dbcontext.Database.Connection.CreateCommand();
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "UPDATE [dbo].[CC_FlowSort] SET Remark='" + gRate + "' WHERE Save_date='" + DateTime.Now.Year + "-01-01 00:00:00' AND Type=7;" +
                "UPDATE [dbo].[CC_FlowSort] SET Remark='" + pRate + "' WHERE Save_date='" + DateTime.Now.Year + "-01-01 00:00:00' AND Type=8;";
            try
            {
                cmd.ExecuteNonQuery();
                dbcontext.Database.Connection.Close();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            dbcontext.Database.Connection.Close();
            return DemandPrediction(areaId, "6");
        }
        
        public JsonResult DemandPrediction(int areaId, string type)
        {
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
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
                    startDatetime = new DateTime(dt.AddMonths(-7).Year, dt.AddMonths(-7).Month, dt.AddMonths(-7).Day, 0, 0, 0);
                    int month = (dt.Month - 1) / 3 * 3 + 1;
                    endDatetime = new DateTime(dt.Year, month, 1, 0, 0, 0).AddSeconds(-1);
                    break;
                case "6":
                    startDatetime = new DateTime(dt.AddYears(-5).Year, dt.AddYears(-5).Month, 1, 0, 0, 0);
                    endDatetime = new DateTime(dt.Year, 1, 1, 0, 0, 0).AddSeconds(-1);
                    break;
                default:
                    startDatetime = new DateTime(dt.AddDays(-15).Year, dt.AddDays(-15).Month, dt.AddDays(-15).Day, 0, 0, 0);
                    endDatetime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0).AddSeconds(-1);
                    break;
            }
            string sql = "SELECT FlowTime Save_date,DMA Sort_key,Sort_value,ForecastFlow,DMAGrade,Type FROM (SELECT * FROM [dbo].[BSB_ForecastFlow] WHERE DMA=" + areaId + " AND MoreAlert=" + type
                 + " AND FlowTime>'" + startDatetime + "' AND FlowTime<='" + endDatetime + "') b LEFT JOIN (SELECT * FROM [dbo].[CC_FlowSort] WHERE Sort_key=" + areaId + " AND Type=" + type
                + " AND Save_date>='" + startDatetime + "' AND Save_date<='" + endDatetime + "') a ON a.Sort_key=b.DMA AND a.Save_date=b.FlowTime";
            List<GetForecastFlow_Result> arrlist = dbcontext.Database.SqlQuery<GetForecastFlow_Result>(sql).OrderBy(i => i.Save_date).ToList();
            for (int i = 0; i < arrlist.Count; i++)
            {
                ArrayList _obtmpDV = new ArrayList();
                decimal? tmpJll = 0m;
                tmpJll = arrlist[i].Sort_value / 10000m;
                if (type.Equals("6"))
                {
                    _obtmpDV.Add(arrlist[i].Save_date.Value.Year.ToString());//date
                }
                else
                {
                    _obtmpDV.Add(arrlist[i].Save_date.Value.ToString("yyyy-MM"));//date
                }
                _obtmpDV.Add(tmpJll == null ? null : tmpJll.Value.ToString("#0"));//value
                _obDateValue.Add(_obtmpDV);
                if (i == 0 && tmpJll != null)
                {
                    _maxJll = tmpJll.Value;
                    _minJll = tmpJll.Value;
                }
                else if (tmpJll > _maxJll && tmpJll != null)
                {
                    _maxJll = tmpJll.Value;
                }
                if (tmpJll < _minJll && tmpJll != null)
                {
                    _minJll = tmpJll.Value;
                }

                _obtmpDV = new ArrayList();
                tmpJll = arrlist[i].ForecastFlow / 10000m;
                if (type.Equals("6"))
                {
                    _obtmpDV.Add(arrlist[i].Save_date.Value.Year.ToString());//date
                }
                else
                {
                    _obtmpDV.Add(arrlist[i].Save_date.Value.ToString("yyyy-MM"));//date
                }
                _obtmpDV.Add(tmpJll == null ? null : tmpJll.Value.ToString("#0"));//value
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
                        tmpdata = arrlist[i].Save_date.Value.Year.ToString();//date
                    else
                        tmpdata = arrlist[i].Save_date.Value.ToString("yyyy-MM");//date
                    if (tmpdata.Equals((_obDateValue[j] as ArrayList)[0].ToString()))
                    {
                        if (Convert.ToDouble((_obDateValue[j] as ArrayList)[1]) == 0) tmpErr = 0;
                        else tmpErr = Math.Abs(Convert.ToDouble(tmpJll) - Convert.ToDouble((_obDateValue[j] as ArrayList)[1])) / Math.Abs(Convert.ToDouble((_obDateValue[j] as ArrayList)[1])) * 100;
                        ArrayList _yctmpErr = new ArrayList();
                        if (type.Equals("6"))
                            _yctmpErr.Add(arrlist[i].Save_date.Value.Year.ToString());//date
                        else
                            _yctmpErr.Add(arrlist[i].Save_date.Value.ToString("yyyy-MM"));//date
                        _yctmpErr.Add(Math.Round(tmpErr, 2));//value
                        _ycErr.Add(_yctmpErr);
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
            }
            sql = "SELECT FlowTime Save_date,DMA Sort_key,ForecastFlow,DMAGrade,MoreAlert Type FROM [dbo].[BSB_ForecastFlow] WHERE DMA=" + areaId
                + " AND MoreAlert=" + type + " AND FlowTime>'" + endDatetime + "'";
            arrlist = dbcontext.Database.SqlQuery<GetForecastFlow_Result>(sql).OrderBy(i => i.Save_date).ToList();
            for (int i = 0; i < arrlist.Count; i++)
            {
                ArrayList _obtmpDV = new ArrayList();
                decimal? tmpJll = 0m;
                tmpJll = arrlist[i].ForecastFlow / 10000m;
                if (type.Equals("6"))
                {
                    _obtmpDV.Add(arrlist[i].Save_date.Value.Year.ToString());//date
                }
                else
                {
                    _obtmpDV.Add(arrlist[i].Save_date.Value.ToString("yyyy-MM"));//date
                }
                _obtmpDV.Add(tmpJll == null ? null : tmpJll.Value.ToString("#0"));//value
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
            result.Add(Convert.ToInt64(_minJll == decimal.MaxValue ? 0m : (_minJll - _minJll * 0.1m)));//最小流量
            result.Add(Math.Round(_maxErr == double.MinValue ? 0.0:(_maxErr * 2.5), 2));//最大误差
            result.Add(Math.Round(_minErr == double.MaxValue ? 0.0:_minErr, 2));//最小误差
            if (type.Equals("6"))
            {
                var dpYear = cfsapp.GetDPYearList();
                var yearP = new DateTime(DateTime.Now.Year - 1, 1, 1);
                var yearNow = new DateTime(DateTime.Now.Year, 1, 1);
                var people = dpYear.Where(i => i.Save_date == yearP && i.Type == 8).Select(i => i.Sort_value).FirstOrDefault().Value;
                var gdp = dpYear.Where(i => i.Save_date == yearP && i.Type == 7).Select(i => i.Sort_value).FirstOrDefault().Value;
                var pRate = (((ycDateValue[0] as ArrayList)[1].ToDecimal() - (_obDateValue[_obDateValue.Count - 1] as ArrayList)[1].ToDecimal()) / (_obDateValue[_obDateValue.Count - 1] as ArrayList)[1].ToDecimal() * 100m).ToString("0.00");
                var gRate = dpYear.Where(i => i.Save_date == yearNow && i.Type == 7).Select(i => i.Remark).FirstOrDefault();
                var nRate = (((ycDateValue[1] as ArrayList)[1].ToDecimal() - (ycDateValue[0] as ArrayList)[1].ToDecimal()) / (ycDateValue[0] as ArrayList)[1].ToDecimal() * 100m).ToString("0.00");
                result.Add(new string[] { people.ToString("#"), gdp.ToString("0.00"), pRate, gRate, nRate });
            }

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
    }
}