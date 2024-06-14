using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.DataDisplay;

namespace TJWCC.Web.Areas.DataDisplay.Controllers
{
    public class TestController : Controller
    {
        Dis_AlarmApp app = new Dis_AlarmApp();
        // GET: DataDisplay/Test
        public ActionResult Index()
        {
            

            //ViewData["entity"] = app.GetMeterCount();
            return View();
        }
    }
}