using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ERP.Report.ReportController
{
    /// <summary>
    /// this is custom class for extend report execution timeout
    /// devlopement by kaysar
    /// </summary>
    public sealed class CommandTimeOut
    {
        public static void ChangeTimeout(Component component, int timeout)
        {
            if (!component.GetType().Name.Contains("TableAdapter"))
            {
                return;
            }

            PropertyInfo adapterProp = component.GetType().GetProperty("CommandCollection", BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance);
            if (adapterProp == null)
            {
                return;
            }

            SqlCommand[] command = adapterProp.GetValue(component, null) as SqlCommand[];

            if (command == null)
            {
                return;
            }

            command[0].CommandTimeout = timeout;
        }
    }
}