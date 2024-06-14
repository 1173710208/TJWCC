using TJWCC.Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJWCC.Repository.BSSys;
using TJWCC.Domain.IRepository.BSSys;
using TJWCC.Domain.Entity.BSSys;
using TJWCC.Data;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using DGSWDC.Code;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using TJWCC.Domain.ViewModel;
using Newtonsoft.Json.Linq;
using TJWCC.Application.SystemManage;

namespace TJWCC.Application.BSSys
{

    public class BS_SCADA_TAG_HISApp
    {
        TJWCCDbContext dbcontext = new TJWCCDbContext();
        private IBS_SCADA_TAG_HISRepository service = new BS_SCADA_TAG_HISRepository();
        private BS_SCADA_TAG_CURRENTApp bstcapp = new BS_SCADA_TAG_CURRENTApp();
        private BSM_Meter_InfoApp bmiApp = new BSM_Meter_InfoApp();
        private SYS_DICApp sysDicApp = new SYS_DICApp();

        /// <summary>
        /// 获取SCADA
        /// </summary>
        /// <returns></returns>
        public List<BS_SCADA_TAG_HISEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        public List<BS_SCADA_TAG_HISEntity> GetList(string Station_key, DateTime sDate, DateTime eDate)
        {
            var expression = ExtLinq.True<BS_SCADA_TAG_HISEntity>();
            if (!string.IsNullOrEmpty(Station_key))
            {
                expression = expression.And(t => t.Station_key.Equals(Station_key));
            }
            expression = expression.And(t => t.Save_date > sDate && t.Save_date <= eDate);
            return service.IQueryable(expression).ToList();
        }
        public List<BS_SCADA_TAG_HISEntity> GetTermList(string Station_key, DateTime? sDate, DateTime? eDate)
        {
            var expression = ExtLinq.True<BS_SCADA_TAG_HISEntity>();
            if (!string.IsNullOrEmpty(Station_key))
            {
                expression = expression.And(t => ("'" + Station_key.Replace(",", "','") + "'").Contains("'" + t.Station_key + "'"));
            }
            else
            {
                return new List<BS_SCADA_TAG_HISEntity>();
            }
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            if (!false)
            {
                //dt = new DateTime(_now.Year, _now.Month, _now.Day, _now.Hour, 0, 0);
                var tmpdt = bstcapp.GetList().Where(t => ("'" + Station_key.Replace(",", "','") + "'").Contains("'" + t.Station_key + "'")).Max(item => item.Save_date);
                if (tmpdt != null)
                {
                    dt = tmpdt.Value;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                }
                if (sDate == null)
                {
                    sDate = dt.AddDays(-30);
                }
                if (eDate == null)
                {
                    eDate = dt;
                }
            }
            expression = expression.And(t => t.Save_date >= sDate && t.Save_date <= eDate);
            return service.IQueryable(expression).ToList();
        }
        public string GetFlowRateDowdload(string Station_key,int mType, DateTime? sDate, DateTime? eDate)
        {
            string where = "";//水表数据过滤条件
            string name = "占比趋势分析";
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            if (!false)
            {
                //dt = new DateTime(_now.Year, _now.Month, _now.Day, _now.Hour, 0, 0);
                var tmpdt = bstcapp.GetList().Where(t => t.Tag_key.Equals(mType.ToString())).Max(item => item.Save_date);
                if (tmpdt != null)
                {
                    dt = tmpdt.Value;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                }
                if (sDate == null)
                {
                    sDate = dt.AddDays(-30);
                }
                if (eDate == null)
                {
                    eDate = dt;
                }
            }
            if (!string.IsNullOrWhiteSpace(Station_key))
            {
                where += " AND a.Station_Key IN('" + Station_key.Replace(",", "','") + "')";
            }
            else
            {
                return "";
            }
            //var bmi = bmiApp.GetList(Station_key).Select(i => i.Meter_Type).FirstOrDefault().ToInt();
            JArray nameK = JArray.FromObject(sysDicApp.GetItemList(14).Where(i => i.ItemID == mType).Select(i => i.ItemKey).FirstOrDefault().Split(','));
            JArray areaIds = JArray.FromObject(Station_key.Split(','));
            where += " AND Save_date>='" + sDate + "' AND Save_date<'" + eDate.Value.AddDays(1) + "'";
            string sql = "";
            if (mType == 1 || mType == 8 || mType == 9)
                sql = "SELECT Tag_value 瞬时流量,Tag_value_zx 正向累计,Tag_value_fx 反向累计,Save_date 数据时间,b.Meter_Name 监测点,b.Explain 数据类型 FROM BS_SCADA_TAG_HIS a," +
                    "(SELECT Meter_Name,Explain,Station_Key,meter_type FROM BSM_Meter_Info) b WHERE b.Station_Key=a.Station_Key AND Tag_key=meter_type AND Tag_key=" + mType + where + " ORDER BY Save_date";
            else
                sql = "SELECT Tag_value 数据值,Save_date 数据时间,b.Meter_Name 监测点,b.Explain 数据类型 FROM BS_SCADA_TAG_HIS a,(SELECT Meter_Name,Explain,Station_Key,meter_type " +
                    "FROM BSM_Meter_Info) b WHERE b.Station_Key=a.Station_Key AND Tag_key=meter_type AND Tag_key=" + mType + where + " ORDER BY Save_date";
            //string sql = "SELECT Save_date 日期时间,(SELECT Explain FROM BSM_Meter_Info WHERE Station_Key=Station_Key) 数据类型,(SELECT Meter_Name FROM BSM_Meter_Info " +
            //    "WHERE Station_Key=Station_Key) 监测点,Tag_value 原始数据,CleanedValue 清洗数据 FROM BS_SCADA_TAG_HIS " + where;
            DbCommand cmd = dbcontext.Database.Connection.CreateCommand();
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            //转成sqlcommand正常查询语句。
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd as SqlCommand;
            DataTable tmp1 = new DataTable();
            da.Fill(tmp1);
            if (nameK[0].ToString().Equals("all"))
            {
                nameK = JArray.FromObject(Station_key.Split(','));
                Station_key = Station_key.Replace(",", "','");
                sql = "SELECT CONVERT(DECIMAL(18,2),(CASE WHEN b.co=0 THEN 0 ELSE a.cc/b.co END)*100) '占比',(SELECT TOP 1 Meter_Name FROM BSM_Meter_Info m WHERE m.Station_Key =a.Station_Key AND Meter_Type=" + mType + ") '监测点'," +
                    "a.Save_date '数据时间' FROM(SELECT SUM(Tag_value) cc,Station_key,CONVERT(VARCHAR(16),Save_date,20) Save_date FROM [dbo].[BS_SCADA_TAG_HIS] WHERE Station_key " +
                    "IN('" + Station_key + "') AND Tag_key=" + mType + " AND Save_date>='" + sDate + "' AND Save_date<'" + eDate.Value.AddDays(1) + "' GROUP BY CONVERT(VARCHAR(16),Save_date,20),Station_key) a LEFT " +
                    "JOIN (SELECT SUM(Tag_value) co,CONVERT(VARCHAR(16),Save_date,20) Save_date FROM [dbo].[BS_SCADA_TAG_HIS] WHERE Station_key IN('" + Station_key + "') AND Tag_key=" + mType + " AND " +
                    "Save_date>='" + sDate + "' AND Save_date<'" + eDate.Value.AddDays(1) + "' GROUP BY CONVERT(VARCHAR(16),Save_date,20)) b ON a.Save_date=b.Save_date ORDER BY a.Save_date";
            }
            else
            {
                if (nameK.Count > 0)
                {
                    Station_key = Station_key.Replace(",", "','");
                    areaIds = JArray.FromObject(sysDicApp.GetItemList(14).Where(i => i.ItemID == mType).Select(i => i.ItemKey).FirstOrDefault().Split(','));
                    string select = "SELECT CONVERT(DECIMAL(18,2),CONVERT(DECIMAL,a.Rate)/CONVERT(DECIMAL,b.co)*100) '占比',a.Save_date+':00:00' '数据时间',a.Station_Key '区间' FROM(";
                    areaIds.Add(areaIds[areaIds.Count - 1].ToString());
                    nameK.Add(nameK[nameK.Count - 1].ToString());
                    for (int i = 0; i < areaIds.Count; i++)
                    {
                        if (i == 0)
                        {
                            sql = "SELECT COUNT(*) Rate,CONVERT(VARCHAR(13),Save_date,20) Save_date,'" + areaIds[i].ToString() + "' Station_Key FROM [dbo].[BS_SCADA_TAG_HIS] WHERE " +
                                "Tag_value<" + areaIds[i].ToString() + " AND Station_key IN('" + Station_key + "') AND Tag_key=" + mType + " AND Save_date>='" + sDate + "' AND Save_date<'" + eDate.Value.AddDays(1) + "' " +
                                "GROUP BY CONVERT(VARCHAR(13),Save_date,20)";
                            nameK[i] = "＜" + areaIds[i].ToString() + "";
                        }
                        else
                        {
                            if (i == areaIds.Count - 1)
                            {
                                sql += " UNION ALL (SELECT COUNT(*) c5,CONVERT(VARCHAR(13),Save_date,20) Save_date,'≥" + areaIds[i].ToString() + "' Station_Key FROM [dbo].[BS_SCADA_TAG_HIS] " +
                                    "WHERE Tag_value>=" + areaIds[i].ToString() + " AND Station_key IN('" + Station_key + "') AND Tag_key=" + mType + " AND Save_date>='" + sDate + "' AND Save_date<'" + eDate.Value.AddDays(1) + "' " +
                                    "GROUP BY CONVERT(VARCHAR(13),Save_date,20))) a LEFT JOIN (SELECT COUNT(*) co,CONVERT(VARCHAR(13),Save_date,20) Save_date FROM [dbo].[BS_SCADA_TAG_HIS] WHERE Station_key IN('" + Station_key + "') AND " +
                                    "Tag_key=" + mType + " AND Save_date>='" + sDate + "' AND Save_date<'" + eDate.Value.AddDays(1) + "' GROUP BY CONVERT(VARCHAR(13),Save_date,20)) b ON a.Save_date=b.Save_date ORDER BY a.Save_date";
                                nameK[i] = "≥" + areaIds[i].ToString();
                                areaIds[i] = "≥" + areaIds[i].ToString();
                            }
                            else
                            {

                                sql += " UNION ALL (SELECT COUNT(*) Rate,CONVERT(VARCHAR(13),Save_date,20) Save_date,'" + areaIds[i].ToString() + "' Station_Key FROM " +
                                    "[dbo].[BS_SCADA_TAG_HIS] WHERE Tag_value>=" + areaIds[i - 1].ToString() + " AND Tag_value<" + areaIds[i].ToString() + " AND Station_key IN('" + Station_key + "') AND " +
                                    "Tag_key=" + mType + " AND Save_date>='" + sDate + "' AND Save_date<'" + eDate.Value.AddDays(1) + "' GROUP BY CONVERT(VARCHAR(13),Save_date,20))";
                                nameK[i] = areaIds[i - 1].ToString() + "-" + areaIds[i].ToString();
                            }
                        }
                    }
                    sql = select + sql;
                }
            }
            //string sql = "SELECT Save_date 日期时间,(SELECT Explain FROM BSM_Meter_Info WHERE Station_Key=Station_Key) 数据类型,(SELECT Meter_Name FROM BSM_Meter_Info " +
            //    "WHERE Station_Key=Station_Key) 监测点,Tag_value 原始数据,CleanedValue 清洗数据 FROM BS_SCADA_TAG_HIS " + where;
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            //转成sqlcommand正常查询语句。
            da = new SqlDataAdapter();
            da.SelectCommand = cmd as SqlCommand;
            DataTable tmp2 = new DataTable();
            da.Fill(tmp2);
            var excel = new ExcelHelper
            {
                FilePath = "/file/"+ name + "明细表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx",
                SheetNames = new string[] { "原始数据", name },
                Titles = new string[] { "原始数据", name }
            };
            excel.AddTables(tmp1);
            excel.AddTables(tmp2);
            excel.CreateExcel();
            return excel.FilePath;
        }
        public string GetFlowDowdload(string Station_key,int mType,int? type, DateTime? sDate, DateTime? eDate)
        {
            string where = "";//水表数据过滤条件
            string where1 = "";//水表数据过滤条件
            string where2 = "";//水表数据过滤条件
            string name = "";
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            if (!false)
            {
                //dt = new DateTime(_now.Year, _now.Month, _now.Day, _now.Hour, 0, 0);
                var tmpdt = bstcapp.GetList().Where(t => t.Tag_key.Equals(mType.ToString())).Max(item => item.Save_date);
                if (tmpdt != null)
                {
                    dt = tmpdt.Value;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                }
                if (sDate == null)
                {
                    sDate = dt.AddDays(-30);
                }
                if (eDate == null)
                {
                    eDate = dt;
                }
            }
            if (!string.IsNullOrWhiteSpace(Station_key))
            {
                var skeys = Station_key.Split(',');
                for (var i = 0; i < skeys.Length; i++)
                {
                    if (skeys[i].Length > 0)
                    {
                        var sskey = skeys[i].Split('_');
                        if (i == 0)
                        {
                            if (skeys.Length == 1)
                                where += " AND (x.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                            else
                                where += " AND ((x.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        }
                        else if (i == skeys.Length - 1)
                        {
                            where += " OR (x.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + "))";
                        }
                        else
                        {
                            where += " OR (x.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        }
                    }
                }
                where1 = where;
                where2 = where;
            }
            switch (type)
            {
                case 1:
                    where += " AND Save_date>='" + sDate + "' AND Save_date<'" + eDate.Value.AddDays(1) + "'";
                    where1 += " AND Save_date>='" + sDate.Value.AddYears(-1) + "' AND Save_date<'" + eDate.Value.AddDays(1).AddYears(-1) + "'";
                    where2 += " AND Save_date>='" + sDate.Value.AddYears(-2) + "' AND Save_date<'" + eDate.Value.AddDays(1).AddYears(-2) + "'";
                    break;
                case 2:
                case 3:
                    where += " AND Save_date>='" + sDate + "' AND Save_date<'" + eDate.Value.AddMonths(1) + "'";
                    where1 += " AND Save_date>='" + sDate.Value.AddYears(-1) + "' AND Save_date<'" + eDate.Value.AddMonths(1).AddYears(-1) + "'";
                    where2 += " AND Save_date>='" + sDate.Value.AddYears(-2) + "' AND Save_date<'" + eDate.Value.AddMonths(1).AddYears(-2) + "'";
                    break;
                default:
                    where += " AND Save_date>='" + sDate + "' AND Save_date<'" + eDate.Value.AddDays(1) + "'";
                    where1 += " AND Save_date>='" + sDate.Value.AddYears(-1) + "' AND Save_date<'" + eDate.Value.AddDays(1).AddYears(-1) + "'";
                    where2 += " AND Save_date>='" + sDate.Value.AddYears(-2) + "' AND Save_date<'" + eDate.Value.AddDays(1).AddYears(-2) + "'";
                    break;
            }
            string sql = "";
            if (mType == 1 || mType == 8 || mType == 9)
                sql = "SELECT Tag_value 瞬时流量,Tag_value_zx 正向累计,Tag_value_fx 反向累计,Save_date 数据时间,b.Meter_Name 监测点,b.Explain 数据类型 FROM BS_SCADA_TAG_HIS x," +
                    "(SELECT Meter_Name,Explain,Station_Key FROM BSM_Meter_Info) b WHERE b.Station_Key=x.Station_Key" + where + " ORDER BY Save_date";
            else
                sql = "SELECT Tag_value 数据值,Save_date 数据时间,b.Meter_Name 监测点,b.Explain 数据类型 FROM BS_SCADA_TAG_HIS x,(SELECT Meter_Name,Explain,Station_Key " +
                    "FROM BSM_Meter_Info) b WHERE b.Station_Key=x.Station_Key" + where + " ORDER BY Save_date";
            //string sql = "SELECT Save_date 日期时间,(SELECT Explain FROM BSM_Meter_Info WHERE Station_Key=Station_Key) 数据类型,(SELECT Meter_Name FROM BSM_Meter_Info " +
            //    "WHERE Station_Key=Station_Key) 监测点,Tag_value 原始数据,CleanedValue 清洗数据 FROM BS_SCADA_TAG_HIS " + where;
            DbCommand cmd = dbcontext.Database.Connection.CreateCommand();
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            //转成sqlcommand正常查询语句。
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd as SqlCommand;
            DataTable tmp1 = new DataTable();
            da.Fill(tmp1);
            if (mType == 1 || mType == 8 || mType == 9)
                sql = "SELECT Tag_value 瞬时流量,Tag_value_zx 正向累计,Tag_value_fx 反向累计,Save_date 数据时间,b.Meter_Name 监测点,b.Explain 数据类型 FROM BS_SCADA_TAG_HIS x," +
                    "(SELECT Meter_Name,Explain,Station_Key FROM BSM_Meter_Info) b WHERE b.Station_Key=x.Station_Key" + where1 + " ORDER BY Save_date";
            else
                sql = "SELECT Tag_value 数据值,Save_date 数据时间,b.Meter_Name 监测点,b.Explain 数据类型 FROM BS_SCADA_TAG_HIS x,(SELECT Meter_Name,Explain,Station_Key " +
                    "FROM BSM_Meter_Info) b WHERE b.Station_Key=x.Station_Key" + where1 + " ORDER BY Save_date";
            //string sql = "SELECT Save_date 日期时间,(SELECT Explain FROM BSM_Meter_Info WHERE Station_Key=Station_Key) 数据类型,(SELECT Meter_Name FROM BSM_Meter_Info " +
            //    "WHERE Station_Key=Station_Key) 监测点,Tag_value 原始数据,CleanedValue 清洗数据 FROM BS_SCADA_TAG_HIS " + where;
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            //转成sqlcommand正常查询语句。
            da = new SqlDataAdapter();
            da.SelectCommand = cmd as SqlCommand;
            DataTable tmp2 = new DataTable();
            da.Fill(tmp2);
            if (mType == 1 || mType == 8 || mType == 9)
                sql = "SELECT Tag_value 瞬时流量,Tag_value_zx 正向累计,Tag_value_fx 反向累计,Save_date 数据时间,b.Meter_Name 监测点,b.Explain 数据类型 FROM BS_SCADA_TAG_HIS x," +
                    "(SELECT Meter_Name,Explain,Station_Key FROM BSM_Meter_Info) b WHERE b.Station_Key=x.Station_Key" + where2 + " ORDER BY Save_date";
            else
                sql = "SELECT Tag_value 数据值,Save_date 数据时间,b.Meter_Name 监测点,b.Explain 数据类型 FROM BS_SCADA_TAG_HIS x,(SELECT Meter_Name,Explain,Station_Key " +
                    "FROM BSM_Meter_Info) b WHERE b.Station_Key=x.Station_Key" + where2 + " ORDER BY Save_date";
            //string sql = "SELECT Save_date 日期时间,(SELECT Explain FROM BSM_Meter_Info WHERE Station_Key=Station_Key) 数据类型,(SELECT Meter_Name FROM BSM_Meter_Info " +
            //    "WHERE Station_Key=Station_Key) 监测点,Tag_value 原始数据,CleanedValue 清洗数据 FROM BS_SCADA_TAG_HIS " + where;
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            //转成sqlcommand正常查询语句。
            da = new SqlDataAdapter();
            da.SelectCommand = cmd as SqlCommand;
            DataTable tmp3 = new DataTable();
            da.Fill(tmp3);
            switch (type)
            {
                case 1:
                    if (mType == 1 || mType == 8 || mType == 9)
                        sql = "SELECT a.Save_date '时间',a.Tag_value '数值',b.Tag_value '去年',c.Tag_value '前年' FROM(SELECT Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10)," +
                        "Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) a1 GROUP BY " +
                        "Save_date) a LEFT JOIN (SELECT Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM " +
                        "BS_SCADA_TAG_HIS x WHERE 1=1" + where1 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) b1 GROUP BY Save_date) b ON RIGHT(a.Save_date,5)=RIGHT(b.Save_date,5) " +
                        "LEFT JOIN (SELECT Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM BS_SCADA_TAG_HIS x WHERE " +
                        "1=1" + where2 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) c1 GROUP BY Save_date) c ON RIGHT(a.Save_date,5)=RIGHT(c.Save_date,5) ORDER BY a.Save_date";
                    else
                        sql = "SELECT a.Save_date '时间',a.Tag_value '数值',b.Tag_value '去年',c.Tag_value '前年' FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(Tag_value) Tag_value" +
                        " FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where + " GROUP BY CONVERT(VARCHAR(10),Save_date,20)) a LEFT JOIN (SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date," +
                        "AVG(Tag_value) Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where1 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20)) b ON RIGHT(a.Save_date,5)=RIGHT(b.Save_date,5) " +
                        "LEFT JOIN (SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(Tag_value) Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where2 + " GROUP BY CONVERT(VARCHAR(10)," +
                        "Save_date,20)) c ON RIGHT(a.Save_date,5)=RIGHT(c.Save_date,5) ORDER BY a.Save_date";
                    name = "日同期对比";
                    break;
                case 2:
                    if (mType == 1 || mType == 8 || mType == 9)
                        sql = "SELECT a.Save_date '时间',a.Tag_value '数值',b.Tag_value '去年',c.Tag_value '前年' FROM(SELECT LEFT(Save_date,7) Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10)," +
                        "Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) a1 GROUP BY " +
                        "LEFT(Save_date,7)) a LEFT JOIN (SELECT LEFT(Save_date,7) Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM " +
                        "BS_SCADA_TAG_HIS x WHERE 1=1" + where1 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) b1 GROUP BY LEFT(Save_date,7)) b ON RIGHT(a.Save_date,2)=RIGHT(b.Save_date,2) " +
                        "LEFT JOIN (SELECT LEFT(Save_date,7) Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM BS_SCADA_TAG_HIS x WHERE " +
                        "1=1" + where2 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) c1 GROUP BY LEFT(Save_date,7)) c ON RIGHT(a.Save_date,2)=RIGHT(c.Save_date,2) ORDER BY a.Save_date";
                    else
                        sql = "SELECT a.Save_date '时间',a.Tag_value '数值',b.Tag_value '去年',c.Tag_value '前年' FROM(SELECT CONVERT(VARCHAR(7),Save_date,20) Save_date,AVG(Tag_value) Tag_value " +
                        "FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where + " GROUP BY CONVERT(VARCHAR(7),Save_date,20)) a LEFT JOIN (SELECT CONVERT(VARCHAR(7),Save_date,20) Save_date,AVG(Tag_value) " +
                        "Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where1 + " GROUP BY CONVERT(VARCHAR(7),Save_date,20)) b ON RIGHT(a.Save_date,2)=RIGHT(b.Save_date,2) LEFT JOIN (SELECT " +
                        "CONVERT(VARCHAR(7),Save_date,20) Save_date,AVG(Tag_value) Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where2 + " GROUP BY CONVERT(VARCHAR(7),Save_date,20)) c ON " +
                        "RIGHT(a.Save_date,2)=RIGHT(c.Save_date,2) ORDER BY a.Save_date";
                    name = "月同期对比";
                    break;
                case 3:
                    if (mType == 1 || mType == 8 || mType == 9)
                        sql = "SELECT CONCAT(LEFT(Save_date,4),'年第',CEILING((RIGHT(Save_date,2)+2) /3),'季度') AS '时间', SUM(v1) '数值',SUM(v2) '去年',SUM(v3) '前年' FROM (SELECT a.Save_date ," +
                        "a.Tag_value v1,b.Tag_value v2,c.Tag_value v3 FROM(FROM(SELECT LEFT(Save_date,7) Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10)," +
                        "Save_date,20) Save_date,AVG(Tag_value)*24 Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) a1 GROUP BY " +
                        "LEFT(Save_date,7)) a LEFT JOIN (SELECT LEFT(Save_date,7) Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(Tag_value)*24 Tag_value FROM " +
                        "BS_SCADA_TAG_HIS x WHERE 1=1" + where1 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) b1 GROUP BY LEFT(Save_date,7)) b ON RIGHT(a.Save_date,2)=RIGHT(b.Save_date,2) " +
                        "LEFT JOIN (SELECT LEFT(Save_date,7) Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(Tag_value)*24 Tag_value FROM BS_SCADA_TAG_HIS x WHERE " +
                        "1=1" + where2 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) c1 GROUP BY LEFT(Save_date,7)) c ON RIGHT(a.Save_date,2)=RIGHT(c.Save_date,2)) d " +
                        "GROUP BY CONCAT(LEFT(Save_date,4),'年第',CEILING((RIGHT(Save_date,2)+2) /3),'季度') ORDER BY Save_date";
                    else
                        sql = "SELECT CONCAT(LEFT(Save_date,4),'年第',CEILING((RIGHT(Save_date,2)+2) /3),'季度') AS '时间', AVG(v1) '数值',AVG(v2) '去年',AVG(v3) '前年' FROM (SELECT a.Save_date ," +
                        "a.Tag_value v1,b.Tag_value v2,c.Tag_value v3 FROM(FROM(SELECT CONVERT(VARCHAR(7),Save_date,20) Save_date,AVG(ABS(Tag_value)) Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + 
                        where + " GROUP BY CONVERT(VARCHAR(7),Save_date,20)) a LEFT JOIN (SELECT CONVERT(VARCHAR(7),Save_date,20) Save_date,AVG(ABS(Tag_value)) Tag_value FROM BS_SCADA_TAG_HIS x WHERE " +
                        "1=1" + where1 + " GROUP BY CONVERT(VARCHAR(7),Save_date,20)) b ON RIGHT(a.Save_date,2)=RIGHT(b.Save_date,2) LEFT JOIN (SELECT CONVERT(VARCHAR(7),Save_date,20) Save_date," +
                        "AVG(ABS(Tag_value)) Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where2 + " GROUP BY CONVERT(VARCHAR(7),Save_date,20)) c ON RIGHT(a.Save_date,2)=RIGHT(c.Save_date,2)) d " +
                        "GROUP BY CONCAT(LEFT(Save_date,4),'年第',CEILING((RIGHT(Save_date,2)+2) /3),'季度') ORDER BY Save_date";
                    name = "季度同期对比";
                    break;
                default:
                    if (mType == 1 || mType == 8 || mType == 9)
                        sql = "SELECT a.Save_date '时间',a.Tag_value '数值',b.Tag_value '去年',c.Tag_value '前年' FROM(SELECT Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10)," +
                        "Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) a1 GROUP BY " +
                        "Save_date) a LEFT JOIN (SELECT Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM " +
                        "BS_SCADA_TAG_HIS x WHERE 1=1" + where1 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) b1 GROUP BY Save_date) b ON RIGHT(a.Save_date,5)=RIGHT(b.Save_date,5) " +
                        "LEFT JOIN (SELECT Save_date,SUM(Tag_value) Tag_value FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(ABS(Tag_value))*24 Tag_value FROM BS_SCADA_TAG_HIS x WHERE " +
                        "1=1" + where2 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20),Station_key) c1 GROUP BY Save_date) c ON RIGHT(a.Save_date,5)=RIGHT(c.Save_date,5) ORDER BY a.Save_date";
                    else
                        sql = "SELECT a.Save_date '时间',a.Tag_value '数值',b.Tag_value '去年',c.Tag_value '前年' FROM(SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(Tag_value) Tag_value " +
                            "FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where + " GROUP BY CONVERT(VARCHAR(10),Save_date,20)) a LEFT JOIN (SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date," +
                            "AVG(Tag_value) Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where1 + " GROUP BY CONVERT(VARCHAR(10),Save_date,20)) b ON RIGHT(a.Save_date,5)=RIGHT(b.Save_date,5) " +
                        "LEFT JOIN (SELECT CONVERT(VARCHAR(10),Save_date,20) Save_date,AVG(Tag_value) Tag_value FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where2 + " GROUP BY CONVERT(VARCHAR(10)," +
                        "Save_date,20)) c ON RIGHT(a.Save_date,5)=RIGHT(c.Save_date,5) ORDER BY a.Save_date";
                    name = "日同期对比";
                    break;
            }
            //string sql = "SELECT Save_date 日期时间,(SELECT Explain FROM BSM_Meter_Info WHERE Station_Key=Station_Key) 数据类型,(SELECT Meter_Name FROM BSM_Meter_Info " +
            //    "WHERE Station_Key=Station_Key) 监测点,Tag_value 原始数据,CleanedValue 清洗数据 FROM BS_SCADA_TAG_HIS " + where;
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            //转成sqlcommand正常查询语句。
            da = new SqlDataAdapter();
            da.SelectCommand = cmd as SqlCommand;
            DataTable tmp4 = new DataTable();
            da.Fill(tmp4);
            var excel = new ExcelHelper
            {
                FilePath = "/file/"+ name + "明细表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx",
                SheetNames = new string[] { "原始数据", "去年原始数据", "前年原始数据", name },
                Titles = new string[] { "原始数据", "去年原始数据", "前年原始数据", name }
            };
            excel.AddTables(tmp1);
            excel.AddTables(tmp2);
            excel.AddTables(tmp3);
            excel.AddTables(tmp4);
            excel.CreateExcel();
            return excel.FilePath;
        }
        public string GetMaxMinDowdload(string Station_key, int mType, int? type, DateTime? sDate, DateTime? eDate)
        {
            string where = "";//水表数据过滤条件
            string name = "";
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            if (true)
            {
                //dt = new DateTime(_now.Year, _now.Month, _now.Day, _now.Hour, 0, 0);
                var tmpdt = bstcapp.GetList().Where(t => t.Tag_key.Equals(mType.ToString())).Max(item => item.Save_date);
                if (tmpdt != null)
                {
                    dt = tmpdt.Value;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                }
                if (sDate == null)
                {
                    sDate = dt.AddDays(-30);
                }
                if (eDate == null)
                {
                    eDate = dt;
                }
            }
            if (!string.IsNullOrWhiteSpace(Station_key))
            {
                var skeys = Station_key.Split(',');
                for (var i = 0; i < skeys.Length; i++)
                {
                    if (skeys[i].Length > 0)
                    {
                        var sskey = skeys[i].Split('_');
                        if (i == 0)
                        {
                            if (skeys.Length == 1)
                                where += " AND (x.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                            else
                                where += " AND ((x.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        }
                        else if (i == skeys.Length - 1)
                        {
                            where += " OR (x.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + "))";
                        }
                        else
                        {
                            where += " OR (x.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        }
                    }
                }
            }
            where += " AND Save_date>='" + sDate + "' AND Save_date<='" + eDate + "'";
            string sql = "";
            if (mType == 1 || mType == 8 || mType == 9)
                sql = "SELECT Tag_value 瞬时流量,Tag_value_zx 正向累计,Tag_value_fx 反向累计,Save_date 数据时间,b.Meter_Name 监测点,b.Explain 数据类型 FROM BS_SCADA_TAG_HIS x," +
                    "(SELECT Meter_Name,Explain,Station_Key,meter_type,DistrictAreaId FROM BSM_Meter_Info) b WHERE b.Station_Key=x.Station_Key AND x.Tag_key=b.meter_type " +
                    "AND x.Number=b.DistrictAreaId" + where + " ORDER BY Save_date";
            else
                sql = "SELECT Tag_value 数据值,Save_date 数据时间,b.Meter_Name 监测点,b.Explain 数据类型 FROM BS_SCADA_TAG_HIS x,(SELECT Meter_Name,Explain,Station_Key,meter_type,DistrictAreaId " +
                    "FROM BSM_Meter_Info) b WHERE b.Station_Key=x.Station_Key AND x.Tag_key=b.meter_type AND x.Number=b.DistrictAreaId" + where + " ORDER BY Save_date";
            //string sql = "SELECT Save_date 日期时间,(SELECT Explain FROM BSM_Meter_Info WHERE Station_Key=Station_Key) 数据类型,(SELECT Meter_Name FROM BSM_Meter_Info " +
            //    "WHERE Station_Key=Station_Key) 监测点,Tag_value 原始数据,CleanedValue 清洗数据 FROM BS_SCADA_TAG_HIS " + where;
            DbCommand cmd = dbcontext.Database.Connection.CreateCommand();
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            //转成sqlcommand正常查询语句。
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd as SqlCommand;
            DataTable tmp1 = new DataTable();
            da.Fill(tmp1);
            //switch (type)
            //{
            //    case 1:
            //        sql = "SELECT CONVERT(VARCHAR(10),Save_date,20) '时间',MAX(ABS(Tag_value)) '最大值',MIN(ABS(Tag_value)) '最小值' FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where + 
            //            " GROUP BY CONVERT(VARCHAR(10),Save_date,20) ORDER BY CONVERT(VARCHAR(10),Save_date,20)";
            //        name = "日最值趋势";
            //        break;
            //    case 2:
            //        sql = "SELECT CONVERT(VARCHAR(7),Save_date,20) '时间',MAX(Tag_value) '最大值',MIN(Tag_value) '最小值' FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where +
            //            " GROUP BY CONVERT(VARCHAR(7),Save_date,20) ORDER BY CONVERT(VARCHAR(7),Save_date,20)";
            //        name = "月最值趋势";
            //        break;
            //    case 3:
            //        sql = "SELECT CONCAT(LEFT(datatime,4),'年第',CEILING((RIGHT(datatime,2)+2) /3),'季度') AS '时间', MAX(maxdata) '最大值',MIN(mindata) '最小值' FROM (SELECT " +
            //            "CONVERT(VARCHAR(7),Save_date,20) datatime,MAX(Tag_value) maxdata,MIN(Tag_value) mindata FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where + " GROUP BY " +
            //            "CONVERT(VARCHAR(7),Save_date,20)) a GROUP BY CONCAT(LEFT(datatime,4),'年第',CEILING((RIGHT(datatime,2)+2) /3),'季度')";
            //        name = "季度最值趋势";
            //        break;
            //    default:
            sql = "SELECT CONVERT(VARCHAR(13),Save_date,20) '时间',MAX(Tag_value) '最大值',MIN(Tag_value) '最小值' FROM BS_SCADA_TAG_HIS x WHERE 1=1" + where +
                " GROUP BY CONVERT(VARCHAR(13),Save_date,20) ORDER BY CONVERT(VARCHAR(13),Save_date,20)";
            name = "小时最值趋势";
            //        break;
            //}
            //string sql = "SELECT Save_date 日期时间,(SELECT Explain FROM BSM_Meter_Info WHERE Station_Key=Station_Key) 数据类型,(SELECT Meter_Name FROM BSM_Meter_Info " +
            //    "WHERE Station_Key=Station_Key) 监测点,Tag_value 原始数据,CleanedValue 清洗数据 FROM BS_SCADA_TAG_HIS " + where;
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            //转成sqlcommand正常查询语句。
            da = new SqlDataAdapter();
            da.SelectCommand = cmd as SqlCommand;
            DataTable tmp2 = new DataTable();
            da.Fill(tmp2);
            var excel = new ExcelHelper
            {
                FilePath = "/file/"+ name + "明细表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx",
                SheetNames = new string[] { "原始数据", name },
                Titles = new string[] { "原始数据", name }
            };
            excel.AddTables(tmp1);
            excel.AddTables(tmp2);
            excel.CreateExcel();
            return excel.FilePath;
        }
        /// <summary>
        /// 分页查询SCADA数据记录
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="Station_key"></param>
        /// <returns></returns>
        public List<BS_SCADA_TAG_HISEntity> GetList(Pagination pagination, string Station_key)
        {
            var expression = ExtLinq.True<BS_SCADA_TAG_HISEntity>();
            if (!string.IsNullOrEmpty(Station_key))
            {
                expression = expression.And(t => t.Station_key.Equals(Station_key));
            }
            return service.FindList(expression, pagination).ToList();
        }

        /// <summary>
        /// 分页查询SCADA数据记录
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public List<BsTagHisResult> GetList(Pagination pagination, string sks, DateTime? sDate, DateTime? eDate)
        {
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            if (!string.IsNullOrWhiteSpace(sks))
            {
                string tkey = sks.Split(',')[0].Split('_')[2].ToString();
                string tid = sks.Split(',')[0].Split('_')[1].ToString();
                var tmpdt = bstcapp.GetList().Where(i => i.Tag_key.Equals(tkey) && i.GYStationId.Equals(tid)).Max(item => item.Save_date);
                //var tmpdt = bstcapp.GetList().Where(i => i.Tag_key.Equals(tkey)).Max(item => item.Save_date);
                if (tmpdt != null)
                {
                    dt = tmpdt.Value;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                }
                if (sDate == null)
                {
                    sDate = dt.AddDays(-30);
                }
                if (eDate == null)
                {
                    eDate = dt;
                }
            }
            string where = "WHERE a.Station_Key=b.Station_Key AND a.Tag_key=b.meter_type AND a.Number=b.DistrictAreaId AND CleanedValue IS NOT NULL";//流量计数据查询条件
            if (!string.IsNullOrWhiteSpace(sks))
            {
                var skeys = sks.Split(',');
                for (var i = 0; i < skeys.Length; i++)
                {
                    if (skeys[i].Length > 0)
                    {
                        var sskey = skeys[i].Split('_');
                        if (i == 0)
                        {
                            if (skeys.Length == 1)
                                where += " AND (b.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                            else
                                where += " AND ((b.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        }
                        else if (i == skeys.Length - 1)
                        {
                            where += " OR (b.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + "))";
                        }
                        else
                        {
                            where += " OR (b.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        }
                    }
                }
            }
            if (sDate != null || eDate != null)
            {
                if (sDate == null && eDate != null)
                {
                    where += " AND Save_date<='" + eDate + "'";
                }
                else if (sDate != null && eDate == null)
                {
                    where += " AND Save_date>='" + sDate + "'";
                }
                else
                {
                    where += " AND Save_date>='" + sDate + "' AND Save_date<='" + eDate + "'";
                }
            }
            //where += " AND Save_date>='" + sDate + "' AND Save_date<='" + eDate + "'";
            string sql = "SELECT a.ID,Save_date,a.Station_Key,Meter_Name,Tag_value,CONVERT(DECIMAL(15,3),CleanedValue) CleanedValue,Explain Tag_key FROM BS_SCADA_TAG_HIS a,BSM_Meter_Info b " + where;
            bool isAsc = pagination.sord.ToLower() == "asc" ? true : false;
            string[] _order = pagination.sidx.Split(',');
            MethodCallExpression resultExp = null;
            dbcontext.Database.CommandTimeout = int.MaxValue;
            var tempData = dbcontext.Database.SqlQuery<BsTagHisResult>(sql).AsQueryable();
            foreach (string item in _order)
            {
                string _orderPart = item;
                _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
                string[] _orderArry = _orderPart.Split(' ');
                string _orderField = _orderArry[0];
                bool sort = isAsc;
                if (_orderArry.Length == 2)
                {
                    isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
                }
                var parameter = Expression.Parameter(typeof(BsTagHisResult), "t");
                var property = typeof(BsTagHisResult).GetProperty(_orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(BsTagHisResult), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
            }
            tempData = tempData.Provider.CreateQuery<BsTagHisResult>(resultExp);
            pagination.records = tempData.Count();
            tempData = tempData.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).AsQueryable();
            return tempData.ToList();
        }
        public string GetDownload(string sks, DateTime? sDate, DateTime? eDate)
        {
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            if (!string.IsNullOrWhiteSpace(sks))
            {
                string tkey = sks.Split(',')[0].Split('_')[2].ToString();
                var tmpdt = bstcapp.GetList().Where(i => i.Tag_key.Equals(tkey)).Max(item => item.Save_date);
                if (tmpdt != null)
                {
                    dt = tmpdt.Value;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                }
                if (sDate == null)
                {
                    sDate = dt.AddDays(-30);
                }
                if (eDate == null)
                {
                    eDate = dt;
                }
            }
            string where = "WHERE a.Station_Key=b.Station_Key AND a.Tag_key=b.meter_type AND a.Number=b.DistrictAreaId AND CleanedValue IS NOT NULL";//流量计数据查询条件
            if (!string.IsNullOrWhiteSpace(sks))
            {
                var skeys = sks.Split(',');
                for (var i = 0; i < skeys.Length; i++)
                {
                    if (skeys[i].Length > 0)
                    {
                        var sskey = skeys[i].Split('_');
                        if (i == 0)
                        {
                            if (skeys.Length == 1)
                                where += " AND (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                            else
                                where += " AND ((a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        }
                        else if (i == skeys.Length - 1)
                        {
                            where += " OR (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + "))";
                        }
                        else
                        {
                            where += " OR (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        }
                    }
                }
            }
            if (sDate != null || eDate != null)
            {
                if (sDate == null && eDate != null)
                {
                    where += " AND Save_date<='" + eDate + "'";
                }
                else if (sDate != null && eDate == null)
                {
                    where += " AND Save_date>='" + sDate + "'";
                }
                else
                {
                    where += " AND Save_date>='" + sDate + "' AND Save_date<='" + eDate + "'";
                }
            }
            //where += " AND Save_date>='" + sDate + "' AND Save_date<='" + eDate + "'";
            string sql = "SELECT Save_date 日期时间,Explain 数据类型,Meter_Name 监测点,Tag_value 原始数据,CONVERT(DECIMAL(15,3),CleanedValue) 清洗数据 FROM BS_SCADA_TAG_HIS a,BSM_Meter_Info b " + where+ " ORDER BY Save_date";
            DbCommand cmd = dbcontext.Database.Connection.CreateCommand();
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            //转成sqlcommand正常查询语句。
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd as SqlCommand;
            DataTable tmp1 = new DataTable();
            da.Fill(tmp1);
            var excel = new ExcelHelper
            {
                FilePath = "/file/清洗数据" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx",
                SheetNames = new string[] { "清洗数据" },
                Titles = new string[] { "清洗数据" }
            };
            excel.AddTables(tmp1);
            excel.CreateExcel();
            return excel.FilePath;
        }

        /// <summary>
        /// 获取SCADA
        /// </summary>
        /// <param name="Station_key"></param>
        /// <param name="Tag_key"></param>
        /// <returns></returns>
        public List<BS_SCADA_TAG_HISEntity> GetList(string Station_key, string Tag_key, int number, DateTime? sDate, DateTime? eDate)
        {
            var expression = ExtLinq.True<BS_SCADA_TAG_HISEntity>();
            if (Station_key != null)
            {
                expression = expression.And(t => t.Station_key.Equals(Station_key));
            }
            if (Tag_key != null)
            {
                expression = expression.And(t => t.Tag_key.Equals(Tag_key));
            }
            expression = expression.And(t => t.Number == number);
            if (sDate != null || eDate != null)
            {
                if (sDate == null && eDate != null)
                    expression = expression.And(t => t.Save_date <= eDate);
                else if (sDate != null && eDate == null)
                    expression = expression.And(t => t.Save_date >= sDate);
                else
                    expression = expression.And(t => t.Save_date >= sDate && t.Save_date <= eDate);
            }
            return service.IQueryable(expression).ToList();
        }

        public List<BS_SCADA_TAG_HISEntity> GetANDList(Pagination pagination, string sTKey, decimal? max, decimal? min, DateTime? sDate, DateTime? eDate, TimeSpan? sTime, TimeSpan? eTime)
        {
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            if (!string.IsNullOrWhiteSpace(sTKey))
            {
                string tkey = sTKey.Split(',')[0].Split('_')[2].ToString();
                string tid = sTKey.Split(',')[0].Split('_')[1].ToString();
                var tmpdt = bstcapp.GetList().Where(i => i.Tag_key.Equals(tkey) && i.GYStationId.Equals(tid)).Max(item => item.Save_date);
                if (tmpdt != null)
                {
                    dt = tmpdt.Value;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                }
                if (sDate == null)
                {
                    sDate = dt.AddHours(-2);
                }
                if (eDate == null)
                {
                    eDate = dt;
                }
            }
            else return new List<BS_SCADA_TAG_HISEntity>();
            string where = "WHERE 1=1";//水表数据过滤条件
            if (sDate != null || eDate != null)
            {
                if (sDate == null && eDate != null)
                {
                    where += " AND Save_date<'" + eDate.Value.AddDays(1) + "'";
                }
                else if (sDate != null && eDate == null)
                {
                    where += " AND Save_date>='" + sDate + "'";
                }
                else
                {
                    where += " AND Save_date>='" + sDate + "' AND Save_date<'" + eDate.Value.AddDays(1) + "'";
                }
            }
            if (!string.IsNullOrWhiteSpace(sTKey))
            {
                var skeys = sTKey.Split(',');
                for (var i=0;i< skeys.Length;i++)
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
            if (min != null || max != null)
            {
                if (min == null && max != null)
                {
                        where += " AND Tag_value<='" + max + "'";
                }
                else if (min != null && max == null)
                {
                        where += " AND Tag_value>='" + min + "'";
                }
                else
                {
                        where += " AND Tag_value>='" + min + "' AND Tag_value<='" + max + "'";
                }
            }
            if (sTime != null || eTime != null)
            {
                if (sTime == null && eTime != null)
                {
                        where += " AND CONVERT(TIME,Save_date)<='" + eTime + "'";
                }
                else if (sTime != null && eTime == null)
                {
                        where += " AND CONVERT(TIME,Save_date)>='" + sTime + "'";
                }
                else
                {
                        where += " AND CONVERT(TIME,Save_date)>='" + sTime + "' AND CONVERT(TIME,Save_date)<='" + eTime + "'";
                }
            }
            string sql = "SELECT ID,Tag_value_ss,Tag_value_zx,Tag_value_fx,Tag_value,Save_date,(SELECT TOP 1 Meter_Name FROM BSM_Meter_Info WHERE Station_Key=a.Station_Key AND Meter_Type=Tag_key " +
                "AND DistrictAreaId=Number) Station_Key,Tag_key,CleanedValue,ModifiedValue,Number FROM BS_SCADA_TAG_HIS a " + where;
            bool isAsc = pagination.sord.ToLower() == "asc" ? true : false;
            string[] _order = pagination.sidx.Split(',');
            MethodCallExpression resultExp = null;
            dbcontext.Database.CommandTimeout = int.MaxValue;
            var tempData = dbcontext.Database.SqlQuery<BS_SCADA_TAG_HISEntity>(sql).AsQueryable();
            foreach (string item in _order)
            {
                string _orderPart = item;
                _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
                string[] _orderArry = _orderPart.Split(' ');
                string _orderField = _orderArry[0];
                bool sort = isAsc;
                if (_orderArry.Length == 2)
                {
                    isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
                }
                var parameter = Expression.Parameter(typeof(BS_SCADA_TAG_HISEntity), "t");
                var property = typeof(BS_SCADA_TAG_HISEntity).GetProperty(_orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(BS_SCADA_TAG_HISEntity), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
            }
            tempData = tempData.Provider.CreateQuery<BS_SCADA_TAG_HISEntity>(resultExp);
            pagination.records = tempData.Count();
            tempData = tempData.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).AsQueryable();
            return tempData.ToList();
        }
        public string GetSumValue(int type, string sTKey, decimal? max, decimal? min, DateTime? sDate, DateTime? eDate, TimeSpan? sTime, TimeSpan? eTime)
        {
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            if (!string.IsNullOrWhiteSpace(sTKey))
            {
                string tkey = sTKey.Split(',')[0].Split('_')[2].ToString();
                string tid = sTKey.Split(',')[0].Split('_')[1].ToString();
                var tmpdt = bstcapp.GetList().Where(i => i.Tag_key.Equals(tkey) && i.GYStationId.Equals(tid)).Max(item => item.Save_date);
                if (tmpdt != null)
                {
                    dt = tmpdt.Value;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                }
                if (sDate == null)
                {
                    sDate = dt.AddHours(-2);
                }
                if (eDate == null)
                {
                    eDate = dt;
                }
            }
            string where = "";//水表数据过滤条件
            if (sDate != null || eDate != null)
            {
                if (sDate == null && eDate != null)
                {
                    where += " AND Save_date<'" + eDate.Value.AddDays(1) + "'";
                }
                else if (sDate != null && eDate == null)
                {
                    where += " AND Save_date>='" + sDate + "'";
                }
                else
                {
                    where += " AND Save_date>='" + sDate + "' AND Save_date<'" + eDate.Value.AddDays(1) + "'";
                }
            }
            if (!string.IsNullOrWhiteSpace(sTKey))
            {
                var skeys = sTKey.Split(',');
                for (var i = 0; i < skeys.Length; i++)
                {
                    var sskey = skeys[i].Split('_');
                    if (i == 0)
                    {
                        if (skeys.Length == 1)
                            where += " AND (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        else
                            where += " AND ((a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                    }
                    else if (i == skeys.Length - 1)
                    {
                        where += " OR (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + "))";
                    }
                    else
                    {
                        where += " OR (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                    }
                }
            }
            if (min != null || max != null)
            {
                if (min == null && max != null)
                {
                    where += " AND Tag_value<='" + max + "'";
                }
                else if (min != null && max == null)
                {
                    where += " AND Tag_value>'" + min + "'";
                }
                else
                {
                    where += " AND Tag_value>'" + min + "' AND Tag_value<='" + max + "'";
                }
            }
            if (sTime != null || eTime != null)
            {
                if (sTime == null && eTime != null)
                {
                    where += " AND CONVERT(TIME,Save_date)<='" + eTime + "'";
                }
                else if (sTime != null && eTime == null)
                {
                    where += " AND CONVERT(TIME,Save_date)>='" + sTime + "'";
                }
                else
                {
                    where += " AND CONVERT(TIME,Save_date)>='" + sTime + "' AND CONVERT(TIME,Save_date)<='" + eTime + "'";
                }
            }
            string sql = "SELECT SUM(Tag_value) FROM BS_SCADA_TAG_HIS a,(SELECT Meter_Name,Explain,Station_Key,DistrictAreaId,meter_type FROM BSM_Meter_Info) b " +
            "WHERE b.Station_Key=a.Station_Key AND a.Tag_key=b.meter_type AND a.Number=b.DistrictAreaId" + where;
            var tempData = dbcontext.Database.SqlQuery<decimal>(sql).FirstOrDefault().ToString("0.###");
            return tempData;
        }
        public string GetAvgValue(int type, string sTKey, decimal? max, decimal? min, DateTime? sDate, DateTime? eDate, TimeSpan? sTime, TimeSpan? eTime)
        {
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            if (!string.IsNullOrWhiteSpace(sTKey))
            {
                string tkey = sTKey.Split(',')[0].Split('_')[2].ToString();
                string tid = sTKey.Split(',')[0].Split('_')[1].ToString();
                var tmpdt = bstcapp.GetList().Where(i => i.Tag_key.Equals(tkey) && i.GYStationId.Equals(tid)).Max(item => item.Save_date);
                if (tmpdt != null)
                {
                    dt = tmpdt.Value;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                }
                if (sDate == null)
                {
                    sDate = dt.AddHours(-2);
                }
                if (eDate == null)
                {
                    eDate = dt;
                }
            }
            string where = "";//水表数据过滤条件
            if (sDate != null || eDate != null)
            {
                if (sDate == null && eDate != null)
                {
                    where += " AND Save_date<'" + eDate.Value.AddDays(1) + "'";
                }
                else if (sDate != null && eDate == null)
                {
                    where += " AND Save_date>='" + sDate + "'";
                }
                else
                {
                    where += " AND Save_date>='" + sDate + "' AND Save_date<'" + eDate.Value.AddDays(1) + "'";
                }
            }
            if (!string.IsNullOrWhiteSpace(sTKey))
            {
                var skeys = sTKey.Split(',');
                for (var i = 0; i < skeys.Length; i++)
                {
                    var sskey = skeys[i].Split('_');
                    if (i == 0)
                    {
                        if (skeys.Length == 1)
                            where += " AND (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        else
                            where += " AND ((a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                    }
                    else if (i == skeys.Length - 1)
                    {
                        where += " OR (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + "))";
                    }
                    else
                    {
                        where += " OR (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                    }
                }
            }
            if (min != null || max != null)
            {
                if (min == null && max != null)
                {
                    where += " AND Tag_value<='" + max + "'";
                }
                else if (min != null && max == null)
                {
                    where += " AND Tag_value>'" + min + "'";
                }
                else
                {
                    where += " AND Tag_value>'" + min + "' AND Tag_value<='" + max + "'";
                }
            }
            if (sTime != null || eTime != null)
            {
                if (sTime == null && eTime != null)
                {
                    where += " AND CONVERT(TIME,Save_date)<='" + eTime + "'";
                }
                else if (sTime != null && eTime == null)
                {
                    where += " AND CONVERT(TIME,Save_date)>='" + sTime + "'";
                }
                else
                {
                    where += " AND CONVERT(TIME,Save_date)>='" + sTime + "' AND CONVERT(TIME,Save_date)<='" + eTime + "'";
                }
            }
            string sql = "SELECT AVG(Tag_value) FROM BS_SCADA_TAG_HIS a,(SELECT Meter_Name,Explain,Station_Key,DistrictAreaId,meter_type FROM BSM_Meter_Info) b " +
            "WHERE b.Station_Key=a.Station_Key AND a.Tag_key=b.meter_type AND a.Number=b.DistrictAreaId" + where;
            var tempData = dbcontext.Database.SqlQuery<decimal>(sql).FirstOrDefault().ToString("0.###");
            return tempData;
        }
        public string GetMaxValue(int type, string sTKey, decimal? max, decimal? min, DateTime? sDate, DateTime? eDate, TimeSpan? sTime, TimeSpan? eTime)
        {
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            if (!string.IsNullOrWhiteSpace(sTKey))
            {
                string tkey = sTKey.Split(',')[0].Split('_')[2].ToString();
                string tid = sTKey.Split(',')[0].Split('_')[1].ToString();
                var tmpdt = bstcapp.GetList().Where(i => i.Tag_key.Equals(tkey) && i.GYStationId.Equals(tid)).Max(item => item.Save_date);
                if (tmpdt != null)
                {
                    dt = tmpdt.Value;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                }
                if (sDate == null)
                {
                    sDate = dt.AddHours(-2);
                }
                if (eDate == null)
                {
                    eDate = dt;
                }
            }
            string where = "";//水表数据过滤条件
            if (sDate != null || eDate != null)
            {
                if (sDate == null && eDate != null)
                {
                    where += " AND Save_date<'" + eDate.Value.AddDays(1) + "'";
                }
                else if (sDate != null && eDate == null)
                {
                    where += " AND Save_date>='" + sDate + "'";
                }
                else
                {
                    where += " AND Save_date>='" + sDate + "' AND Save_date<'" + eDate.Value.AddDays(1) + "'";
                }
            }
            if (!string.IsNullOrWhiteSpace(sTKey))
            {
                var skeys = sTKey.Split(',');
                for (var i = 0; i < skeys.Length; i++)
                {
                    var sskey = skeys[i].Split('_');
                    if (i == 0)
                    {
                        if (skeys.Length == 1)
                            where += " AND (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        else
                            where += " AND ((a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                    }
                    else if (i == skeys.Length - 1)
                    {
                        where += " OR (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + "))";
                    }
                    else
                    {
                        where += " OR (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                    }
                }
            }
            if (min != null || max != null)
            {
                if (min == null && max != null)
                {
                    where += " AND Tag_value<='" + max + "'";
                }
                else if (min != null && max == null)
                {
                    where += " AND Tag_value>'" + min + "'";
                }
                else
                {
                    where += " AND Tag_value>'" + min + "' AND Tag_value<='" + max + "'";
                }
            }
            if (sTime != null || eTime != null)
            {
                if (sTime == null && eTime != null)
                {
                    where += " AND CONVERT(TIME,Save_date)<='" + eTime + "'";
                }
                else if (sTime != null && eTime == null)
                {
                    where += " AND CONVERT(TIME,Save_date)>='" + sTime + "'";
                }
                else
                {
                    where += " AND CONVERT(TIME,Save_date)>='" + sTime + "' AND CONVERT(TIME,Save_date)<='" + eTime + "'";
                }
            }
            string sql = "SELECT MAX(Tag_value) FROM BS_SCADA_TAG_HIS a,(SELECT Meter_Name,Explain,Station_Key,DistrictAreaId,meter_type FROM BSM_Meter_Info) b " +
            "WHERE b.Station_Key=a.Station_Key AND a.Tag_key=b.meter_type AND a.Number=b.DistrictAreaId" + where;
            var tempData = dbcontext.Database.SqlQuery<decimal>(sql).FirstOrDefault().ToString("0.###");
            return tempData;
        }
        public string GetMinValue(int type, string sTKey, decimal? max, decimal? min, DateTime? sDate, DateTime? eDate, TimeSpan? sTime, TimeSpan? eTime)
        {
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            if (!string.IsNullOrWhiteSpace(sTKey))
            {
                string tkey = sTKey.Split(',')[0].Split('_')[2].ToString();
                string tid = sTKey.Split(',')[0].Split('_')[1].ToString();
                var tmpdt = bstcapp.GetList().Where(i => i.Tag_key.Equals(tkey) && i.GYStationId.Equals(tid)).Max(item => item.Save_date);
                //var tmpdt = bstcapp.GetList().Where(i => i.Tag_key.Equals(tkey)).Max(item => item.Save_date);
                if (tmpdt != null)
                {
                    dt = tmpdt.Value;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                }
                if (sDate == null)
                {
                    sDate = dt.AddHours(-2);
                }
                if (eDate == null)
                {
                    eDate = dt;
                }
            }
            string where = "";//水表数据过滤条件
            if (sDate != null || eDate != null)
            {
                if (sDate == null && eDate != null)
                {
                    where += " AND Save_date<'" + eDate.Value.AddDays(1) + "'";
                }
                else if (sDate != null && eDate == null)
                {
                    where += " AND Save_date>='" + sDate + "'";
                }
                else
                {
                    where += " AND Save_date>='" + sDate + "' AND Save_date<'" + eDate.Value.AddDays(1) + "'";
                }
            }
            if (!string.IsNullOrWhiteSpace(sTKey))
            {
                var skeys = sTKey.Split(',');
                for (var i = 0; i < skeys.Length; i++)
                {
                    var sskey = skeys[i].Split('_');
                    if (i == 0)
                    {
                        if (skeys.Length == 1)
                            where += " AND (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        else
                            where += " AND ((a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                    }
                    else if (i == skeys.Length - 1)
                    {
                        where += " OR (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + "))";
                    }
                    else
                    {
                        where += " OR (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                    }
                }
            }
            if (min != null || max != null)
            {
                if (min == null && max != null)
                {
                    where += " AND Tag_value<='" + max + "'";
                }
                else if (min != null && max == null)
                {
                    where += " AND Tag_value>'" + min + "'";
                }
                else
                {
                    where += " AND Tag_value>'" + min + "' AND Tag_value<='" + max + "'";
                }
            }
            if (sTime != null || eTime != null)
            {
                if (sTime == null && eTime != null)
                {
                    where += " AND CONVERT(TIME,Save_date)<='" + eTime + "'";
                }
                else if (sTime != null && eTime == null)
                {
                    where += " AND CONVERT(TIME,Save_date)>='" + sTime + "'";
                }
                else
                {
                    where += " AND CONVERT(TIME,Save_date)>='" + sTime + "' AND CONVERT(TIME,Save_date)<='" + eTime + "'";
                }
            }
            string sql = "SELECT MIN(Tag_value) FROM BS_SCADA_TAG_HIS a,(SELECT Meter_Name,Explain,Station_Key,DistrictAreaId,meter_type FROM BSM_Meter_Info) b " +
            "WHERE b.Station_Key=a.Station_Key AND a.Tag_key=b.meter_type AND a.Number=b.DistrictAreaId" + where;
            var tempData = dbcontext.Database.SqlQuery<decimal>(sql).FirstOrDefault().ToString("0.###");
            return tempData;
        }
        public string GetANDDownload(int type, string sTKey, decimal? max, decimal? min, DateTime? sDate, DateTime? eDate, TimeSpan? sTime, TimeSpan? eTime)
        {
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            if (!string.IsNullOrWhiteSpace(sTKey))
            {
                string tkey = sTKey.Split(',')[0].Split('_')[2].ToString();
                string tid = sTKey.Split(',')[0].Split('_')[1].ToString();
                var tmpdt = bstcapp.GetList().Where(i => i.Tag_key.Equals(tkey) && i.GYStationId.Equals(tid)).Max(item => item.Save_date);
                if (tmpdt != null)
                {
                    dt = tmpdt.Value;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                }
                if (sDate == null)
                {
                    sDate = dt.AddHours(-2);
                }
                if (eDate == null)
                {
                    eDate = dt;
                }
            }
            else return "";
            string where = "";//水表数据过滤条件
            if (sDate != null || eDate != null)
            {
                if (sDate == null && eDate != null)
                {
                    where += " AND Save_date<'" + eDate.Value.AddDays(1) + "'";
                }
                else if (sDate != null && eDate == null)
                {
                    where += " AND Save_date>='" + sDate + "'";
                }
                else
                {
                    where += " AND Save_date>='" + sDate + "' AND Save_date<'" + eDate.Value.AddDays(1) + "'";
                }
            }
            if (!string.IsNullOrWhiteSpace(sTKey))
            {
                var skeys = sTKey.Split(',');
                for (var i = 0; i < skeys.Length; i++)
                {
                    var sskey = skeys[i].Split('_');
                    if (i == 0)
                    {
                        if (skeys.Length == 1)
                            where += " AND (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                        else
                            where += " AND ((a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                    }
                    else if (i == skeys.Length - 1)
                    {
                        where += " OR (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + "))";
                    }
                    else
                    {
                        where += " OR (a.Station_Key ='" + sskey[1] + "' AND Tag_key=" + sskey[2] + " AND Number=" + sskey[3] + ")";
                    }
                }
            }
            if (min != null || max != null)
            {
                if (min == null && max != null)
                {
                    where += " AND Tag_value<='" + max + "'";
                }
                else if (min != null && max == null)
                {
                    where += " AND Tag_value>'" + min + "'";
                }
                else
                {
                    where += " AND Tag_value>'" + min + "' AND Tag_value<='" + max + "'";
                }
            }
            if (sTime != null || eTime != null)
            {
                if (sTime == null && eTime != null)
                {
                    where += " AND CONVERT(TIME,Save_date)<='" + eTime + "'";
                }
                else if (sTime != null && eTime == null)
                {
                    where += " AND CONVERT(TIME,Save_date)>='" + sTime + "'";
                }
                else
                {
                    where += " AND CONVERT(TIME,Save_date)>='" + sTime + "' AND CONVERT(TIME,Save_date)<='" + eTime + "'";
                }
            }
            string sql = "";
            //if(type==1 || type==8||type==9)
            //    sql = "SELECT Tag_value_ss 瞬时流量,Tag_value_zx 正向累计,Tag_value_fx 反向累计,Tag_value 瞬时流量,Save_date 数据时间,(SELECT TOP 1 Meter_Name 监测点,Explain 数据类型 " +
            //    "FROM BSM_Meter_Info WHERE Station_Key=a.Station_Key) FROM BS_SCADA_TAG_HIS a " + where;
            //else
            sql = "SELECT Tag_value 数据值,Save_date 数据时间,b.Meter_Name 监测点,b.Explain 数据类型 FROM BS_SCADA_TAG_HIS a,(SELECT Meter_Name,Explain,Station_Key,DistrictAreaId,meter_type " +
            "FROM BSM_Meter_Info) b WHERE b.Station_Key=a.Station_Key AND a.Tag_key=b.meter_type AND a.Number=b.DistrictAreaId" + where+ " ORDER BY Save_date";
            //string sql = "SELECT Save_date 日期时间,(SELECT Explain FROM BSM_Meter_Info WHERE Station_Key=Station_Key) 数据类型,(SELECT Meter_Name FROM BSM_Meter_Info " +
            //    "WHERE Station_Key=Station_Key) 监测点,Tag_value 原始数据,CleanedValue 清洗数据 FROM BS_SCADA_TAG_HIS " + where;
            DbCommand cmd = dbcontext.Database.Connection.CreateCommand();
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            //转成sqlcommand正常查询语句。
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd as SqlCommand;
            DataTable tmp1 = new DataTable();
            da.Fill(tmp1);
            var excel = new ExcelHelper
            {
                FilePath = "/file/数据查询" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx",
                SheetNames = new string[] { "数据查询" },
                Titles = new string[] { "数据查询" }
            };
            excel.AddTables(tmp1);
            excel.CreateExcel();
            return excel.FilePath;
        }


        /// <summary>
        /// 根据id删除SCADA数据记录
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(int id)
        {
            service.Delete(t => t.ID == id);
        }

        /// <summary>
        /// 添加或更新用户
        /// </summary>
        /// <param name="moduleEntity">数据实体信息</param>
        /// <param name="userID">用户编号</param>
        public void SubmitForm(BS_SCADA_TAG_HISEntity moduleEntity, string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                service.Update(moduleEntity);
            }
            else
            {
                service.Insert(moduleEntity);
            }
        }

        public string GetSingleFeeRecord(string id)
        {
            decimal tmp = 0;
            try
            {
                tmp = Convert.ToDecimal(id);
            }
            catch { }
            var result = service.FindEntity(t => t.ID == tmp);
            return JsonConvert.SerializeObject(result);
        }
    }
}
