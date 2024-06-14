using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TJWCC.Application.Test;
using TJWCC.Code;

namespace TJWCC.Web.Areas.Test.Controllers
{
    public class TestPageController : ControllerBase
    {

        private TestApp testApp = new TestApp(); 

        
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination)
        {
            var data = new
            {
                rows = testApp.GetList(pagination),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }


    }
}
