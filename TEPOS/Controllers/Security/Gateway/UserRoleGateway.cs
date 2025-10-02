using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ERP.Controllers.Manager;
using ERP.Models;
using ERP.Models.Security;
using ERP.CSharpLib;

namespace ERP.Controllers.Security.Gateway
{
    public class UserRoleGateway
    {
        public dynamic GetUserRole(int cmnId)
        {
            using (ConnectionDatabase database=new ConnectionDatabase())
            {
                var lst = (from userrole in database.UserRoleDbSet
                           join usr in database.UserDbSet on userrole.SecUserId equals usr.Id
                           join role in database.RoleDbSet on userrole.SecRoleId equals role.Id
                           where usr.CmnCompanyId == cmnId
                           select new
                           {
                               Id = userrole.Id,
                               UserId = userrole.SecUserId,
                               RoleId = userrole.SecRoleId,
                               UserName = usr.LoginName,
                               RoleName = role.Name,
                               Status = userrole.Status
                           }).ToList();
                return lst;
            }
        }


        public int InsertUserRole(SecUserRole userRole, ErpManager erpManager)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                userRole.CreateBy = erpManager.UserId;
                userRole.CreateDate = UTCDateTime.BDDateTime();
                database.UserRoleDbSet.Add(userRole);
                return database.SaveChanges();
            }
        }

        public int UpdateUserRole(SecUserRole userRole, ErpManager erpManager)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                SecUserRole userRoleLst = database.UserRoleDbSet.FirstOrDefault(f => f.Id == userRole.Id);
                if (userRoleLst != null)
                {
                    userRoleLst.SecRoleId = userRole.SecRoleId;
                    userRoleLst.SecUserId = userRole.SecUserId;
                    userRoleLst.Status = userRole.Status;
                    userRoleLst.ModifiedBy = erpManager.UserId;
                    userRoleLst.ModifiedDate = UTCDateTime.BDDateTime();
                    return database.SaveChanges();
                }
                return 0;
            }
        }

        public int DeleteUserRole(int id)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                SecUserRole userRoleLst = new SecUserRole {Id = id};
                database.Entry(userRoleLst).State = EntityState.Deleted;
                return database.SaveChanges();

            }
        }
    }
}