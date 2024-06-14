using DGSWDC.Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.SystemManage;
using TJWCC.Application.WCC;
using TJWCC.Code;
using TJWCC.Data;
using TJWCC.Domain.ViewModel;

namespace TJWCC.Web.Areas.DataSynthetical.Controllers
{
    public class ReportController : ControllerBase
    {
        private SYS_DICApp sysDicApp = new SYS_DICApp();
        private CC_FlowSortApp cfsApp = new CC_FlowSortApp();
        // 报表管理: DataSynthetical/Report


        public ActionResult GetReportList()
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            var sds = sysDicApp.GetItemList(11);
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

        
        public ActionResult GetReportTypeList()
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            var sds = sysDicApp.GetItemList(12);
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

        public ActionResult GetCreateReport(int sr, int type, int cycle, DateTime? sDate, DateTime? eDate)
        {
            string result = "";
            string name = DateTime.Now.Year.ToString();
            if (sDate == null)
            {
                if (eDate == null) eDate = cfsApp.GetMaxDate();
                if (cycle == 1) sDate = eDate.Value.AddDays(-30);
                else sDate = eDate.Value.AddMonths(-10);
            }

            switch (cycle)
            {
                case 1://按日统计
                    switch (type)
                    {
                        case 1://水量统计
                            {
                                string where = " AND Save_date>='" + sDate + "' AND Save_date<='" + eDate + "'";
                                string sql = "SELECT CONVERT(VARCHAR(10),Save_date,23) '日期',JY_flow '芥园',LZ_flow '凌庄',XKH_flow '新开河',JB_flow '津滨',SQ_flow '市区',XQ_flow '新区',XH_flow '新河',XC_flow '新村'," +
                                    "YT_flow '大港油田',JG_flow '津港',BH_flow '滨海',Tflow '合计',JY_press '芥园压力',LZ_press '凌庄压力',XKH_press '新开河压力',JB_press '津滨压力',XQ_press " +
                                    "'新区压力',XH_press '新河压力',XC_press '新村压力',YT_press '滨海压力',JG_press '津港压力',JBLS_flow '津滨来水',DG_flow '大港供水',TT_flow '小计' " +
                                    "FROM [dbo].[CC_ReportList] WHERE Cycle=1" + where + " ORDER BY Save_date";
                                TJWCCDbContext dbcontext = new TJWCCDbContext();
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
                                    FilePath = "/file/日水量统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx",
                                    SheetNames = new string[] { "统计" },
                                    Titles = new string[] { sDate.ToDateString() + "~" + eDate.ToDateString() + "统计" }
                                };
                                excel.AddTables(tmp1);
                                excel.CreateStatistics();
                                result = excel.FilePath;
                                break;
                            }
                        case 2://水量分析
                            {
                                string where = " AND Save_date>='" + sDate + "' AND Save_date<='" + eDate + "'";
                                string where1 = " AND Save_date>='" + sDate.Value.AddYears(-1) + "' AND Save_date<='" + eDate.Value.AddYears(-1) + "'";
                                string sql = "SELECT a.Save_date '日期',b.JY_flow '芥园同比',a.JY_flow '芥园',CONVERT(DECIMAL(18,2),(a.JY_flow-b.JY_flow)/NULLIF(a.JY_flow,0)*100.0) '芥园变化'," +
                                    "b.LZ_flow '凌庄同比',a.LZ_flow '凌庄',CONVERT(DECIMAL(18,2),(a.LZ_flow-b.LZ_flow)/NULLIF(a.LZ_flow,0)*100.0) '凌庄变化',b.XKH_flow '新开河同比',a.XKH_flow '新开河'," +
                                    "CONVERT(DECIMAL(18,2),(a.XKH_flow-b.XKH_flow)/NULLIF(a.XKH_flow,0)*100.0) '新开河变化',b.JB_flow '津滨同比',a.JB_flow '津滨',CONVERT(DECIMAL(18,2),(a.JB_flow-b.JB_flow)" +
                                    "/NULLIF(a.JB_flow,0)*100.0) '津滨变化',b.XQ_flow '新区同比',a.XQ_flow '新区',CONVERT(DECIMAL(18,2),(a.XQ_flow-b.XQ_flow)/NULLIF(a.XQ_flow,0)*100.0) '新区变化',b.XH_flow '新河同比'," +
                                    "a.XH_flow '新河',CONVERT(DECIMAL(18,2),(a.XH_flow-b.XH_flow)/NULLIF(a.XH_flow,0)*100.0) '新河变化',b.XC_flow '新村同比',a.XC_flow '新村',CONVERT(DECIMAL(18,2)," +
                                    "(a.XC_flow-b.XC_flow)/NULLIF(a.XC_flow,0)*100.0) '新村变化',b.YT_flow '大港油田同比',a.YT_flow '大港油田',CONVERT(DECIMAL(18,2),(a.YT_flow-b.YT_flow)/NULLIF(a.YT_flow,0)*100.0) " +
                                    "'大港油田变化',b.JG_flow '津港同比',a.JG_flow '津港',CONVERT(DECIMAL(18,2),(a.JG_flow-b.JG_flow)/NULLIF(a.JG_flow,0)*100.0) '津港变化',b.SQ_flow '市区同比',a.SQ_flow '市区'," +
                                    "CONVERT(DECIMAL(18,2),(a.SQ_flow-b.SQ_flow)/NULLIF(a.SQ_flow,0)*100.0) '市区变化',b.BH_flow '滨海同比',a.BH_flow '滨海',CONVERT(DECIMAL(18,2),(a.BH_flow-b.BH_flow)/NULLIF(a.BH_flow,0)*" +
                                    "100.0) '滨海变化',b.Tflow '合计同比',a.Tflow '合计',CONVERT(DECIMAL(18,2),(a.Tflow-b.Tflow)/NULLIF(a.Tflow,0)*100.0) '合计变化' FROM(SELECT CONVERT(VARCHAR(5),Save_date,10) " +
                                    "Save_date,JY_flow,LZ_flow,XKH_flow,JB_flow,XQ_flow,XH_flow,XC_flow,YT_flow,JG_flow,SQ_flow,BH_flow,Tflow FROM [dbo].[CC_ReportList] WHERE Cycle=1" + where + ") a " +
                                    "LEFT JOIN (SELECT CONVERT(VARCHAR(5),Save_date,10) Save_date,JY_flow,LZ_flow,XKH_flow,JB_flow,XQ_flow,XH_flow,XC_flow,YT_flow,JG_flow,SQ_flow,BH_flow,Tflow FROM " +
                                    "[dbo].[CC_ReportList] WHERE Cycle=1" + where1 + ") b ON a.Save_date=b.Save_date ORDER BY a.Save_date";
                                TJWCCDbContext dbcontext = new TJWCCDbContext();
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
                                    FilePath = "/file/日水量分析" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx",
                                    SheetNames = new string[] { "分析" },
                                    Titles = new string[] { sDate.ToDateString() + "~" + eDate.ToDateString() + "水量分析" }
                                };
                                excel.AddTables(tmp1);
                                excel.CreateAnalyse();
                                result = excel.FilePath;
                                break;
                            }
                    }
                    break;
                case 2://按月统计
                    switch (type)
                    {
                        case 1:
                            {
                                int month = (eDate.Value.Year - sDate.Value.Year) * 12 + eDate.Value.Month - sDate.Value.Month + 1;
                                string[] sheets = new string[month];
                                string[] Titles = new string[month];
                                var excel = new ExcelHelper
                                {
                                    FilePath = "/file/月水量统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx",
                                };
                                for (int i = 0; i < month; i++)
                                {
                                    var ssDate = Convert.ToDateTime(sDate.Value.AddMonths(i).ToString("yyyy-MM") + "-01");
                                    var eeDate = Convert.ToDateTime(ssDate.AddMonths(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
                                    sheets[i] = ssDate.ToString("yyyy-MM");
                                    Titles[i] = ssDate.ToDateString() + "~" + eeDate.ToDateString() + "统计";
                                    string where = " AND Save_date>='" + ssDate + "' AND Save_date<='" + eeDate + "'";
                                    string sql = "SELECT CONVERT(VARCHAR(10),Save_date,23) '日期',JY_flow '芥园',LZ_flow '凌庄',XKH_flow '新开河',JB_flow '津滨',SQ_flow '市区',XQ_flow '新区',XH_flow '新河',XC_flow '新村'," +
                                        "YT_flow '大港油田',JG_flow '津港',BH_flow '滨海',Tflow '合计',JY_press '芥园压力',LZ_press '凌庄压力',XKH_press '新开河压力',JB_press '津滨压力',XQ_press " +
                                        "'新区压力',XH_press '新河压力',XC_press '新村压力',YT_press '滨海压力',JG_press '津港压力',JBLS_flow '津滨来水',DG_flow '大港供水',TT_flow '小计' " +
                                        "FROM [dbo].[CC_ReportList] WHERE Cycle=1" + where + " ORDER BY Save_date";
                                    TJWCCDbContext dbcontext = new TJWCCDbContext();
                                    DbCommand cmd = dbcontext.Database.Connection.CreateCommand();
                                    cmd.CommandTimeout = int.MaxValue;
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandText = sql;
                                    //转成sqlcommand正常查询语句。
                                    SqlDataAdapter da = new SqlDataAdapter();
                                    da.SelectCommand = cmd as SqlCommand;
                                    DataTable tmp1 = new DataTable();
                                    da.Fill(tmp1);
                                    excel.AddTables(tmp1);
                                }
                                excel.Titles = Titles;
                                excel.SheetNames = sheets;
                                excel.CreateStatistics();
                                result = excel.FilePath;
                                break;
                            }
                        case 2:
                            {
                                int month = (eDate.Value.Year - sDate.Value.Year) * 12 + eDate.Value.Month - sDate.Value.Month + 1;
                                string[] sheets = new string[month];
                                string[] Titles = new string[month];
                                var excel = new ExcelHelper
                                {
                                    FilePath = "/file/月水量分析" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx",
                                };
                                for (int i = 0; i < month; i++)
                                {
                                    var ssDate = Convert.ToDateTime(sDate.Value.AddMonths(i).ToString("yyyy-MM") + "-01");
                                    var eeDate = Convert.ToDateTime(ssDate.AddMonths(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
                                    sheets[i] = ssDate.ToString("yyyy-MM");
                                    Titles[i] = ssDate.ToDateString() + "~" + eeDate.ToDateString() + "水量分析";
                                    string where = " AND Save_date>='" + ssDate + "' AND Save_date<='" + eeDate + "'";
                                    string where1 = " AND Save_date>='" + ssDate.AddYears(-1) + "' AND Save_date<='" + eeDate.AddYears(-1) + "'";
                                    string sql = "SELECT a.Save_date '日期',b.JY_flow '芥园同比',a.JY_flow '芥园',CONVERT(DECIMAL(18,2),(a.JY_flow-b.JY_flow)/NULLIF(a.JY_flow,0)*100.0) '芥园变化'," +
                                        "b.LZ_flow '凌庄同比',a.LZ_flow '凌庄',CONVERT(DECIMAL(18,2),(a.LZ_flow-b.LZ_flow)/NULLIF(a.LZ_flow,0)*100.0) '凌庄变化',b.XKH_flow '新开河同比',a.XKH_flow '新开河'," +
                                        "CONVERT(DECIMAL(18,2),(a.XKH_flow-b.XKH_flow)/NULLIF(a.XKH_flow,0)*100.0) '新开河变化',b.JB_flow '津滨同比',a.JB_flow '津滨',CONVERT(DECIMAL(18,2),(a.JB_flow-b.JB_flow)" +
                                        "/NULLIF(a.JB_flow,0)*100.0) '津滨变化',b.XQ_flow '新区同比',a.XQ_flow '新区',CONVERT(DECIMAL(18,2),(a.XQ_flow-b.XQ_flow)/NULLIF(a.XQ_flow,0)*100.0) '新区变化',b.XH_flow '新河同比'," +
                                        "a.XH_flow '新河',CONVERT(DECIMAL(18,2),(a.XH_flow-b.XH_flow)/NULLIF(a.XH_flow,0)*100.0) '新河变化',b.XC_flow '新村同比',a.XC_flow '新村',CONVERT(DECIMAL(18,2)," +
                                        "(a.XC_flow-b.XC_flow)/NULLIF(a.XC_flow,0)*100.0) '新村变化',b.YT_flow '大港油田同比',a.YT_flow '大港油田',CONVERT(DECIMAL(18,2),(a.YT_flow-b.YT_flow)/NULLIF(a.YT_flow,0)*100.0) " +
                                        "'大港油田变化',b.JG_flow '津港同比',a.JG_flow '津港',CONVERT(DECIMAL(18,2),(a.JG_flow-b.JG_flow)/NULLIF(a.JG_flow,0)*100.0) '津港变化',b.SQ_flow '市区同比',a.SQ_flow '市区'," +
                                        "CONVERT(DECIMAL(18,2),(a.SQ_flow-b.SQ_flow)/NULLIF(a.SQ_flow,0)*100.0) '市区变化',b.BH_flow '滨海同比',a.BH_flow '滨海',CONVERT(DECIMAL(18,2),(a.BH_flow-b.BH_flow)/NULLIF(a.BH_flow,0)*" +
                                        "100.0) '滨海变化',b.Tflow '合计同比',a.Tflow '合计',CONVERT(DECIMAL(18,2),(a.Tflow-b.Tflow)/NULLIF(a.Tflow,0)*100.0) '合计变化' FROM(SELECT CONVERT(VARCHAR(5),Save_date,10) " +
                                        "Save_date,JY_flow,LZ_flow,XKH_flow,JB_flow,XQ_flow,XH_flow,XC_flow,YT_flow,JG_flow,SQ_flow,BH_flow,Tflow FROM [dbo].[CC_ReportList] WHERE Cycle=1" + where + ") a " +
                                        "LEFT JOIN (SELECT CONVERT(VARCHAR(5),Save_date,10) Save_date,JY_flow,LZ_flow,XKH_flow,JB_flow,XQ_flow,XH_flow,XC_flow,YT_flow,JG_flow,SQ_flow,BH_flow,Tflow FROM " +
                                        "[dbo].[CC_ReportList] WHERE Cycle=1" + where1 + ") b ON a.Save_date=b.Save_date ORDER BY a.Save_date";
                                    TJWCCDbContext dbcontext = new TJWCCDbContext();
                                    DbCommand cmd = dbcontext.Database.Connection.CreateCommand();
                                    cmd.CommandTimeout = int.MaxValue;
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandText = sql;
                                    //转成sqlcommand正常查询语句。
                                    SqlDataAdapter da = new SqlDataAdapter();
                                    da.SelectCommand = cmd as SqlCommand;
                                    DataTable tmp1 = new DataTable();
                                    da.Fill(tmp1);
                                    excel.AddTables(tmp1);
                                }
                                excel.Titles = Titles;
                                excel.SheetNames = sheets;
                                excel.CreateStatistics();
                                result = excel.FilePath;
                            }
                            break;
                    }
                    break;
                case 3://按年统计
                    switch (type)
                    {
                        case 1:
                            {
                                int month = 12;
                                string[] sheets = new string[month];
                                string[] Titles = new string[month];
                                var excel = new ExcelHelper
                                {
                                    FilePath = "/file/年水量统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx",
                                };
                                for (int i = 0; i < month; i++)
                                {
                                    var ssDate = Convert.ToDateTime(sDate.Value.Year + "-01-01").AddMonths(i);
                                    var eeDate = Convert.ToDateTime(ssDate.AddMonths(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
                                    sheets[i] = ssDate.ToString("yyyy-MM");
                                    Titles[i] = ssDate.ToDateString() + "~" + eeDate.ToDateString() + "统计";
                                    string where = " AND Save_date>='" + ssDate + "' AND Save_date<='" + eeDate + "'";
                                    string sql = "SELECT CONVERT(VARCHAR(10),Save_date,23) '日期',JY_flow '芥园',LZ_flow '凌庄',XKH_flow '新开河',JB_flow '津滨',SQ_flow '市区',XQ_flow '新区',XH_flow '新河',XC_flow '新村'," +
                                        "YT_flow '大港油田',JG_flow '津港',BH_flow '滨海',Tflow '合计',JY_press '芥园压力',LZ_press '凌庄压力',XKH_press '新开河压力',JB_press '津滨压力',XQ_press " +
                                        "'新区压力',XH_press '新河压力',XC_press '新村压力',YT_press '滨海压力',JG_press '津港压力',JBLS_flow '津滨来水',DG_flow '大港供水',TT_flow '小计' " +
                                        "FROM [dbo].[CC_ReportList] WHERE Cycle=1" + where + " ORDER BY Save_date";
                                    TJWCCDbContext dbcontext = new TJWCCDbContext();
                                    DbCommand cmd = dbcontext.Database.Connection.CreateCommand();
                                    cmd.CommandTimeout = int.MaxValue;
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandText = sql;
                                    //转成sqlcommand正常查询语句。
                                    SqlDataAdapter da = new SqlDataAdapter();
                                    da.SelectCommand = cmd as SqlCommand;
                                    DataTable tmp1 = new DataTable();
                                    da.Fill(tmp1);
                                    excel.AddTables(tmp1);
                                }
                                excel.Titles = Titles;
                                excel.SheetNames = sheets;
                                excel.CreateStatistics();
                                result = excel.FilePath;
                                break;
                            }
                        case 2:
                            {
                                int month = 12;
                                string[] sheets = new string[month];
                                string[] Titles = new string[month];
                                var excel = new ExcelHelper
                                {
                                    FilePath = "/file/年水量分析" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx",
                                };
                                for (int i = 0; i < month; i++)
                                {
                                    var ssDate = Convert.ToDateTime(sDate.Value.Year + "-01-01").AddMonths(i);
                                    var eeDate = Convert.ToDateTime(ssDate.AddMonths(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
                                    sheets[i] = ssDate.ToString("yyyy-MM");
                                    Titles[i] = ssDate.ToDateString() + "~" + eeDate.ToDateString() + "水量分析";
                                    string where = " AND Save_date>='" + ssDate + "' AND Save_date<='" + eeDate + "'";
                                    string where1 = " AND Save_date>='" + ssDate.AddYears(-1) + "' AND Save_date<='" + eeDate.AddYears(-1) + "'";
                                    string sql = "SELECT a.Save_date '日期',b.JY_flow '芥园同比',a.JY_flow '芥园',CONVERT(DECIMAL(18,2),(a.JY_flow-b.JY_flow)/NULLIF(a.JY_flow,0)*100.0) '芥园变化'," +
                                        "b.LZ_flow '凌庄同比',a.LZ_flow '凌庄',CONVERT(DECIMAL(18,2),(a.LZ_flow-b.LZ_flow)/NULLIF(a.LZ_flow,0)*100.0) '凌庄变化',b.XKH_flow '新开河同比',a.XKH_flow '新开河'," +
                                        "CONVERT(DECIMAL(18,2),(a.XKH_flow-b.XKH_flow)/NULLIF(a.XKH_flow,0)*100.0) '新开河变化',b.JB_flow '津滨同比',a.JB_flow '津滨',CONVERT(DECIMAL(18,2),(a.JB_flow-b.JB_flow)" +
                                        "/NULLIF(a.JB_flow,0)*100.0) '津滨变化',b.XQ_flow '新区同比',a.XQ_flow '新区',CONVERT(DECIMAL(18,2),(a.XQ_flow-b.XQ_flow)/NULLIF(a.XQ_flow,0)*100.0) '新区变化',b.XH_flow '新河同比'," +
                                        "a.XH_flow '新河',CONVERT(DECIMAL(18,2),(a.XH_flow-b.XH_flow)/NULLIF(a.XH_flow,0)*100.0) '新河变化',b.XC_flow '新村同比',a.XC_flow '新村',CONVERT(DECIMAL(18,2)," +
                                        "(a.XC_flow-b.XC_flow)/NULLIF(a.XC_flow,0)*100.0) '新村变化',b.YT_flow '大港油田同比',a.YT_flow '大港油田',CONVERT(DECIMAL(18,2),(a.YT_flow-b.YT_flow)/NULLIF(a.YT_flow,0)*100.0) " +
                                        "'大港油田变化',b.JG_flow '津港同比',a.JG_flow '津港',CONVERT(DECIMAL(18,2),(a.JG_flow-b.JG_flow)/NULLIF(a.JG_flow,0)*100.0) '津港变化',b.SQ_flow '市区同比',a.SQ_flow '市区'," +
                                        "CONVERT(DECIMAL(18,2),(a.SQ_flow-b.SQ_flow)/NULLIF(a.SQ_flow,0)*100.0) '市区变化',b.BH_flow '滨海同比',a.BH_flow '滨海',CONVERT(DECIMAL(18,2),(a.BH_flow-b.BH_flow)/NULLIF(a.BH_flow,0)*" +
                                        "100.0) '滨海变化',b.Tflow '合计同比',a.Tflow '合计',CONVERT(DECIMAL(18,2),(a.Tflow-b.Tflow)/NULLIF(a.Tflow,0)*100.0) '合计变化' FROM(SELECT CONVERT(VARCHAR(5),Save_date,10) " +
                                        "Save_date,JY_flow,LZ_flow,XKH_flow,JB_flow,XQ_flow,XH_flow,XC_flow,YT_flow,JG_flow,SQ_flow,BH_flow,Tflow FROM [dbo].[CC_ReportList] WHERE Cycle=1" + where + ") a " +
                                        "LEFT JOIN (SELECT CONVERT(VARCHAR(5),Save_date,10) Save_date,JY_flow,LZ_flow,XKH_flow,JB_flow,XQ_flow,XH_flow,XC_flow,YT_flow,JG_flow,SQ_flow,BH_flow,Tflow FROM " +
                                        "[dbo].[CC_ReportList] WHERE Cycle=1" + where1 + ") b ON a.Save_date=b.Save_date ORDER BY a.Save_date";
                                    TJWCCDbContext dbcontext = new TJWCCDbContext();
                                    DbCommand cmd = dbcontext.Database.Connection.CreateCommand();
                                    cmd.CommandTimeout = int.MaxValue;
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandText = sql;
                                    //转成sqlcommand正常查询语句。
                                    SqlDataAdapter da = new SqlDataAdapter();
                                    da.SelectCommand = cmd as SqlCommand;
                                    DataTable tmp1 = new DataTable();
                                    da.Fill(tmp1);
                                    excel.AddTables(tmp1);
                                }
                                excel.Titles = Titles;
                                excel.SheetNames = sheets;
                                excel.CreateStatistics();
                                result = excel.FilePath;
                            }
                            break;
                    }
                    break;
            }
            return Content(result);
        }
    }
}