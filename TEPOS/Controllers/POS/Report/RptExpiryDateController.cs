using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Controllers.Manager;
using ERP.Models.Security.Authorization;

namespace ERP.Controllers.POS.Report
{
    [HasAuthorization]
    public class RptExpiryDateController : BaseController
    {
        public ActionResult ExpiryDate()
        {
            return View();
        }


    }
}