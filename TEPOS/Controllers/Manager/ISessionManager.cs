using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ERP.Models.Security.Authorization;

namespace ERP.Controllers.Manager
{
    interface ISessionManager
    {
        JsonResult SessionTimeOut { get; }
        JsonResult ActionResultDefault { get; }
        void SetModulSession(int moduleId);

    }
    interface ISessionMangers
    {
        int SalesPriceIncOrExcVat { get; }
        int CmnId { get; }
        int UserId { get; }
        int RoleId { get; }
        List<VmResourcePermission> GetResourcePermissions { get; }
        List<VmRolePermission> GetRolePermissions { get; }
        int GetModuleStatus(int moduleId);
        int ModuleId { get; }
        List<int> UserOffice { get; }

        string CompanyName { get; }
        string CompanyAddress { get; }
        string CompanyPhone { get; }
        string CompanyEmail { get; }
        string RegistrationNo { get; }
        string VatRegNo { get; }
        string BranchName { get; }
        string BranchAddress { get; }
        string BranchNameList(List<int> branchId);
        string UserName { get; }
        string TerminalId { get; }
        string BillingFooterText { get; }
        string PoweredBy { get; }
        int MainLocationId { get; }
    }
}
