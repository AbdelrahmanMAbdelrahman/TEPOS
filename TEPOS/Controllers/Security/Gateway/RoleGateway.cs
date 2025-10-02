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
    public class RoleGateway
    {
        public int InsertRole(SecRole role, ErpManager erpManager)
        {
            int rtr = 0;
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                using (var trn = database.Database.BeginTransaction())
                {
                    role.CmnCompanyId = erpManager.CmnId;
                    role.SecUserId = erpManager.UserId;
                    role.CreateBy = erpManager.UserId;
                    role.Level = 0;
                    role.CreateDate = UTCDateTime.BDDateTime();
                    database.RoleDbSet.Add(role);
                    rtr += database.SaveChanges();
                    int[]  resId = database.UserDbSet.Any(a => a.Id == erpManager.UserId && a.Type == "sadmin") ? database.ResourceDbSet.Where(w => w.Status).Select(s => s.Id).ToArray() : database.ResourcePermissionDbSet.Where(w => w.SecRoleId == erpManager.RoleId && w.Status).Select(s => s.SecResourceId).ToArray();
                    var resourceLst = (from resource in database.ResourceDbSet
                        where resId.Contains(resource.Id)
                                       select new
                        {
                            resource.Id,
                            resource.Name,
                            resource.DisplayName,
                            resource.SecModuleId,
                            resource.Status,
                            resource.Serial,
                            resource.Level,
                            resource.Method,
                            resource.IsReport,
                            resource.RoleLevel
                        }).ToList();
                    List<SecResourcePermission> resP = new List<SecResourcePermission>();
                    List<SecRolePermission> rolP = new List<SecRolePermission>();
                    foreach (var lst in resourceLst)
                    {
                        SecResourcePermission resourcePermission = new SecResourcePermission();
                        resourcePermission.DisplayName = lst.DisplayName;
                        resourcePermission.Level = lst.Level;
                        resourcePermission.Method = lst.Method;
                        resourcePermission.Name = lst.Name;
                        resourcePermission.SecModuleId = lst.SecModuleId;
                        resourcePermission.SecResourceId = lst.Id;
                        resourcePermission.SecRoleId = role.Id;
                        resourcePermission.Serial = lst.Serial;
                        resourcePermission.IsReport = lst.IsReport;
                        resourcePermission.RoleLevel = lst.RoleLevel;
                        resourcePermission.Status = true;
                        resourcePermission.CreateBy = erpManager.UserId;
                        resourcePermission.CreateDate = UTCDateTime.BDDateTime();
                        resP.Add(resourcePermission);

                        SecRolePermission rolePermission = new SecRolePermission();
                        rolePermission.SecRoleId = role.Id;
                        rolePermission.SecResourceId = lst.Id;
                        rolePermission.AddPermi = true;
                        rolePermission.EditPermi = true;
                        rolePermission.DeletePermi = true;
                        rolePermission.PrintPermi = true;
                        rolePermission.ReadonlyPermi = true;
                        rolePermission.CreateBy = erpManager.UserId;
                        rolePermission.CreateDate = UTCDateTime.BDDateTime();
                        rolP.Add(rolePermission);
                    }
                    database.ResourcePermissionDbSet.AddRange(resP);
                    database.RolePermissionDbSet.AddRange(rolP);
                    rtr += database.SaveChanges();
                    trn.Commit();
                }
            }
            return rtr;
        }


        public dynamic GetRole(ErpManager erp)
        {
            using (ConnectionDatabase database=new ConnectionDatabase())
            {
                return database.RoleDbSet.Where(w=>w.CmnCompanyId==erp.CmnId).Select(s => new {s.Id,s.Name, s.Status}).ToList();
            }
        }

        public int DeleteRole(int id)
        {
            int rtr = 0;
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                using (var trn = database.Database.BeginTransaction())
                {
                    database.ResourcePermissionDbSet.RemoveRange(database.ResourcePermissionDbSet.Where(s => s.SecRoleId == id));
                    rtr += database.SaveChanges();
                    database.RolePermissionDbSet.RemoveRange(database.RolePermissionDbSet.Where(w => w.SecRoleId == id));
                    rtr += database.SaveChanges();
                    SecRole role = new SecRole {Id = id};
                    database.Entry(role).State = EntityState.Deleted;
                    rtr += database.SaveChanges();
                    trn.Commit();
                }
                return rtr;

            }
        }


        public int UpdateRole(SecRole role, ErpManager erpManager)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                var roleLst = database.RoleDbSet.FirstOrDefault(w => w.Id == role.Id);
                if (roleLst != null)
                {
                    roleLst.Name = role.Name;
                    roleLst.Status = role.Status;
                    roleLst.ModifiedBy = erpManager.UserId;
                    role.ModifiedDate = UTCDateTime.BDDateTime();
                    return database.SaveChanges();
                }
                return 0;
            }
        }

    }
}