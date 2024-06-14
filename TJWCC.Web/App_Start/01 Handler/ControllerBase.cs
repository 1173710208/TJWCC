using TJWCC.Code;
using System.Web.Mvc;

namespace TJWCC.Web
{
    [HandlerLogin]
    public abstract class ControllerBase : Controller
    {
        public Log FileLog
        {
            get { return LogFactory.GetLogger(this.GetType().ToString()); }
        }
        
        [HandlerAuthorize]
        public virtual ActionResult Index()
        {
            return View();
        }
        
        [HandlerAuthorize]
        public virtual ActionResult Form()
        {
            return View();
        }
        
        [HandlerAuthorize]
        public virtual ActionResult Details()
        {
            return View();
        }
        protected virtual ActionResult Success(string message)
         {
            return Content(new AjaxResult { state = ResultType.success.ToString(), message = message }.ToJson());
        }
        protected virtual ActionResult Success(string message, object data)
        {
            return Content(new AjaxResult { state = ResultType.success.ToString(), message = message, data = data }.ToJson());
        }
        protected virtual ActionResult Error(string message)
        {
            return Content(new AjaxResult { state = ResultType.error.ToString(), message = message }.ToJson());
        }
        protected virtual ActionResult Info(string message)
        {
            return Content(new AjaxResult { state = ResultType.info.ToString(), message = message }.ToJson());
        }
    }
}
