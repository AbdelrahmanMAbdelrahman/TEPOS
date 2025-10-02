using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP.Controllers.Manager;
using ERP.Controllers.POS.Gateway;
using ERP.Models.POS;
using ERP.Models.VModel;
using ERP.CSharpLib;

namespace ERP.Controllers.POS.Manager
{
    public class ItemRegisterManager
    {
        public static int Insert(PosProduct posProduct, String batchList, ErpManager erpManager)
        {
            List<VmItemBatch> vmItemBatches = (List<VmItemBatch>) Newtonsoft.Json.JsonConvert.DeserializeObject(batchList, typeof (List<VmItemBatch>));
            return ItemRegisterGateway.Insert(posProduct, vmItemBatches, erpManager);
        }

        public static int Update(PosProduct posProduct, String batchList, ErpManager erpManager)
        {
            posProduct.ModifiedBy = erpManager.UserId;
            posProduct.ModifideDate = UTCDateTime.BDDateTime();
            List<VmItemBatch> vmItemBatches = (List<VmItemBatch>)Newtonsoft.Json.JsonConvert.DeserializeObject(batchList, typeof(List<VmItemBatch>));
            return ItemRegisterGateway.Update(posProduct, vmItemBatches, erpManager);
        }
    }
}