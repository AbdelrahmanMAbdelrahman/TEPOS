using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Controllers.Manager;
using ERP.Models;

namespace ERP.Controllers.POS
{
    public partial class PosDropDownController:BaseController
    {
        public ActionResult GetUser()
        {
            ActionResult rtr;
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    var lst = (from l in database.UserDbSet
                        orderby l.LoginName ascending
                        where l.CmnCompanyId == _erpManager.CmnId && l.Status
                        select new
                        {
                            l.Id,
                            Name = l.LoginName
                        }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetRole()
        {
            ActionResult rtr;
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    var lst = (from l in database.RoleDbSet
                        orderby l.Name ascending
                        where l.CmnCompanyId == _erpManager.CmnId && l.Status
                        select new
                        {
                            l.Id,
                            Name = l.Name
                        }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);

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