using ERP.Controllers.Manager;
using ERP.Controllers.POS.Gateway;
using ERP.Controllers.POS.Manager;
using ERP.CSharpLib;
using ERP.Models;
using ERP.Models.POS;
using ERP.Models.Security.Authorization;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace ERP.Controllers.POS
{

    public partial class PosSettingController : Controller
    {
        ErpManager _erpManager = new ErpManager();



        #region Item register

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult ItemRegister()
        {
            return View();
        }

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult ItemEntry()
        {
            return View();
        }

        public ActionResult GetItemBatch(int productId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                rtr = UtilityManager.JsonResultMax(Json(new ItemRegisterGateway().GetItemBatch(productId)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetBarcodeByItemWise(int productId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                rtr = UtilityManager.JsonResultMax(Json(new ItemRegisterGateway().GetBarcodeByItemWise(productId)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetUomDetailsByUomGroupId(int uomGroupId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                ItemRegisterGateway gateway = new ItemRegisterGateway();
                rtr = UtilityManager.JsonResultMax(Json(gateway.GetUomDetailsByUomGroupId(uomGroupId)), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }


        public ActionResult GetItemList()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                ItemRegisterGateway gateway = new ItemRegisterGateway();
                rtr = UtilityManager.JsonResultMax(Json(gateway.GetItemList(_erpManager.CmnId)), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateItemRegister(ItemRegister register, String itemBatch, int productId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                register.Product.Id = productId;
                rtr = UtilityManager.JsonResultMax(Json(ItemRegisterManager.Update(register.Product, itemBatch, _erpManager)), JsonRequestBehavior.AllowGet);

            }
            catch (DbUpdateException ex)
            {
                rtr = _erpManager.DbUpdateExceptionEntity(ex);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult InsertItemRegister(ItemRegister register, String itemBatch)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                rtr = UtilityManager.JsonResultMax(Json(ItemRegisterManager.Insert(register.Product, itemBatch, _erpManager)), JsonRequestBehavior.AllowGet);

            }
            catch (DbUpdateException ex)
            {
                rtr = _erpManager.DbUpdateExceptionEntity(ex);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        [HasAuthorization(AccessLevel = 4)]
        public ActionResult DeleteProductById(int productId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                rtr = UtilityManager.JsonResultMax(Json(ItemRegisterGateway.Delete(productId)), JsonRequestBehavior.AllowGet);

            }
            catch (DbUpdateException ex)
            {
                rtr = _erpManager.DbUpdateExceptionEntity(ex);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
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



        #endregion end of item register



        #region Uom Master

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult UomMaster()
        {
            return View();
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult SaveUnitMaster( PosUomMaster master)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                int cmnId = _erpManager.CmnId;
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    master.CmnCompanyId = cmnId;
                    master.CreatedDate = UTCDateTime.BDDateTime();
                    master.CreatedBy = _erpManager.UserId;
                    dbContext.PosUomMasterDbSet.Add(master);
                    rtr = Json(dbContext.SaveChanges(), JsonRequestBehavior.AllowGet);
                }
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

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateUnitMaster(PosUomMaster master)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    master.ModifiedBy = _erpManager.UserId;
                    master.ModifideDate = UTCDateTime.BDDateTime();
                    dbContext.Entry(master).State = EntityState.Modified;
                    dbContext.Entry(master).Property(o => o.CreatedDate).IsModified = false;
                    dbContext.Entry(master).Property(o => o.CreatedBy).IsModified = false;
                    dbContext.Entry(master).Property(o => o.CmnCompanyId).IsModified = false;
                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
                }
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

        [HasAuthorization(AccessLevel = 4)]
        public ActionResult DeleteUnitMaster(int id)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    PosUomMaster master = new PosUomMaster() {Id = id};
                    dbContext.Entry(master).State = EntityState.Deleted;
                    return Json(dbContext.SaveChanges(), JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetUnitMaster()
        {
            try
            {
                using (ConnectionDatabase database=new ConnectionDatabase())
                {
                    var lst= database.PosUomMasterDbSet.Where(w => w.CmnCompanyId == _erpManager.CmnId).Select(s => new {s.Id,s.UomCode, s.UomDescription, s.IsBaseUom}).ToList();
                    return UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        #endregion

    }
}