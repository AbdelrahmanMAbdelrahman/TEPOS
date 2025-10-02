using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Models.Security.Authorization;

namespace ERP.Controllers.POS.Report
{
    [HasAuthorization]
    public class RptPurchaseController : BaseController
    {
        public ActionResult PurchaseReceipt()
        {
            return View();
        }
    }
}