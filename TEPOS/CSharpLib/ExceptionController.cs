using System;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Web;

namespace ERP.CSharpLib
{
    public class ExceptionController
    {
        public static string SessionTimeOut = "SessionTimeOut";
        public static string ModelStateStatus = "ModelStateFalse";


        public static int ThrowSqlException(int e)
        {
            HttpContext.Current.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return e;
        }

        public static int ThrowSqlException(SqlException e)
        {
            HttpContext.Current.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return e.Number;
        }

        public static int ThrowSqlException(DbUpdateException e)
        {
            SqlException ex = e.GetBaseException() as SqlException;
            HttpContext.Current.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return ex.Number;
        }


        public static string ThrowException(string e)
        {
            HttpContext.Current.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return e;
        }

        public static string ThrowException(Exception e)
        {
            HttpContext.Current.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return e.Message;
        }


    }
}