using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.SystemManage;
using TJWCC.Application.WCC;
using TJWCC.Code;
using TJWCC.Data;
using TJWCC.Domain.Entity.WCC;
using TJWCC.Domain.ViewModel;

namespace TJWCC.Web.Areas.Scheduling.Controllers
{
    public class LogController : ControllerBase
    {
        log4net.ILog loger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private SYS_DICApp sysDicApp = new SYS_DICApp();
        private DutyApp dutyApp = new DutyApp();
        private CC_DispatchPlanApp dpApp = new CC_DispatchPlanApp();
        private UserApp uApp = new UserApp();
        // 调度日志: Scheduling/Log

        /// <summary>
        /// 获取调度日志中班次下拉菜单列表
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetShiftList()
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            var sds = dutyApp.GetList();
            foreach (var sd in sds)
            {
                GetDataType_Result rdr = new GetDataType_Result()
                {
                    Name = sd.FULLNAME,
                    Value = sd.ENCODE
                };
                list.Add(rdr);
            }
            return Content(list.ToJson());
        }

        /// <summary>
        /// 获取调度日志中调度类型下拉菜单列表
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetDispatchTypeList()
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            var sds = sysDicApp.GetItemList(6);
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
        /// 获取调度日志中操作员下拉菜单列表
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetOperatorList()
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            var sds = uApp.GetList();
            foreach (var sd in sds)
            {
                GetDataType_Result rdr = new GetDataType_Result()
                {
                    Name = sd.REALNAME,
                    Value = sd.REALNAME
                };
                list.Add(rdr);
            }
            return Content(list.ToJson());
        }
        /// <summary>
        /// 获取调度日志中调令状态下拉菜单列表
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetStatusList()
        {
            List<GetDataType_Result> list = new List<GetDataType_Result>();
            var sds = sysDicApp.GetItemList(7);
            foreach (var sd in sds)
            {
                GetDataType_Result rdr = new GetDataType_Result()
                {
                    Name = sd.ItemName,
                    Value = sd.ItemName
                };
                list.Add(rdr);
            }
            return Content(list.ToJson());
        }

        
        public ActionResult AddPlanBase(int planId)
        {
            TJWCCDbContext dbcontext = new TJWCCDbContext();
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
            cmd.CommandText = "UPDATE [dbo].[CC_DispatchPlan] SET IsPlanBase=1 WHERE ID="+ planId;
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
            return Success("添加成功。");
        }
        /// <summary>
        /// 根据条件获取调度日志数据
        /// </summary>
        ///<param name="typeId">水厂类型</param> 
        /// <returns></returns>
        public ActionResult GetDispatchPlan(Pagination pagination, DateTime? sdDate, DateTime? edDate, DateTime? seDate, DateTime? eeDate, int? shift, string oper, decimal? devi, string keyword)
        {
            var data = new
            {
                rows = dpApp.GetList(pagination, sdDate, edDate, seDate, eeDate, shift, oper, devi, keyword),
                pagination.total,
                pagination.page,
                pagination.records
            };
            return Content(data.ToJson());
        }
        public ActionResult GetDownloadJson(DateTime? sdDate, DateTime? edDate, DateTime? seDate, DateTime? eeDate, int? shift, string oper, decimal? devi, int? status, int? isPlan, string keyword)
        {
            string path = dpApp.GetDownload(sdDate, edDate, seDate, eeDate, shift, oper, devi, status, isPlan, keyword);
            //StringBuilder sbScript = new StringBuilder();
            //sbScript.Append("<script type='text/javascript'>$.loading(false);</script>");
            return Content("http://" + Request.Url.Host + ":" + Request.Url.Port + path);
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(CC_DispatchPlanEntity areaEntity, int? keyValue)
        {
            areaEntity.DispatchType = 2;
            areaEntity.Status = "1";
            areaEntity.Operator = OperatorProvider.Provider.GetCurrent().UserName;
            areaEntity.IsPlanBase = 0;
            areaEntity.Shift = Convert.ToInt32(OperatorProvider.Provider.GetCurrent().DutyId);
            dpApp.SubmitForm(areaEntity, keyValue);
            return Success("操作成功。");
        }
    }
}