using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Models;
using ERP.Models.Security.Authorization;

namespace ERP.Controllers
{
    [HasAuthorization]
    public class SoftwareUpdateController : BaseController
    {
        // GET: SoftwareUpdate
        public ActionResult ExecuteSqlCommand()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [AllowAnonymous]
        public ActionResult ExecuteSqlCommandLine(string commandLine)
        {
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    int rtr = database.Database.ExecuteSqlCommand(commandLine);
                    if (rtr !=0)
                    {
                        TempData["Message"] = "Success";
                    }
                    else
                    {
                        TempData["Message"] = "Failed";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("ExecuteSqlCommand");
        }
    }
}