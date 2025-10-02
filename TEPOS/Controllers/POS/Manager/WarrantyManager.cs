using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using ERP.Controllers.Manager;
using ERP.Models;

namespace ERP.Controllers.POS.Manager
{
    public class WarrantyManager
    {

        public dynamic GetWarrantyProduct(long invoiceNo)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                var lstWarranty = (from w in database.PosSalesProductWarrantyIssueDbSet
                    join siP in database.PosSalesInvoiceProductsDbSet on w.PosSalesInvoiceProductId equals siP.Id
                    join pb in database.PosProductBatchDbSet on siP.PosProductBatchId equals pb.Id
                    join prd in database.PosProductDbSet on pb.PosProductId equals prd.Id
                    join inv in database.PosSalesInvoiceDbSet on siP.PosSalesInvoiceId equals inv.Id
                    where inv.InvoiceNumber == invoiceNo
                    select new
                    {
                        w.PosSalesInvoiceProductId,
                        prd.Code,
                        ProductName = prd.Name,
                        pb.BatchName,
                        siP.Qty,
                        w.SerialOrImeiNo,
                        w.WarrantyPeriod,
                        w.WarrantyType,
                        w.Remarks
                    });

                var lstOfInvoiceProduct = (from siP in database.PosSalesInvoiceProductsDbSet
                    join pb in database.PosProductBatchDbSet on siP.PosProductBatchId equals pb.Id
                    join prd in database.PosProductDbSet on pb.PosProductId equals prd.Id
                    join inv in database.PosSalesInvoiceDbSet on siP.PosSalesInvoiceId equals inv.Id
                    where inv.InvoiceNumber == invoiceNo && !lstWarranty.Select(s=>s.PosSalesInvoiceProductId).ToList().Contains(siP.Id)
                    select new
                    {
                        PosSalesInvoiceProductId = siP.Id,
                        prd.Code,
                        ProductName = prd.Name,
                        pb.BatchName,
                        siP.Qty,
                        SerialOrImeiNo = "",
                        WarrantyPeriod = 1,
                        WarrantyType = 1,
                        Remarks = ""
                    });
                var lstOfUnion = lstWarranty.Union(lstOfInvoiceProduct).ToList();
                return lstOfUnion;
            }
        }

    }
}