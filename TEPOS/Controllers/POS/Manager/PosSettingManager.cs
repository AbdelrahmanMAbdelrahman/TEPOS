using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Controllers.Manager;
using ERP.Models;
using ERP.Models.POS;
using ERP.CSharpLib;

namespace ERP.Controllers.POS.Manager
{
    public class PosSettingManager
    {
        ErpManager _erpManager = new ErpManager();

        public static int SaveBranch(PosBranch branch, ErpManager erpManager)
        {

                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    int cmnId = erpManager.CmnId;
                    int userId = erpManager.UserId;

                    branch.CmnCompanyId = cmnId;
                    branch.CreatedBy = userId;
                    branch.CreatedDate = UTCDateTime.BDDateTime();

                    dbContext.PosBrancheDbSet.Add(branch);
                    return dbContext.SaveChanges();
                }
        }
    }
}