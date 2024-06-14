using TJWCC.Application.SystemManage;
using TJWCC.Code;
using TJWCC.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace TJWCC.Web.Controllers
{
    [HandlerLogin]
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {

            return View();
        }
        
        public ActionResult Default()
        {
            return View();
        } 
    }
}
