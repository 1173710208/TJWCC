using System.Web.Mvc;

namespace TJWCC.Web.Areas.DataSynthetical
{
    public class DataSyntheticalAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DataSynthetical";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DataSynthetical_default",
                "DataSynthetical/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}