using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.BSSys;
using TJWCC.Code;
using TJWCC.Data;
using TJWCC.Domain.Entity.BSSys;

namespace TJWCC.Web.Areas.SystemManage.Controllers
{
    public class AlarmController : ControllerBase
    {
        log4net.ILog loger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BSM_Meter_InfoApp dmiApp = new BSM_Meter_InfoApp();
        private BSB_Warn_RecordsApp bwrApp = new BSB_Warn_RecordsApp();

        // 报警管理: SystemManage/Alarm
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string keyword)
        {
            var data = new
            {
                rows = dmiApp.GetList(pagination, keyword),
                pagination.total,
                pagination.page,
                pagination.records
            };
            return Content(data.ToJson());
        }
        [HandlerAjaxOnly]
        public ActionResult AllRead()
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
            cmd.CommandText = "UPDATE [dbo].[BSB_Warn_Records] SET IsRead=1 WHERE IsRead=0";
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
            return Success("批量修改成功！");
        }
        [HandlerAjaxOnly]
        public ActionResult WarnCount()
        {
            int count = 0;
            TJWCCDbContext db = new TJWCCDbContext();
            try
            {
                count = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM [dbo].[BSB_Warn_Records] WHERE IsRead=0").FirstOrDefault();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            return Content(count.ToJson());
        }
        [HandlerAjaxOnly]
        public ActionResult WarnNewInfo()
        {
            JArray result = new JArray();
            TJWCCDbContext db = new TJWCCDbContext();
            try
            {
                var NewInfo = db.Database.SqlQuery<string>("SELECT Remark FROM [dbo].[BSB_Warn_Records] WHERE id in(SELECT MAX(id) FROM [dbo].[BSB_Warn_Records] WHERE IsRead=0 " +
                    "AND (abs(datediff(mi , GETDATE() , StartTime)))<=2 AND IsActive=1)").FirstOrDefault()?.Split(';');
                if (NewInfo != null)
                    result = JArray.FromObject(NewInfo);
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            return Content(result.ToJson());
        }
        [HandlerAjaxOnly]
        public ActionResult AllChange(string keyword, int type, decimal value)
        {
            TJWCCDbContext db = new TJWCCDbContext();
            string where = "";
            try
            {
                db.Database.Connection.Open();
            }
            catch (Exception ex)
            {
                loger.Error(ex.Message);
                loger.Error(ex.Source + " |" + ex.StackTrace);
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var skeys = keyword.Split(',');
                for (var i = 0; i < skeys.Length; i++)
                {
                    if (skeys[i].Length > 0)
                    {
                        var sskey = skeys[i].Split('_');
                        if (i == 0)
                        {
                            if (skeys.Length == 1)
                                where += " AND (Station_Key ='" + sskey[1] + "' AND Meter_Type=" + sskey[2] + " AND DistrictAreaId=" + sskey[3] + ")";
                            else
                                where += " AND ((Station_Key ='" + sskey[1] + "' AND Meter_Type=" + sskey[2] + " AND DistrictAreaId=" + sskey[3] + ")";
                        }
                        else if (i == skeys.Length - 1)
                        {
                            where += " OR (Station_Key ='" + sskey[1] + "' AND Meter_Type=" + sskey[2] + " AND DistrictAreaId=" + sskey[3] + "))";
                        }
                        else
                        {
                            where += " OR (Station_Key ='" + sskey[1] + "' AND Meter_Type=" + sskey[2] + " AND DistrictAreaId=" + sskey[3] + ")";
                        }
                    }
                }
            }
            DbCommand cmd = db.Database.Connection.CreateCommand();
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.Text;
            switch (type)
            {
                case 1://超上限
                    cmd.CommandText = "UPDATE [dbo].[BSM_Meter_Info] SET PHUp=" + value + " WHERE 1=1" + where;
                    break;
                case 2://超下限
                    cmd.CommandText = "UPDATE [dbo].[BSM_Meter_Info] SET PHDown=" + value + " WHERE 1=1" + where;
                    break;
                case 3://上限
                    cmd.CommandText = "UPDATE [dbo].[BSM_Meter_Info] SET PressureUp=" + value + " WHERE 1=1" + where;
                    break;
                case 4://下限
                    cmd.CommandText = "UPDATE [dbo].[BSM_Meter_Info] SET PressureDown=" + value + " WHERE 1=1" + where;
                    break;
            }
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
            return Success("批量修改成功！");
        }
        
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(long keyValue)
        {
            var data = dmiApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(BSM_Meter_InfoEntity dmiEntity, string keyValue)
        {
            dmiApp.SubmitForm(dmiEntity, keyValue);
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAuthorize]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(int keyValue)
        {
            dmiApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
        [HandlerAjaxOnly]
        public ActionResult GetAlarmGridJson(Pagination pagination, string keyword,DateTime? sDate, DateTime? eDate)
        {
            var data = new
            {
                rows = bwrApp.GetList(pagination, keyword, sDate, eDate),
                pagination.total,
                pagination.page,
                pagination.records
            };
            return Content(data.ToJson());
        }
    }
}