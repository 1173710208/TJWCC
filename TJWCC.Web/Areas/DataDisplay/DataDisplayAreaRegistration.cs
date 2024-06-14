using System.Web.Mvc;

namespace TJWCC.Web.Areas.DataDisplay
{
    public class DataDisplayAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DataDisplay";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DataDisplay_default",
                "DataDisplay/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}