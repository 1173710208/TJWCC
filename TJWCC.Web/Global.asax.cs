using TJWCC.Code;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;

namespace TJWCC.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// 启动应用程序
        /// </summary>
        protected void Application_Start()
        {
            //配置log4
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(Server.MapPath("~/Configs/log4net.config")));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Database.SetInitializer<TJWCC.Data.TJWCCDbContext>(null);
        }
    }
}