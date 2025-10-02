using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ERP.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string culture = (string)Session["CurrentCulture"];

            if (string.IsNullOrEmpty(culture))
            {
                culture = "en-US"; // اللغة الافتراضية
                Session["CurrentCulture"] = culture;
            }

            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

            base.OnActionExecuting(filterContext);
        }

        public ActionResult ChangeCulture(string culture)
        {
            Session["CurrentCulture"] = culture;
            string returnUrl = Request.UrlReferrer?.ToString() ?? Url.Action("Index", "Home");
            return Redirect(returnUrl);
        }


    }
}