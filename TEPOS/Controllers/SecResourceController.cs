using ERP.Models;
using ERP.Models.Security;
using ERP.Models.VModel;
using iTextSharp.text;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using ERP.Controllers;
public class SecResourceController : BaseController
{
    private readonly ConnectionDatabase _dbContext;

    public SecResourceController()
    {
        _dbContext = new ConnectionDatabase();
    }

    [HttpPost]
    public JsonResult AddChildResource(int parentResId, string childName, int secRoleId, int secModuleId)
    {
        if (string.IsNullOrEmpty(childName) || parentResId <= 0)
            return Json(new { success = false, message = "invalid data ." });

        try
        {
            // 1- إنشاء Resource
            var newResource = CreateResource(parentResId, childName, secModuleId);

            // 2- إنشاء ResourcePermission
            var newResourcePermission = CreateResourcePermission(newResource.Id, secRoleId, childName, parentResId, secModuleId);
            List<int> Roles = _dbContext.RoleDbSet.Select(r => r.Id).ToList();
            foreach (var id in Roles)
            {
                if (id == 1352) { continue; }// case Admin
                CreateResourcePermission(newResource.Id, id, childName, parentResId, secModuleId);

            }

            // 3- إنشاء RolePermission
            var newRolePermission = CreateRolePermission(newResource.Id, secRoleId);
            foreach (var id in Roles)
            {
                if (id == 1352) { continue; }// case Admin
                CreateRolePermission(newResource.Id, id);

            }
              var newResourceViewModel = BuildViewModel(newResource, newResourcePermission, newRolePermission, parentResId);

            return Json(new { success = true, resource = newResourceViewModel });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "error : " + ex.Message });
        }
    }


  

    private SecResource CreateResource(int parentResId, string childName, int secModuleId)
    {
        int nextSerial = _dbContext.ResourceDbSet.Any()
            ? _dbContext.ResourceDbSet.Max(r => r.Serial) + 1
            : 1;

        var resource = new SecResource
        {
            Name = childName,
            DisplayName = childName,
            SecResourceId = parentResId,
            SecModuleId = 2,
            Status = true,
            Serial = nextSerial,
            Level = parentResId,
            RoleLevel = 1,
            IsReport = false,
            CreateDate = DateTime.Now,
            CreateBy = 1
        };

        _dbContext.ResourceDbSet.Add(resource);
        _dbContext.SaveChanges();

        return resource;
    }

    private SecResourcePermission CreateResourcePermission(int newResourceId, int secRoleId, string childName, int parentResId, int secModuleId)
    {
        int nextSerial = _dbContext.ResourcePermissionDbSet.Any()
            ? _dbContext.ResourcePermissionDbSet.Max(r => r.Serial) + 1
            : 1;

        var resourcePermission = new SecResourcePermission
        {
            SecResourceId = newResourceId,
            SecRoleId = secRoleId,
            Name = childName,
            DisplayName = childName,
            SecModuleId = 2,
            Status = true,
            Serial = nextSerial,
            Level = parentResId,
            RoleLevel = 1,
            IsReport = false,
            CreateDate = DateTime.Now,
            CreateBy = 1
        };

        _dbContext.ResourcePermissionDbSet.Add(resourcePermission);
        _dbContext.SaveChanges();

        return resourcePermission;
    }

    private SecRolePermission CreateRolePermission(int newResourceId, int secRoleId)
    {
        var rolePermission = new SecRolePermission
        {
            SecResourceId = newResourceId,
            SecRoleId = secRoleId,
            AddPermi = true,
            ReadonlyPermi = true,
            EditPermi = true,
            DeletePermi = true,
            PrintPermi = true,
            CreateDate = DateTime.Now,
            CreateBy = 1
        };

        _dbContext.RolePermissionDbSet.Add(rolePermission);
        _dbContext.SaveChanges();

        return rolePermission;
    }

    private ResourceViewModel BuildViewModel(SecResource resource, SecResourcePermission resourcePermission, SecRolePermission rolePermission, int parentResId)
    {
        return new ResourceViewModel
        {
            ResId = resource.Id,
            RpermiId = resourcePermission.Id,
            RolePrmId = rolePermission.Id,
            Name = resource.Name,
            DisplayName = resource.DisplayName,
            Level = parentResId,
            RoleLevel = resource.RoleLevel,
            Status = resource.Status,
            IsReport = resource.IsReport,
            AddPermi = rolePermission.AddPermi,
            EditPermi = rolePermission.EditPermi,
            DeletePermi = rolePermission.DeletePermi
        };
    }
}
