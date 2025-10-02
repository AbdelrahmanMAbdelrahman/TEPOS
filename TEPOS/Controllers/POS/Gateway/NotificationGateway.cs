using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP.Controllers.Manager;
using ERP.Models;
using ERP.Models.VModel;

namespace ERP.Controllers.POS.Gateway
{
    public class NotificationGateway
    {
        public List<VmStockTransferInvoiceNo> GetListOfBranchTransferStockInvoice(List<int> branchList)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                var lst = (from inv in database.PosSalesInvoiceDbSet
                    join c in database.PosCustomerDbSet on inv.PosCustomerId equals c.Id
                    where
                        inv.IsReceiveTransferStock == false && inv.PosInvoiceType == PosInvoiceType.StockTransfar &&
                        branchList.Contains(c.PosBranchId)
                    select new VmStockTransferInvoiceNo
                    {
                        InvoiceNumber = inv.InvoiceNumber,
                        InvDate = inv.InvDate,
                        CreatedDate = inv.CreatedDate ?? DateTime.MinValue
                    }).ToList();
                return lst;
            }

        }
    }
}