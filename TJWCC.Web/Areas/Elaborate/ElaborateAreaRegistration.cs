using System.Web.Mvc;

namespace TJWCC.Web.Areas.Elaborate
{
    public class ElaborateAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Elaborate";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Elaborate_default",
                "Elaborate/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}