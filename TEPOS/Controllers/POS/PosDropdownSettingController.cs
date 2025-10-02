using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Controllers.Manager;
using ERP.Models;
using ERP.Models.Security.Authorization;

namespace ERP.Controllers.POS
{
    [HasAuthorization]
    public class PosDropdownSettingController : BaseController
    {
        ErpManager _erpManager = new ErpManager();

        public ActionResult GetCustomerClasses()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    int cmnId = _erpManager.CmnId;
                    var data = (from cls in dbContext.PosCustomerClassDbSet
                        where cls.CmnCompanyId == cmnId && cls.Status
                        select new
                        {
                            cls.Id,
                            cls.Name
                        }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(data), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetCustomerRegions()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    var data = (from rgn in dbContext.PosRegionDbSet
                        select new
                        {
                            rgn.Id,
                            rgn.Name

                        }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(data), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetCustomerNearestCity()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    var data = (from cty in dbContext.PosCityOrNearestZoneDbSet
                        select new
                        {
                            cty.Id,
                            cty.Name

                        }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(data), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetTenders()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    var data = (from tndr in dbContext.PosTenderTypeDbSet
                        select new
                        {
                            tndr.Id,
                            tndr.Name
                        }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(data), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetUnitGroupDetails()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    var data = (from cls in dbContext.PosUomGroupDbSet
                                select new
                                {
                                    cls.Id,
                                    cls.Name
                                }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(data), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetUnitGroupMasters()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    int cmnId = _erpManager.CmnId;
                    var data = (from posUM in dbContext.PosUomMasterDbSet
                                select new
                                {
                                    posUM.Id,
                                    Name=posUM.UomCode
                                }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(data), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetAllBranchList()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    var data = (from l in dbContext.PosBrancheDbSet
                                select new
                                {
                                    l.Id,
                                   l.Name
                                }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(data), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetBranchList()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    var data = (//from l in database.PosBrancheList
                                from l in dbContext.PosBrancheDbSet
                                select new
                                {
                                    l.Id,
                                    l.Name
                                }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(data), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

    }
}


