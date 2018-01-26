using System.Web.Mvc;

namespace Jobs.Areas.PropertyTax
{
    public class PropertyTaxAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PropertyTax";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PropertyTax_default",
                "PropertyTax/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Jobs.Areas.PropertyTax.Controllers" }
            );
        }
    }
}