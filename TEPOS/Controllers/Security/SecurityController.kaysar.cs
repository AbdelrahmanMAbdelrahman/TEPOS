using ERP.Controllers.Manager;
using ERP.Controllers.Security.Gateway;
using ERP.CSharpLib;
using ERP.Models;
using ERP.Models.Security;
using ERP.Models.Security.Authorization;
using ERP.Models.VModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using VmRolePermission = ERP.Models.Security.Authorization.VmRolePermission;

namespace ERP.Controllers.Security
{
    public partial class SecurityController : Controller
    {
        #region User

        public ActionResult GetUsers()
        {
            try
            {
                return UtilityManager.JsonResultMax(Json(new UserGateway().GetUsers(_erpManager)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult InsertUser(SecUser secUser)
        {
            try
            {
                return Json(new UserGateway().InsertUser(secUser, _erpManager), JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException.InnerException.Message.Contains("UK_SecUsers_UserCode"))
                {
                    return Json("UserCodeExists", JsonRequestBehavior.DenyGet);
                }
                return _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateUser(SecUser secUser)
        {
            try
            {
                return Json(new UserGateway().UpdateUser(secUser, _erpManager), JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException.InnerException.Message.Contains("UK_SecUsers_UserCode"))
                {
                    return Json("UserCodeExists", JsonRequestBehavior.DenyGet);
                }
                return _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        [HasAuthorization(AccessLevel = 4)]
        public ActionResult DeleteUser(int id)
        {
            try
            {
                return Json(new UserGateway().DeleteUser(id), JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException e)
            {
                return _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        #endregion


        #region Role

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult Role()
        {
            return View();
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult InsertRole(SecRole role)
        {
            try
            {
                return Json(new RoleGateway().InsertRole(role, _erpManager), JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException e)
            {
                return _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateRole(SecRole role)
        {
            try
            {
                return Json(new RoleGateway().UpdateRole(role, _erpManager), JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException e)
            {
                return _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        public ActionResult GetRole()
        {
            try
            {
                return UtilityManager.JsonResultMax(Json(new RoleGateway().GetRole(_erpManager)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        [HasAuthorization(AccessLevel = 4)]
        public ActionResult DeleteRole(int id)
        {
            try
            {
                return Json(new RoleGateway().DeleteRole(id), JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException e)
            {
                return _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        #endregion


        #region User Role

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult UserRole()
        {
            return View();
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult InsertUserRole(SecUserRole usrRole)
        {
            try
            {
                return Json(new UserRoleGateway().InsertUserRole(usrRole, _erpManager), JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException e)
            {
                return _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateUserRole(SecUserRole role)
        {
            try
            {
                return Json(new UserRoleGateway().UpdateUserRole(role, _erpManager), JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException e)
            {
                return _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        public ActionResult GetUserRole()
        {
            try
            {
                return UtilityManager.JsonResultMax(Json(new UserRoleGateway().GetUserRole(_erpManager.CmnId)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        [HasAuthorization(AccessLevel = 4)]
        public ActionResult DeleteUserRole(int id)
        {
            try
            {
                return Json(new UserRoleGateway().DeleteUserRole(id), JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException e)
            {
                return _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        #endregion


        #region RolePermission
     

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult RolePermission()
        {

            using (ConnectionDatabase dbcontext = new ConnectionDatabase())
            {
                int roleId = 0;
                if (Session["temproleId"] != null)
                {
                    roleId = Convert.ToInt32(Session["temproleId"]);
                }
                List<ResourceViewModel> resourceList = (from rp in dbcontext.ResourcePermissionDbSet
                    join res in dbcontext.ResourceDbSet on rp.SecResourceId equals res.Id
                    join rolePrmi in dbcontext.RolePermissionDbSet on res.Id equals rolePrmi.SecResourceId
                    where rp.SecRoleId == roleId && rolePrmi.SecRoleId == roleId && rp.Level != 0
                    select new ResourceViewModel
                    {
                        RpermiId = rp.Id,
                        ResId = rp.SecResourceId,
                        Name = res.DisplayName,
                        DisplayName = rp.DisplayName,
                        PermiId = rp.Id,
                        SecResourceId = res.SecResourceId,
                        Status = rp.Status,
                        Level = rp.Level,
                        AddPermi = rolePrmi.AddPermi,
                        EditPermi = rolePrmi.EditPermi,
                        DeletePermi = rolePrmi.DeletePermi,
                        RolePrmId = rolePrmi.Id,
                        RoleLevel = rp.RoleLevel,
                        IsReport = rp.IsReport
                    }).ToList();
                Session["temproleId"] = null;
                ViewBag.ResourceList = resourceList.Count > 0 ? resourceList : new List<ResourceViewModel>();
                return View();
            }
        }

        public ActionResult SetRoleIdForResourcePermission(int roleId)
        {
            ActionResult rtr = null;
            try
            {
                if (roleId > 0)
                {
                    Session["temproleId"] = roleId;
                    return Json(1, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception e)
            {
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            return rtr;
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult UpdateRoleWithResourcePermission(string objRoleWithResourcePermission)
        {
            try
            {
                List<VmUserRolePermission> rolePermission = (List<VmUserRolePermission>) JsonConvert.DeserializeObject(objRoleWithResourcePermission, typeof (List<VmUserRolePermission>));
                return UtilityManager.JsonResultMax(Json(new RolePermissionGateway().UpdateRoleWithResourcePermission(rolePermission, _erpManager)), JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateResourcePermission(int id, string displayName)
        {
            try
            {
                return UtilityManager.JsonResultMax(Json(new RolePermissionGateway().UpdateResourcePermission(id, displayName, _erpManager)), JsonRequestBehavior.DenyGet);
            }
            catch (DbUpdateException e)
            {
                return _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        #endregion

        #region UserOffice

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult UserOffice()
        {
            return View();
        }

        public ActionResult GetUserOffice()
        {
            try
            {
                return UtilityManager.JsonResultMax(Json(new UserOfficeGateway().GetUserOffice(_erpManager.CmnId)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult InsertUserOffice(string offices, int userId)
        {
            try
            {
                return Json(new UserOfficeGateway().InsertUserOffice(offices, userId, _erpManager), JsonRequestBehavior.DenyGet);

            }
            catch (DbUpdateException ex)
            {
                return _erpManager.DbUpdateExceptionEntity(ex);
            }
            catch (Exception ex)
            {
                return _erpManager.ExceptionDefault(ex);
            }

        }

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateUserOffice(string existingOfc, string updatedOfc, int existingUserId)
        {
            try
            {
                return Json(new UserOfficeGateway().UpdateUserOffice(existingOfc, updatedOfc, existingUserId, _erpManager), JsonRequestBehavior.DenyGet);

            }
            catch (DbUpdateException ex)
            {
                return _erpManager.DbUpdateExceptionEntity(ex);
            }
            catch (Exception ex)
            {
                return _erpManager.ExceptionDefault(ex);
            }

        }

        [HasAuthorization(AccessLevel = 4)]
        public ActionResult DeleteUserOffice(int userId)
        {
            try
            {
                return Json(new UserOfficeGateway().DeleteUserOffice(userId), JsonRequestBehavior.DenyGet);

            }
            catch (DbUpdateException ex)
            {
                return _erpManager.DbUpdateExceptionEntity(ex);
            }
            catch (Exception ex)
            {
                return _erpManager.ExceptionDefault(ex);
            }

        }

        #endregion
        

        #region company

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult Company()
        {
            using (ConnectionDatabase database=new ConnectionDatabase())
            {
                return View(database.CompanyDbSet.Find(_erpManager.CmnId));
            }
            
        }

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateCompany(CmnCompany company)
        {
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {

                    HttpPostedFileBase isLogo = company.LogoFile;
                    string serverPath = "/Constants/Company/" + _erpManager.CmnId;
                    string folderPath = Path.Combine(Server.MapPath("~"+serverPath));
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    if (isLogo != null)
                    {
                        company.Logo = serverPath + "/" + company.LogoFile.FileName;
                        if (System.IO.File.Exists( folderPath + "/" + company.LogoFile.FileName))
                        {
                            System.IO.File.Delete(folderPath + "/" + company.LogoFile.FileName);
                        }
                        var logoDbUrl = database.CompanyDbSet.Where(a => a.Id == _erpManager.CmnId).Select(s => s.Logo).FirstOrDefault();
                        if (System.IO.File.Exists(Path.Combine(Server.MapPath("~" + logoDbUrl))))
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath("~" + logoDbUrl)));
                        }
                        Request.Files[0].SaveAs(Path.Combine(Server.MapPath(serverPath+"/"),Path.GetFileName(company.LogoFile.FileName)));
                       
                    }
                    company.Id = _erpManager.CmnId;
                    database.Entry(company).State=EntityState.Modified;
                    if (company.LogoFile == null)
                    {
                        database.Entry(company).Property(s => s.CompanyIdOnExs).IsModified = false;
                    }
                 return Json (database.SaveChanges(),JsonRequestBehavior.DenyGet);
                }
            }
            catch (DbUpdateException ex)
            {
                return _erpManager.DbUpdateExceptionEntity(ex);
            }
            catch (Exception ex)
            {
                return _erpManager.ExceptionDefault(ex);
            }

        }

        #endregion

        [AllowAnonymous]
        public ActionResult NewLogin()
        {
            return View();
        }

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