using TJWCC.Code;
using System.Web.Mvc;

namespace TJWCC.Web
{
    public class HandlerErrorAttribute : HandleErrorAttribute
    {
        log4net.ILog loger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            context.ExceptionHandled = true;
            context.HttpContext.Response.StatusCode = 200;
            context.Result = new ContentResult { Content = new AjaxResult { state = ResultType.error.ToString(), message = context.Exception.Message }.ToJson() };
            loger.Error(context.Exception);
        }
        private void WriteLog(ExceptionContext context)
        {
            if (context == null)
                return;
            var log = LogFactory.GetLogger(context.Controller.ToString());
            log.Error(context.Exception);
        }
    }
}