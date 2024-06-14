using TJWCC.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TJWCC.Web.Areas.ExampleManage.Controllers
{
    public class SendMailController : ControllerBase
    {
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SendMail(string account, string title, string content)
        {
            MailHelper mail = new MailHelper();
            mail.MailServer = Configs.GetValue("MailHost");
            mail.MailUserName = Configs.GetValue("MailUserName");
            mail.MailPassword = Configs.GetValue("MailPassword");
            mail.MailName = "集抄平台";
            mail.Send(account, title, content);
            return Success("发送成功。");
        }
    }
}
