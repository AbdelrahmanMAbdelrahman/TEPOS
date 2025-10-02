using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Models.Security;
using ERP.Models.Security.Authorization;

namespace ERP.Controllers.Manager
{
    public class ErpManager : ISessionManager, ISessionMangers
    {
        readonly SessionManager _sessionManager = new SessionManager();
        readonly SessionManagers _sessionManagers = new SessionManagers();
        readonly ExceptionManager _exceptionManager = new ExceptionManager();

        public List<int> UserOffice => _sessionManagers.UserOffice;

        public string BranchNameList(List<int> branchId)
        {
          return  _sessionManagers.BranchNameList(branchId);
        }

    
        public void SetModulSession(int moduleId)
        {
            _sessionManager.SetModulSession(moduleId);
        }

        public JsonResult ActionResultDefault => _sessionManager.ActionResultDefault;
        public JsonResult SqlExceptionAdoDotNet(SqlException e)
        {
            return _exceptionManager.SqlExceptionAdoDotNet(e);
        }

        public JsonResult ExceptionDefault(Exception ex)
        {
            return _exceptionManager.ExceptionDefault(ex);
        }

        public JsonResult DbUpdateExceptionEntity(DbUpdateException ex)
        {
            return _exceptionManager.DbUpdateExceptionEntity(ex);
        }

        public int ModuleId => _sessionManagers.ModuleId;

        public JsonResult SessionTimeOut => _sessionManager.SessionTimeOut;

        public int CmnId => _sessionManagers.CmnId;

        public int SalesPriceIncOrExcVat => _sessionManagers.SalesPriceIncOrExcVat;

        public int UserId => _sessionManagers.UserId;

        public int RoleId => _sessionManagers.RoleId;

        public List<VmResourcePermission> GetResourcePermissions => _sessionManagers.GetResourcePermissions;

        public List<VmRolePermission> GetRolePermissions => _sessionManagers.GetRolePermissions; 

        public int GetModuleStatus(int moduleId)
        {
            return _sessionManagers.GetModuleStatus(moduleId);
        }

        public string CompanyName => _sessionManagers.CompanyName;

        public string CompanyAddress => _sessionManagers.CompanyAddress;

        public string CompanyPhone => _sessionManagers.CompanyPhone;

        public string CompanyEmail => _sessionManagers.CompanyEmail;

        public string RegistrationNo => _sessionManagers.RegistrationNo;

        public string VatRegNo => _sessionManagers.VatRegNo;

        public string BranchName => _sessionManagers.BranchName;

        public string BranchAddress => _sessionManagers.BranchAddress;

        public string UserName => _sessionManagers.UserName;

        public string TerminalId => _sessionManagers.TerminalId;

        public string BillingFooterText => _sessionManagers.BillingFooterText;

        public string PoweredBy => _sessionManagers.PoweredBy;
        public int MainLocationId => _sessionManagers.MainLocationId;
    }
}