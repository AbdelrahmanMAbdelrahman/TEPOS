using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ERP.Controllers.Manager
{
    interface IExceptionManager
    {
        JsonResult DbUpdateExceptionEntity(DbUpdateException ex);
        JsonResult ExceptionDefault(Exception ex);
        JsonResult SqlExceptionAdoDotNet(SqlException e);
    }
}
