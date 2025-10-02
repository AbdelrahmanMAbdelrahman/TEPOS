using ERP.Controllers.Security.Gateway;
using ERP.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP.Controllers.Manager;
using ERP.Models;
using ERP.Models.VModel;

namespace ERP.Controllers.Security.Manager
{
    public class SecurityManager
    {
        public static List<SecModule> GetModuleList(int cmnId)
        {
            using(ConnectionDatabase dbContext=new ConnectionDatabase())
            {
                SecurityGateway gateway = new SecurityGateway();
              return gateway.GetModuleList(dbContext,cmnId);
            }
        }

        public  List<ResourcePermissionViewModel> ResourcePermissionList()
        {
            using (ConnectionDatabase dbContext = new ConnectionDatabase())
            {
                ErpManager erpManager = new ErpManager();
                int moduleId = erpManager.ModuleId;
                int roleId = erpManager.RoleId;
                if (dbContext.ModuleDbSet.Any(w => w.Status && w.Id == moduleId))
                {
                    return dbContext.ResourcePermissionDbSet
                        .Where(w => w.Status && w.SecModuleId == moduleId && w.SecRoleId == roleId)
                        .Select(s => new ResourcePermissionViewModel
                        {
                            DisplayName = s.DisplayName,
                            Icon = s.SecResource.IconUrl,
                            Id = s.Id,
                            IsReport = s.IsReport,
                            Level = s.Level,
                            Method = s.Method,
                            Name = s.Name,
                            RoleLevel = s.RoleLevel,
                            SecResourceId = s.SecResourceId,
                            SecRoleId = s.SecRoleId,
                            Serial = s.Serial,
                            Status = s.Status
                        }).ToList();
                }

                return new List<ResourcePermissionViewModel>();
            }
        }


    }
}