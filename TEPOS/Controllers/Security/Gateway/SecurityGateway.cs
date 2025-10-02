using ERP.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP.Models;

namespace ERP.Controllers.Security.Gateway
{
    public class SecurityGateway
    {
        public List<SecModule> GetModuleList(ConnectionDatabase dbContext,int cmnId)
        {
            return dbContext.ModuleDbSet.Where(c => c.CmnCompanyId == cmnId && c.Status).ToList();
        }


    }
}