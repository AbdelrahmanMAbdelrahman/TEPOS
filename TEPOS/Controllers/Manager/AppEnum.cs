using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Controllers.Manager
{
    public enum PosInvoiceType
    {
        Active=1,
        Cancel=2,
        Hold=3,
        Credit=4,
        StockTransfar=5,
        CreditStockTransfer=6
    }
}