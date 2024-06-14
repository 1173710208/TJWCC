using System.Web.Mvc;

namespace TJWCC.Web.Areas.ToView
{
    public class ToViewAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ToView";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ToView_default",
                "ToView/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}