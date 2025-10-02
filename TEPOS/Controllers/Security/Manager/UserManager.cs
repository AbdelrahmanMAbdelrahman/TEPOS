using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP.Controllers.Manager;
using ERP.Models;
using ERP.Models.Security;
using Microsoft.AspNet.Identity;

namespace ERP.Controllers.Security.Manager
{
    public class UserManager
    {
        public SecUser UserLoginInfo()
        {
            using (ConnectionDatabase database=new ConnectionDatabase())
            {
                int userId = new ErpManager().UserId;
                return database.UserDbSet.Find(userId);
            }
        }
    }
}