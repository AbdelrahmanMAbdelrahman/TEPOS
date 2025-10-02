using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ERP.Controllers.Manager;
using ERP.Controllers.Security.Gateway;
using ERP.Models;
using ERP.Models.Security;
using ERP.Models.Security.Authorization;
using iTextSharp.text.html;

namespace ERP.Controllers.Security
{

    [HasAuthorization]
    public partial class SecurityController : Controller
    {
        private readonly ErpManager _erpManager = new ErpManager();

        [AllowAnonymous]
        public ActionResult Login()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }

        }

        [AllowAnonymous]
        public ActionResult CompanyRegister()
        {
            try
            {

                return RedirectToAction("Login", "Security");
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }

        [AllowAnonymous]
        public ActionResult CreatingNewCompanyAccount(CmnCompany company)
        {
            try
            {
               
            }
            catch (Exception e)
            {
               
            }
            return RedirectToAction("Login", "Security");
        }



        [HttpPost]
        [ActionName("Login")]
        [AllowAnonymous]
        public ActionResult Login(SecUser user)
        {
            using (ConnectionDatabase dbContext = new ConnectionDatabase())
            {
                user.Password = ConvertToBinaryString(user.Password);// Adel

                SecUser userLst = dbContext.UserDbSet.AsNoTracking().FirstOrDefault(f => f.LoginName == user.LoginName && f.Password == user.Password && f.Status);
                 if (userLst != null)
                {
                    if (userLst.Password == user.Password && userLst.LoginName == user.LoginName)
                    {
                        SecUserRole userRole = dbContext.UserRoleDbSet.AsNoTracking().FirstOrDefault(f => f.SecUserId == userLst.Id);
                        if (userRole != null)
                        {
                            string remoteAddress = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                            this.SetSession(userLst, userRole, dbContext);
                            if (userRole.SecRoleId == 1356)
                            {
                                return RedirectToAction("BillingCallCenter", "Pos");

                            }
                            return RedirectToAction("PosDashBoard", "PosDashBoard", new { moduleId = "2" });

                        }

                        //role don't set
                        TempData["msg"] = "User role don't set yet.";
                        TempData["type"] = "danger";
                        return RedirectToAction("Login", "Security");
                    }

                    //user && pass don't match
                    TempData["msg"] = "User or Password doesn\\'t match !";
                    TempData["type"] = "danger";
                    return RedirectToAction("Login", "Security");
                }

                //user && pass don't match
                TempData["msg"] = "User or Password doesn\\'t match !";
                TempData["type"] = "danger";
                return RedirectToAction("Login", "Security");
            }

        }
        // Adel 06/07/2024
        private static string ConvertToBinaryString(string input)
        {
            // Convert the input string to a byte array using UTF-8 encoding
            byte[] bytes = Encoding.UTF8.GetBytes(input);

            // Convert the byte array to a hexadecimal string prefixed with "0x"
            StringBuilder hex = new StringBuilder(bytes.Length * 2 + 2);
            hex.Append("0x");
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }
        private int InsertLoginHistory(SecLoginHistory logHistory, ConnectionDatabase dbContext)
        {
            dbContext.LoginHistoryDbSet.Add(logHistory);
            return dbContext.SaveChanges();
        }

        private void SetSession(SecUser userLst, SecUserRole userRole, ConnectionDatabase dbContext)
        {
            Session["cmnId"] = userLst.CmnCompanyId;
            Session["userId"] = userLst.Id;
            Session["roleId"] = userRole.SecRoleId;

            var usrBranch = dbContext.UserBranchDbSet.AsNoTracking().Where(w => w.SecUserId == userLst.Id).Select(s => s.PosBranchId).ToList();
            if (usrBranch.Any())
            {
                Session["userBranch"] = usrBranch;
            }
            else
            {
                Session["userBranch"] = dbContext.PosBrancheDbSet.AsNoTracking().Where(w => w.CmnCompanyId == userLst.CmnCompanyId).Select(s => s.Id).ToList();
            }

            this.SetSessionDefaultData();
        }

        private void SetSessionDefaultData()
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                int cmnId = _erpManager.CmnId;
                int userId = _erpManager.UserId;
                var comLst = database.CompanyDbSet.Where(w => w.Id == cmnId).Select(s => new { s.Name, s.Address, s.Phone, s.Email, s.RegistrationNo, s.VatRegNo,s.SalesPriceIncOrExcVat }).FirstOrDefault();
                if (comLst != null)
                {
                    Session["CompanyName"] = comLst.Name;
                    Session["CompanyAddress"] = comLst.Address;
                    Session["CompanyPhone"] = comLst.Phone;
                    Session["CompanyEmail"] = comLst.Email;
                    Session["RegistrationNo"] = comLst.RegistrationNo;
                    Session["VatRegNo"] = comLst.VatRegNo;
                    Session.Add("SalesPriceIncOrExcVat", comLst.SalesPriceIncOrExcVat);
                }

                var branchLst = database.PosBrancheDbSet.Where(w => _erpManager.UserOffice.Contains(w.Id)).Select(s => new { s.Name, s.Address }).FirstOrDefault();
                if (branchLst != null)
                {
                    Session["BranchName"] = branchLst.Name;
                    Session["BranchAddress"] = branchLst.Address;

                }
                
                var userLst = database.UserDbSet.Where(w => w.Id == userId).Select(s => new { s.TerminalId, s.LoginName }).FirstOrDefault();
                if (userLst != null)
                {
                    Session["UserName"] = userLst.LoginName;
                    Session["TerminalId"] = userLst.TerminalId;

                }
                int branchId = _erpManager.UserOffice.First();
                var billingFooterLst = database.PosBillingReportTextDbSet.FirstOrDefault(f => f.CmnCompanyId == cmnId && f.PosBranchId == branchId);
                if (billingFooterLst != null)
                {
                    Session["BillingFooterText"] = billingFooterLst.Text;
                    Session["PoweredBy"] = billingFooterLst.PoweredBy;
                }

                List<VmResourcePermission> resourcePermissions = (from l in database.ResourcePermissionDbSet
                                                                  join ur in database.UserRoleDbSet on l.SecRoleId equals ur.SecRoleId
                                                                  where l.Status && ur.SecRoleId == _erpManager.RoleId && ur.SecUserId == _erpManager.UserId
                                                                  select new VmResourcePermission
                                                                  {
                                                                      Id = l.Id,
                                                                      SecResourceId = l.SecResourceId,
                                                                      Status = l.Status,
                                                                      Method = l.Method,
                                                                      SecRoleId = l.SecRoleId
                                                                  }).ToList();

                if (resourcePermissions.Any())
                {
                    Session.Add("resourcePermission", resourcePermissions);
                }

                List<int> resLstInts = resourcePermissions.Select(s => s.SecResourceId).ToList();
                List<VmRolePermission> rolePermissions = (from l in database.RolePermissionDbSet
                                                          where resLstInts.Contains(l.SecResourceId) && l.SecRoleId == _erpManager.RoleId
                                                          select new VmRolePermission
                                                          {
                                                              SecResourceId = l.SecResourceId,
                                                              Id = l.Id,
                                                              SecRoleId = l.SecRoleId,
                                                              AddPermi = l.AddPermi,
                                                              DeletePermi = l.DeletePermi,
                                                              EditPermi = l.EditPermi,
                                                              PrintPermi = l.PrintPermi,
                                                              ReadonlyPermi = l.ReadonlyPermi
                                                          }).ToList();
                if (rolePermissions.Any())
                {
                    Session.Add("rolePermission", rolePermissions);
                }
            }
        }

        public ActionResult SecurityDashboard()
        {
            int moduleId = Convert.ToInt16(Request.QueryString["id"]);
            Session["sModuleId"] = moduleId;
            return View();
        }


        public ActionResult ModuleList()
        {
            return View();
        }

        #region User

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult ErpUser()
        {
            return View();
        }

        public JsonResult ActionResultd()
        {
            return Json(0, JsonRequestBehavior.DenyGet);

        }

        #endregion
        [AllowAnonymous]
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Login", "Security");
        }

    }
}