using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using ERP.Controllers.Manager;
using ERP.Models;
using ERP.Models.Security;
using ERP.CSharpLib;

namespace ERP.Controllers.Security.Gateway
{
    public class UserOfficeGateway
    {
        public dynamic GetUserOffice(int  cmnId)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                var datalist = (from usr in database.UserDbSet
                    where usr.CmnCompanyId == cmnId && database.UserBranchDbSet.Select(u => u.SecUserId).Contains(usr.Id)
                    select new
                    {
                        UserId = usr.Id,
                        UserName = usr.LoginName,
                        Offices = (from uOfc in database.UserBranchDbSet
                            join ofc in database.PosBrancheDbSet on uOfc.PosBranchId equals ofc.Id
                            where uOfc.SecUserId == usr.Id
                            select new
                            {
                                OfficeId = ofc.Id,
                                OfficName = ofc.Name
                            }).ToList()


                    }).ToList();
                return datalist;
            }
        }


        public int InsertUserOffice(string offices, int userId, ErpManager erpManager)
        {
            int[] officeIds = offices == "null" ? null : Array.ConvertAll(offices.Split(','), int.Parse);
            int ret = 0;
            using (ConnectionDatabase dbContext = new ConnectionDatabase())
            {
                if (officeIds != null)
                {
                    foreach (int oId in officeIds)
                    {
                        SecUserBranch userOffice = new SecUserBranch();
                        userOffice.SecUserId = userId;
                        userOffice.PosBranchId = oId;
                        userOffice.CmnCompanyId = erpManager.CmnId;
                        userOffice.CreatedBy = erpManager.UserId;
                        userOffice.CreatedDate = UTCDateTime.BDDateTime();
                        dbContext.UserBranchDbSet.Add(userOffice);
                    }
                }

                if (dbContext.ChangeTracker.HasChanges())
                {
                    ret = dbContext.SaveChanges();
                }
                return ret;
            }
        }

        public int UpdateUserOffice(string existingOfc, string updatedOfc, int existingUserId, ErpManager erpManager)
        {
            int[] existingOfcIds = existingOfc == "null" ? null : Array.ConvertAll(existingOfc.Split(','), int.Parse);
            int[] updatedOfcIds = updatedOfc == "null" ? null : Array.ConvertAll(updatedOfc.Split(','), int.Parse);
            if (updatedOfcIds != null && existingOfcIds != null)
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    //user office start
                    var deletedOfcIds = existingOfcIds.Except(updatedOfcIds);
                    var addedOfcIds = updatedOfcIds.Except(existingOfcIds);

                    var deletedOfc = dbContext.UserBranchDbSet.Where(a => deletedOfcIds.Contains(a.PosBranchId) && a.SecUserId == existingUserId).ToList();
                    deletedOfc.ForEach(d => dbContext.Entry(d).State = EntityState.Deleted);

                    //add new items to user office
                    foreach (int oId in addedOfcIds)
                    {
                        SecUserBranch userOffice = new SecUserBranch();
                        userOffice.SecUserId = existingUserId;
                        userOffice.PosBranchId = oId;
                        userOffice.CmnCompanyId = erpManager.CmnId;
                        userOffice.CreatedBy = erpManager.UserId;
                        userOffice.CreatedDate = UTCDateTime.BDDateTime();
                        dbContext.UserBranchDbSet.Add(userOffice);
                    }
                    return dbContext.SaveChanges();
                }
            }
            return 0;
        }

        public int DeleteUserOffice(int userId)
        {
            using (ConnectionDatabase dbContext = new ConnectionDatabase())
            {
                dbContext.UserBranchDbSet.RemoveRange(dbContext.UserBranchDbSet.Where(w => w.SecUserId == userId));
                return dbContext.SaveChanges();
            }
        }

    }
}