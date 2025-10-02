using ERP.Controllers.Manager;
using ERP.Controllers.POS.Gateway;
using ERP.Controllers.POS.Manager;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Models.Security.Authorization;

namespace ERP.Controllers.POS
{
    [HasAuthorization]
    public class PosSchemeController : BaseController
    {
        ErpManager _erpManager = new ErpManager();

        [HasAuthorization(AccessLevel = 1)]
        public virtual ActionResult Scheme()
        {
            return View();
        }

        public ActionResult GetScheme()
        {
            try
            {
                ActionResult rtr = _erpManager.ActionResultDefault;
                SchemeGateway schemeGateway = new SchemeGateway();
                rtr = Json(schemeGateway.GetScheme(_erpManager.CmnId), JsonRequestBehavior.AllowGet);
                return rtr;
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult Save(string saveData)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;

            try
            {
                SchemeManager schemeManager = new SchemeManager();
                rtr = UtilityManager.JsonResultMax(Json(schemeManager.SaveScheme(saveData, _erpManager)), JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException e)
            {
                rtr = _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }
        [HasAuthorization(AccessLevel =3)]
        public ActionResult Update(string saveData)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;

            try
            {
                SchemeManager schemeManager = new SchemeManager();
                rtr = UtilityManager.JsonResultMax(Json(schemeManager.UpdateScheme(saveData, _erpManager)), JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException e)
            {
                rtr = _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }
        [HasAuthorization(AccessLevel =4 )]
        public ActionResult DeleteScheme(long schemeId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                SchemeGateway schemeGateway = new SchemeGateway();
                rtr = Json(schemeGateway.DeleteScheme(schemeId, _erpManager), JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException e)
            {
                rtr = _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;

        }

        public ActionResult GetSchemeById(int schemeId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                SchemeGateway schemeGateway = new SchemeGateway();
                rtr = Json(schemeGateway.GetSchemeById(schemeId, _erpManager), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }
    }
}