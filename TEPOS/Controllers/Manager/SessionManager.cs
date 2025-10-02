using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using ERP.CSharpLib;
using ERP.Models;
using ERP.Models.Security;
using ERP.Models.Security.Authorization;


namespace ERP.Controllers.Manager
{
    public class SessionManager : Controller, ISessionManager
    {
        public void SetModulSession(int moduleId)
        {
            //Session["sModuleId"] = 0;
            //if (Session["sModuleId"] != null)
            //{
            //    Session["sModuleId"] = null;
            //}
            Session["sModuleId"] = moduleId;
        }

        public JsonResult ActionResultDefault => Json(0, JsonRequestBehavior.DenyGet);
        public JsonResult SessionTimeOut => Json(ExceptionController.ThrowException(ExceptionController.SessionTimeOut), JsonRequestBehavior.DenyGet);
        
   }


    public class SessionManagers : ISessionMangers
    {
        public int SalesPriceIncOrExcVat => Convert.ToInt32(HttpContext.Current.Session["SalesPriceIncOrExcVat"]);
        public string CompanyName => HttpContext.Current.Session["CompanyName"].ToString();
        public string CompanyAddress => HttpContext.Current.Session["CompanyAddress"].ToString();
        public string CompanyPhone => HttpContext.Current.Session["CompanyPhone"].ToString();
        public string CompanyEmail=>HttpContext.Current.Session["CompanyEmail"].ToString();
        public string RegistrationNo=> HttpContext.Current.Session["RegistrationNo"].ToString();
        public string VatRegNo => HttpContext.Current.Session["VatRegNo"].ToString();
        public string BranchName => HttpContext.Current.Session["BranchName"].ToString();

        public string BranchNameList(List<int> branchId)
        {
            using (ConnectionDatabase database=new ConnectionDatabase())
            {
                var branchLst = database.PosBrancheDbSet.Where(w => branchId.Contains(w.Id)).Select(s => new {s.Name}).ToList();
                return branchLst.Any() ? string.Join(",", branchLst.Select(s => s.Name).ToList()) : "Not found";
            }
        }
        public string BranchAddress => HttpContext.Current.Session["BranchAddress"].ToString();
        public string UserName => HttpContext.Current.Session["UserName"].ToString();
        public string TerminalId => HttpContext.Current.Session["TerminalId"].ToString();
        public string BillingFooterText => /*HttpContext.Current.Session["BillingFooterText"].ToString()*/"Thank you for shopping with 19011 Please download our mobile app for online purchase .  Purchase of defected item must be exchange by 24 hours with invoice.";
        public string PoweredBy =>/* HttpContext.Current.Session["PoweredBy"].ToString()*/"<b>Powered By: www.Trend-egypt.com</b>";
        public int GetModuleStatus(int moduleId)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                return database.ModuleDbSet.Count(c => c.Status && c.Id == moduleId);
            }

        }

        public int CmnId => Convert.ToInt32(HttpContext.Current.Session["cmnId"]);
        public int UserId => Convert.ToInt16(HttpContext.Current.Session["userId"]);
        public int RoleId => Convert.ToInt32(HttpContext.Current.Session["roleId"]);
        public int ModuleId => Convert.ToInt32(HttpContext.Current.Session["sModuleId"]);
        public List<int> UserOffice => (List<int>)HttpContext.Current.Session["userBranch"];
        public int MainLocationId => Convert.ToInt32(HttpContext.Current.Session["MainLocationId"]);
        public List<VmResourcePermission> GetResourcePermissions => (List<VmResourcePermission>) HttpContext.Current.Session["resourcePermission"];
        public List<VmRolePermission> GetRolePermissions => (List<VmRolePermission>) HttpContext.Current.Session["rolePermission"];
        
       
    }
}