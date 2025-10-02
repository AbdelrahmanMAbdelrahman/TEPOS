using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using ERP.Controllers.Manager;
using ERP.Models;
using ERP.Models.Security;
using ERP.Models.VModel;

using ERP.CSharpLib;

namespace ERP.Controllers.Security.Gateway
{
    public class RolePermissionGateway
    {

        public int UpdateResourcePermission(int id, string displayName, ErpManager erpManager)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                SecResourcePermission resourcePermission = new SecResourcePermission() {Id = id};
                resourcePermission.Name = "N/A";
                resourcePermission.DisplayName = displayName;
                resourcePermission.ModifiedBy = erpManager.UserId;
                resourcePermission.ModifiedDate = UTCDateTime.BDDateTime();
                database.ResourcePermissionDbSet.Attach(resourcePermission);
                database.Entry(resourcePermission).Property(p => p.DisplayName).IsModified = true;
                database.Entry(resourcePermission).Property(p => p.ModifiedBy).IsModified = true;
                database.Entry(resourcePermission).Property(p => p.ModifiedDate).IsModified = true;
                return database.SaveChanges();
            }
        }

        public int UpdateRoleWithResourcePermission(List<VmUserRolePermission> rolePermission, ErpManager erpManager)
        {
            int ret = 0;
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                using (var trn = database.Database.BeginTransaction())
                {
                    foreach (VmUserRolePermission rol in rolePermission)
                    {
                        if (rol.Flag == 1)
                        {
                            SecResourcePermission resourcePermission = new SecResourcePermission() {Id = (int) rol.ResourcePermissionId};
                            resourcePermission.Name = "N/A";
                            resourcePermission.DisplayName = "N/A";
                            resourcePermission.Status = rol.ResourceStatus;
                            resourcePermission.ModifiedBy = erpManager.UserId;
                            resourcePermission.ModifiedDate = UTCDateTime.BDDateTime();
                            database.ResourcePermissionDbSet.Attach(resourcePermission);
                            database.Entry(resourcePermission).Property(p => p.Status).IsModified = true;
                            database.Entry(resourcePermission).Property(p => p.ModifiedBy).IsModified = true;
                            database.Entry(resourcePermission).Property(p => p.ModifiedDate).IsModified = true;
                            ret += database.SaveChanges();
                            database.Entry(resourcePermission).State = EntityState.Detached;
                        }
                        else if (rol.Flag == 2)
                        {
                            SecRolePermission roleP = new SecRolePermission {Id = rol.RolePermissionId};
                            roleP.AddPermi = rol.Add ?? false;
                            roleP.EditPermi = rol.Edit ?? false;
                            roleP.DeletePermi = rol.Delete ?? false;
                            roleP.ModifiedBy = erpManager.UserId;
                            roleP.ModifiedDate = UTCDateTime.BDDateTime();
                            roleP.PrintPermi = rol.Add ?? false;
                            database.RolePermissionDbSet.Attach(roleP);
                            database.Entry(roleP).Property(p => p.AddPermi).IsModified = rol.Add != null;
                            database.Entry(roleP).Property(p => p.EditPermi).IsModified = rol.Edit != null;
                            database.Entry(roleP).Property(p => p.DeletePermi).IsModified = rol.Delete != null;
                            database.Entry(roleP).Property(p => p.PrintPermi).IsModified = rol.Add != null;
                            database.Entry(roleP).Property(p => p.ModifiedBy).IsModified = true;
                            database.Entry(roleP).Property(p => p.ModifiedDate).IsModified = true;
                            ret += database.SaveChanges();
                            database.Entry(roleP).State = EntityState.Detached;
                        }
                    }
                    trn.Commit();
                }
            }
            return ret;
        }


        


    }
}