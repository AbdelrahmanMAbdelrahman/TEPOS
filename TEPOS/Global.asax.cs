using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ERP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //protected void application_beginrequest(object sender, eventargs e)
        //{

        //    if (application.count == 0)
        //    {
        //        //////01-12-2021 enas ensure that request is comming from alnoursmart
        //        /////2024-06-20 enas add protection with url for 19011 (not work )
        //        if (!httpcontext.current.request.url.tostring().startswith("http://."))
        //        {
        //            httpcontext.current.response.redirect("http://./trend/");
        //        }
        //        //splendidinit.initapp(this.context);
        //        //workflowinit.startruntime(this.application);
        //        //initschedulermanager();
        //        //initemailmanager();
        //        //// 08/28/2013 paul.  add support for twilio and signalr. 
        //        //twiliomanager.initapp(this.context);
        //        //// 11/10/2014 paul.  add chatchannels support. 
        //        //chatmanager.initapp(this.context);
        //        //signalrutils.initapp();
        //    }
        //}
    }
}
