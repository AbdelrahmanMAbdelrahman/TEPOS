using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.CSharpLib;


namespace ERP.Controllers.Manager
{
    public class ExceptionManager: Controller, IExceptionManager
    {
        public JsonResult DbUpdateExceptionEntity(DbUpdateException ex)
        {
            return Json(ExceptionController.ThrowSqlException(ex), JsonRequestBehavior.DenyGet);
        }

        public JsonResult ExceptionDefault(Exception ex)
        {
            return Json(ExceptionController.ThrowException(ex.Message), JsonRequestBehavior.DenyGet);
        }

        public JsonResult SqlExceptionAdoDotNet(SqlException e)
        {
            return Json(ExceptionController.ThrowSqlException(e.Number), JsonRequestBehavior.DenyGet);
        }
    }
}