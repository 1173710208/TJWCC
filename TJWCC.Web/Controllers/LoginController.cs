using TJWCC.Domain.Entity.SystemSecurity;
using TJWCC.Application.SystemSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Domain.Entity.SystemManage;
using TJWCC.Application.SystemManage;
using TJWCC.Code;
using TJWCC.Application;
using TJWCC.Application.BSSys;

namespace TJWCC.Web.Controllers
{
    public class LoginController : Controller
    {
        private BSOT_SysInfoApp app = new BSOT_SysInfoApp();
        
        public virtual ActionResult Index()
        {
            var test = string.Format("{0:E2}", 1);
            return View();
        } 
        
        public ActionResult OutLogin()
        {
            new LogApp().WriteDbLog(new LogEntity
            {
                MODULENAME = "系统登录",
                TYPE = DbLogType.Exit.ToString(),
                ACCOUNT = OperatorProvider.Provider.GetCurrent().UserCode,
                NICKNAME = OperatorProvider.Provider.GetCurrent().UserName,
                RESULT = true,
                DESCRIPTION = "安全退出系统",
            });
            Session.Abandon();
            Session.Clear();
            OperatorProvider.Provider.RemoveCurrent();
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult CheckLogin(string username, string password, string code)
        {
            LogEntity logEntity = new LogEntity();
            logEntity.MODULENAME = "系统登录";
            logEntity.TYPE = DbLogType.Login.ToString();
            try
            {
                UserEntity userEntity = new UserApp().CheckLogin(username, password);
                if (userEntity != null)
                {
                    DateTime nowdate = DateTime.Now;
                    DateTime sdate = new DateTime(nowdate.Year, nowdate.Month, nowdate.Day, 7, 0, 0);
                    DateTime edate = new DateTime(nowdate.Year, nowdate.Month, nowdate.Day, 19, 0, 0);
                    OperatorModel operatorModel = new OperatorModel();
                    operatorModel.UserId = userEntity.ID;
                    operatorModel.UserCode = userEntity.ACCOUNT;
                    operatorModel.UserName = userEntity.REALNAME;
                    operatorModel.CompanyId = userEntity.ORGANIZEID;
                    operatorModel.DepartmentId = userEntity.DEPARTMENTID;
                    operatorModel.RoleId = userEntity.ROLEID;
                    operatorModel.DutyId = (nowdate > sdate && nowdate < edate) ? "1" : "2";
                    operatorModel.LoginIPAddress = Net.Ip;
                    operatorModel.LoginIPAddressName = Net.GetLocation(operatorModel.LoginIPAddress);
                    operatorModel.LoginTime = Convert.ToDateTime(DateTime.Now.ToDateTimeString());
                    operatorModel.LoginToken = DESEncrypt.Encrypt(Guid.NewGuid().ToString());
                    //if (userEntity.ACCOUNT == "admin" || userEntity.ACCOUNT == "user1" || userEntity.ACCOUNT == "user2")
                    //{
                        operatorModel.IsSystem = true;
                    //}
                    //else
                    //{
                    //    operatorModel.IsSystem = false;
                    //}
                    //公司名称
                    operatorModel.CompanyName = app.GetList()[0].CompanyName;
                    //ArcGIS API信息
                    operatorModel.ArcGISAPI = app.GetList()[0].ArcGISAPI;
                    //ArcGIS Server信息
                    operatorModel.ArcGISServer = app.GetList()[0].ArcGISServer;
                    OperatorProvider.Provider.AddCurrent(operatorModel);
                    logEntity.ACCOUNT = userEntity.ACCOUNT;
                    logEntity.NICKNAME = userEntity.REALNAME;
                    logEntity.RESULT = true;
                    logEntity.DESCRIPTION = "登录成功";
                    new LogApp().WriteDbLog(logEntity);
                }
                return Content(new AjaxResult { state = ResultType.success.ToString(), message = "登录成功。" }.ToJson());
            }
            catch (Exception ex)
            {
                logEntity.ACCOUNT = username;
                logEntity.NICKNAME = username;
                logEntity.RESULT = false;
                logEntity.DESCRIPTION = "登录失败，" + ex.Message;
                new LogApp().WriteDbLog(logEntity);
                return Content(new AjaxResult { state = ResultType.error.ToString(), message = ex.Message }.ToJson());
            }
        }
    }
}
