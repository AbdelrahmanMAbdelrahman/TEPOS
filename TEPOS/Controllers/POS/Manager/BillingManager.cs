using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using ERP.Controllers.Manager;
using ERP.Controllers.POS.Gateway;
using ERP.Models;
using ERP.Models.POS;
using iTextSharp.text.pdf;
using ERP.CSharpLib;
using ERP.Models.Security;
using ERP.Models.VModel;

namespace ERP.Controllers.POS.Manager
{
    public class BillingManager
    {
        public int CancelInvoice(long invoiceNo, int posInvoiceType, string UserCode, ErpManager erpManager)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                SecUser userLst = database.UserDbSet.AsNoTracking().FirstOrDefault(f => f.Id == erpManager.UserId);
                if (UserCode == null)
                {
                    if (userLst != null)
                    {

                        UserCode = userLst.UserCode;
                    }
                }

                var invId = database.PosSalesInvoiceDbSet.Where(f => f.InvoiceNumber == invoiceNo && f.PosInvoiceType == (posInvoiceType > 0 ? (PosInvoiceType)posInvoiceType : f.PosInvoiceType)).Select(s => s.Id).FirstOrDefault();
                if (invId > 0)
                {
                    var salesInvBill = database.PosSalesInvoiceDbSet.Where(f => f.InvoiceNumber == invoiceNo && f.PosInvoiceType == (posInvoiceType > 0 ? (PosInvoiceType)posInvoiceType : f.PosInvoiceType)).FirstOrDefault();
                    //2025
                    //cannot return item when invoice created befor 14 days
                    if (salesInvBill.CreatedDate != null)
                    {
                        var x = salesInvBill.CreatedDate.Value;
                        TimeSpan diffOfDates = DateTime.Now - x;
                        if ((int)diffOfDates.TotalDays > 14)
                        {
                            return -00014;
                        }
                    }

                    var salesInvoice = database.PosSalesInvoiceDbSet.FirstOrDefault(s => s.Id == invId);

                    if (salesInvoice != null)
                    {
                        // Update the invoice properties
                        salesInvoice.PosInvoiceType = PosInvoiceType.Cancel;
                        salesInvoice.ModifiedBy = erpManager.UserId;
                        salesInvoice.ModifideDate = UTCDateTime.BDDateTime();
                        salesInvoice.UserCode = UserCode;

                        // Mark the entity as modified
                        database.Entry(salesInvoice).State = EntityState.Modified;

                        // Save changes to the database
                        return database.SaveChanges();
                    }

                }
                return -0001;
            }
        }


        public int CancelPurchaseInvoice(string invoiceNo, int posInvoiceType, ErpManager erpManager)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                SecUser userLst = database.UserDbSet.AsNoTracking().FirstOrDefault(f => f.Id == erpManager.UserId);
                SecUserBranch Userbranch = database.UserBranchDbSet.AsNoTracking().FirstOrDefault(f => f.SecUserId == userLst.Id);
                PosBranch branch = database.PosBrancheDbSet.AsNoTracking().FirstOrDefault(f => f.Id == Userbranch.PosBranchId);

                var invId = database.PosStockDbSet.Where(f => f.InvReferenceNo == invoiceNo && (f.PosInvoiceType == (posInvoiceType > 0 ? (PosInvoiceType)posInvoiceType : f.PosInvoiceType) && branch.Id == f.PosBranchId)).Select(s => s.Id).FirstOrDefault();
                if (invId > 0)
                {
                    var CompanyInvNo = database.PosStockDbSet.Where(f => f.InvReferenceNo == invoiceNo && (f.PosInvoiceType == (posInvoiceType > 0 ? (PosInvoiceType)posInvoiceType : f.PosInvoiceType) && branch.Id == f.PosBranchId)).Select(s => s.CompanyInvNo).FirstOrDefault();
                    PosStock Stock = new PosStock
                    {
                        Id = invId,
                        PosInvoiceType = PosInvoiceType.Cancel,
                        ModifiedBy = erpManager.UserId,
                        ModifideDate = UTCDateTime.BDDateTime(),
                        CompanyInvNo = CompanyInvNo
                    };
                    database.PosStockDbSet.Attach(Stock);
                    database.Entry(Stock).Property(p => p.PosInvoiceType).IsModified = true;
                    database.Entry(Stock).Property(p => p.ModifiedBy).IsModified = true;
                    database.Entry(Stock).Property(p => p.ModifideDate).IsModified = true;

                    return database.SaveChanges();
                }
                return -0001;
            }
        }

        public long GetLastInvoiceNo(int cmnId, int userId)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                return database.PosSalesInvoiceDbSet.Where(w => w.CmnCompanyId == cmnId && w.CreatedBy == userId).OrderByDescending(w => w.InvoiceNumber).Select(s => s.InvoiceNumber).First();
            }
        }

        private void ReCalculatePosSalesInvoice(PosSalesInvoice salesInvoice, ConnectionDatabase dbContext, int cmnId, int salesPriceIncOrExcVat)
        {
            decimal mrpTotal = 0;
            decimal vat = 0;
            decimal sd = 0;
            decimal otherDiscount = 0;
            decimal subTotalAmt = 0;

            foreach (var sp in salesInvoice.PosSalesInvoiceProduct)
            {
                decimal prdVat = 0;
                decimal prdSd = 0;
                //sales product amt calculaion
                var prdInfo = dbContext.PosProductBatchDbSet.Where(f => f.Id == sp.PosProductBatchId).Select(s => new { s.Id, s.PosProduct.Vat, s.SellingRate, s.PurchaseRate, s.PosProduct.Sd, s.PosProduct.IsPriceChangeable }).First();

                if (salesInvoice.PosInvoiceType != PosInvoiceType.StockTransfar)
                {
                    prdVat = prdInfo.Vat;
                    prdSd = prdInfo.Sd;
                }
                decimal unitPrice = salesInvoice.PosInvoiceType == PosInvoiceType.StockTransfar ? (prdInfo.IsPriceChangeable ? sp.Rate : prdInfo.PurchaseRate) : (prdInfo.IsPriceChangeable ? sp.Rate : prdInfo.SellingRate);
                decimal tempMrpTotal = Math.Round(sp.Qty * unitPrice, 2);
                mrpTotal += tempMrpTotal;
                var tmpSchDiscount = (sp.SchDiscount / 100) * tempMrpTotal;
                sp.SchDiscount = tmpSchDiscount;
                decimal vatableAmount = tempMrpTotal - (sp.OtherDiscount + sp.SchDiscount);
                sp.Sd = Math.Round((vatableAmount * prdSd) / 100, 2);
                sd += sp.Sd;
                //including vat
                if (salesPriceIncOrExcVat == 1)
                {
                    sp.Vat = Math.Round((vatableAmount / (prdVat + 100)) * prdVat, 2);
                    subTotalAmt += (tempMrpTotal + sp.Sd) - (sp.SchDiscount + sp.OtherDiscount);
                }
                else
                { //excluding vat
                    sp.Vat = Math.Round((vatableAmount * prdVat) / 100, 2);
                    subTotalAmt += (tempMrpTotal + sp.Sd + sp.Vat) - (sp.SchDiscount + sp.OtherDiscount);
                }
                vat += sp.Vat;
                otherDiscount += sp.OtherDiscount;

            }
            if (otherDiscount > 0)
            {
                salesInvoice.OtherDiscount = otherDiscount;
            }
            salesInvoice.SdTotal = sd;
            salesInvoice.MrpTotal = mrpTotal;
            salesInvoice.VatTotal = vat;
            salesInvoice.TotalAmt = subTotalAmt;
            salesInvoice.PaidableAmount = Math.Round(salesInvoice.TotalAmt, MidpointRounding.AwayFromZero);


            string[] rvTenderType = { "R", "PA", "DA", "CA" };
            //the given below query next time store to the session
            var tenderAddToList = dbContext.PosTenderDbSet.Where(w => rvTenderType.Contains(w.Type) && w.CmnCompanyId == cmnId).Select(s => new { s.Id, s.Type }).ToList();
            decimal sumOfCashOrBankReceipt = salesInvoice.PosSalesInvoiceTender?.Select(s => s.TenderAmount).DefaultIfEmpty(0).Sum() ?? 0;
            List<PosSalesInvoiceTender> invoiceTenderList = new List<PosSalesInvoiceTender>();
            foreach (var t in tenderAddToList)
            {
                PosSalesInvoiceTender invoiceTender = new PosSalesInvoiceTender();
                invoiceTender.PosTenderId = t.Id;
                switch (t.Type)
                {
                    case "R":
                        invoiceTender.TenderAmount = salesInvoice.PaidableAmount - salesInvoice.TotalAmt;
                        break;
                    case "PA":
                        invoiceTender.TenderAmount = salesInvoice.PaidableAmount;
                        break;
                    case "DA":
                        decimal dueAmt = salesInvoice.PaidableAmount - sumOfCashOrBankReceipt;
                        invoiceTender.TenderAmount = dueAmt > 0 ? dueAmt : 0;
                        salesInvoice.DueAmount = invoiceTender.TenderAmount;
                        break;
                    case "CA":
                        invoiceTender.TenderAmount = sumOfCashOrBankReceipt - salesInvoice.PaidableAmount;
                        break;
                }
                if (salesInvoice.PosSalesInvoiceTender != null)
                {
                    salesInvoice.PosSalesInvoiceTender.Add(invoiceTender);
                }
                else
                {
                    invoiceTenderList.Add(invoiceTender);
                }

            }
            if (invoiceTenderList.Any())
            {
                salesInvoice.PosSalesInvoiceTender = invoiceTenderList;
            }


            salesInvoice.ReceivedAmount = sumOfCashOrBankReceipt;
        }

        private dynamic UpdateInvoice(PosSalesInvoice salesInvoice, ErpManager erpManager, ConnectionDatabase database)
        {
            using (var trn = database.Database.BeginTransaction())
            {
                var getSalesInvoice = database.PosSalesInvoiceDbSet.Include("PosSalesInvoiceProduct").Include("PosSalesInvoiceFreeProduct").Include("PosSalesInvoiceTender").FirstOrDefault(w => w.InvoiceNumber == salesInvoice.InvoiceNumber);
                List<PosSalesInvoiceProduct> invoiceProduct = new List<PosSalesInvoiceProduct>();
                List<PosSalesInvoiceFreeProduct> freeProduct = new List<PosSalesInvoiceFreeProduct>();
                List<PosSalesInvoiceTender> invoiceTender = new List<PosSalesInvoiceTender>();
                if (salesInvoice.PosBranchId == null)
                    salesInvoice.PosBranchId = erpManager.UserOffice.First();
                if (getSalesInvoice != null)
                {
                    getSalesInvoice.PosInvoiceType = salesInvoice.PosInvoiceType;
                    getSalesInvoice.Discount = salesInvoice.Discount;
                    getSalesInvoice.OtherDiscount = salesInvoice.OtherDiscount;
                    getSalesInvoice.DueAmount = salesInvoice.DueAmount;
                    getSalesInvoice.MrpTotal = salesInvoice.MrpTotal;
                    getSalesInvoice.PaidableAmount = salesInvoice.PaidableAmount;
                    getSalesInvoice.ReceivedAmount = salesInvoice.ReceivedAmount;
                    getSalesInvoice.SdTotal = salesInvoice.SdTotal;
                    getSalesInvoice.TotalAmt = salesInvoice.TotalAmt;
                    getSalesInvoice.VatTotal = salesInvoice.VatTotal;
                    getSalesInvoice.ModifideDate = UTCDateTime.BDDateTime();
                    getSalesInvoice.ModifiedBy = erpManager.UserId;
                    getSalesInvoice.Notes = salesInvoice.Notes;
                    getSalesInvoice.PosDeliveryManId = salesInvoice.PosDeliveryManId;
                    getSalesInvoice.UserCode = salesInvoice.UserCode;
                    getSalesInvoice.PosCustomerId = salesInvoice.PosCustomerId;
                    getSalesInvoice.PosBranchId = salesInvoice.PosBranchId;
                    getSalesInvoice.PosShiftId = salesInvoice.PosShiftId;
                    //getSalesInvoice.UserId = salesInvoice.UserId;
                    #region sales Product
                    ///new item add in invoice
                    foreach (var product in salesInvoice.PosSalesInvoiceProduct)
                    {
                        //var salesInvBill = salesInvoice.PosSalesInvoiceProduct.FirstOrDefault(w => w.PosProductId == sp.PosProductId && w.PosProductBatchId == sp.PosProductBatchId);
                        var salesInvBill = getSalesInvoice.PosSalesInvoiceProduct.FirstOrDefault(w => w.PosProductId == product.PosProductId && w.PosProductBatchId == product.PosProductBatchId);
                        if (salesInvBill == null)
                        {
                            product.CreatedDate = UTCDateTime.BDDateTime();
                            product.CreatedBy = erpManager.UserId;
                            getSalesInvoice.PosSalesInvoiceProduct.Add(product);
                        }
                    }
                    /////

                    foreach (var gslp in getSalesInvoice.PosSalesInvoiceProduct)
                    {
                        bool isDeletedSalesProduct = true;
                        foreach (var slp in salesInvoice.PosSalesInvoiceProduct)
                        {
                            if (gslp.PosProductBatchId == slp.PosProductBatchId && gslp.PosProductId == slp.PosProductId && gslp.Qty == slp.Qty)
                            {
                                //no changes
                                salesInvoice.PosSalesInvoiceProduct.Remove(slp);
                                isDeletedSalesProduct = false;
                                break;
                            }
                            if (gslp.PosProductBatchId == slp.PosProductBatchId && gslp.PosProductId == slp.PosProductId && gslp.Qty != slp.Qty)
                            {
                                gslp.Qty = slp.Qty;
                                salesInvoice.PosSalesInvoiceProduct.Remove(slp);
                                isDeletedSalesProduct = false;
                                break;
                            }
                        }
                        if (isDeletedSalesProduct)
                        {
                            invoiceProduct.Add(gslp);
                        }
                    }
                    if (salesInvoice.PosSalesInvoiceProduct != null && salesInvoice.PosSalesInvoiceProduct.Any())
                    {
                        foreach (var sip in salesInvoice.PosSalesInvoiceProduct)
                        {
                            sip.CreatedDate = UTCDateTime.BDDateTime();
                            sip.CreatedBy = erpManager.UserId;
                            getSalesInvoice.PosSalesInvoiceProduct.Add(sip);
                        }

                    }

                    #endregion end of sales product

                    #region sales Free Product

                    foreach (var gfp in getSalesInvoice.PosSalesInvoiceFreeProduct)
                    {
                        bool isDeletedSalesFreeProduct = true;
                        if (salesInvoice.PosSalesInvoiceFreeProduct != null)
                        {
                            foreach (var fp in salesInvoice.PosSalesInvoiceFreeProduct)
                            {
                                if (gfp.PosProductBatchId == fp.PosProductBatchId && gfp.PosProductId == fp.PosProductId && gfp.Qty == fp.Qty && gfp.ManualQty == fp.ManualQty)
                                {
                                    //no changes
                                    salesInvoice.PosSalesInvoiceFreeProduct.Remove(fp);
                                    isDeletedSalesFreeProduct = false;
                                    break;
                                }
                                if (gfp.PosProductBatchId == fp.PosProductBatchId && gfp.PosProductId == fp.PosProductId && (gfp.Qty != fp.Qty || gfp.ManualQty != fp.ManualQty))
                                {
                                    gfp.Qty = fp.Qty;
                                    gfp.ManualQty = fp.ManualQty;
                                    salesInvoice.PosSalesInvoiceFreeProduct.Remove(fp);
                                    isDeletedSalesFreeProduct = false;
                                    break;
                                }
                            }
                        }
                        if (isDeletedSalesFreeProduct)
                            freeProduct.Add(gfp);

                    }
                    if (salesInvoice.PosSalesInvoiceFreeProduct != null && salesInvoice.PosSalesInvoiceFreeProduct.Any())
                    {
                        foreach (var fp in salesInvoice.PosSalesInvoiceFreeProduct)
                        {
                            getSalesInvoice.PosSalesInvoiceFreeProduct.Add(fp);
                        }
                    }

                    #endregion end of  sales free product

                    #region sales Pos Tender

                    foreach (var gt in getSalesInvoice.PosSalesInvoiceTender)
                    {
                        bool isDeletedSalesTender = true;
                        foreach (var t in salesInvoice.PosSalesInvoiceTender)
                        {
                            if (gt.PosTenderId == t.PosTenderId && gt.TenderAmount == t.TenderAmount)
                            {
                                //no changes
                                salesInvoice.PosSalesInvoiceTender.Remove(t);
                                isDeletedSalesTender = false;
                                break;
                            }
                            if (gt.PosTenderId == t.PosTenderId && gt.TenderAmount != t.TenderAmount)
                            {
                                gt.TenderAmount = t.TenderAmount;
                                salesInvoice.PosSalesInvoiceTender.Remove(t);
                                isDeletedSalesTender = false;
                                break;
                            }
                        }
                        if (isDeletedSalesTender)
                        {
                            invoiceTender.Add(gt);
                        }
                    }

                    if (salesInvoice.PosSalesInvoiceTender.Any())
                    {
                        foreach (var st in salesInvoice.PosSalesInvoiceTender)
                        {
                            getSalesInvoice.PosSalesInvoiceTender.Add(st);
                        }
                    }

                    #endregion end of pos tender


                }
                int ret = database.SaveChanges();
                foreach (var sp in invoiceProduct)
                {
                    database.Entry(sp).State = EntityState.Deleted;
                }
                foreach (var fp in freeProduct)
                {
                    database.Entry(fp).State = EntityState.Deleted;
                }
                foreach (var t in invoiceTender)
                {
                    database.Entry(t).State = EntityState.Deleted;
                }

                ret += database.SaveChanges();
                trn.Commit();
                if (ret > 0)
                {
                    //successfuly inserted bill
                    var rtr = new { LastInv = salesInvoice.InvoiceNumber, rtrCode = 00006 };
                    return rtr;
                }
            }
            //bill inserted failed
            var rtr1 = new { LastInv = salesInvoice.InvoiceNumber, rtrCode = -00006 };
            return rtr1;
        }

        public dynamic InsertOrUpdateBilling(PosSalesInvoice salesInvoice, ErpManager erpManager)
        {
            foreach (var p in salesInvoice.PosSalesInvoiceProduct)
            {
                p.CreatedBy = erpManager.UserId;
                p.CreatedDate = UTCDateTime.BDDateTime();
            }
                using (ConnectionDatabase database = new ConnectionDatabase())
            {
                long PosShiftId = database.PosShiftDbSet.Where(w => w.UserId == erpManager.UserId && w.EndTime == null).Select(s => s.Id).FirstOrDefault();
                if (PosShiftId == 0)
                {
                    var rtr1 = new { rtrCode = 10000 };
                    return rtr1;
                }
                SecUser userLst = database.UserDbSet.AsNoTracking().FirstOrDefault(f => f.UserCode == salesInvoice.UserCode && f.CmnCompanyId == erpManager.CmnId);
                if (userLst == null)
                {
                    var rtr1 = new { rtrCode = 405 };
                    return rtr1;
                }
                SecUserBranch Userbranch = database.UserBranchDbSet.AsNoTracking().FirstOrDefault(f => f.SecUserId == userLst.Id);
                if (Userbranch == null)
                {
                    var rtr1 = new { rtrCode = 405 };
                    return rtr1;
                }
                PosBranch branch = database.PosBrancheDbSet.AsNoTracking().FirstOrDefault(f => f.Id == Userbranch.PosBranchId);
                if (branch == null || branch.Name != erpManager.BranchName)
                {
                    var rtr1 = new { rtrCode = 405 };
                    return rtr1;
                }
                salesInvoice.UserId = erpManager.UserId;
                salesInvoice.PosShiftId = PosShiftId;
                long customerId = 0;
                bool posCustomer = salesInvoice.PosInvoiceType == PosInvoiceType.StockTransfar ? true : false;
                if (salesInvoice.PosCustomerId > 0 && database.PosCustomerDbSet.Any(a => a.Id == salesInvoice.PosCustomerId && a.IsPosBranchCustomer == posCustomer))
                {
                    customerId = salesInvoice.PosCustomerId;
                }
                else
                {
                    //customer not found
                    var rtr1 = new { LastInv = salesInvoice.InvoiceNumber, rtrCode = -00002 };
                    return rtr1;
                }

                if (salesInvoice.PosBranchId == 0)
                    salesInvoice.PosBranchId = erpManager.UserOffice.First();

                if (!UtilityManager.SalesPriceIncOrExcVat.Contains(erpManager.SalesPriceIncOrExcVat))
                {
                    //did not set including/excluding vat on company table
                    return -00007;
                }
                ReCalculatePosSalesInvoice(salesInvoice, database, erpManager.CmnId, erpManager.SalesPriceIncOrExcVat);

                if (salesInvoice.InvoiceNumber > 0)
                {
                    #region hold invoice

                    if (database.PosSalesInvoiceDbSet.Any(a => a.InvoiceNumber == salesInvoice.InvoiceNumber && a.PosInvoiceType == PosInvoiceType.Hold))
                    {
                        return UpdateInvoice(salesInvoice, erpManager, database);
                    }

                    #endregion end of hold invoice

                    var getSalesInvoice = database.PosSalesInvoiceDbSet.Where(l => l.InvoiceNumber == salesInvoice.InvoiceNumber)
                                            .Include(PosSalesInvoiceProduct => PosSalesInvoiceProduct.PosSalesInvoiceProduct)
                                            .Include(PosSalesInvoiceFreeProduct => PosSalesInvoiceFreeProduct.PosSalesInvoiceFreeProduct)
                                            .Include(PosSalesInvoiceTender => PosSalesInvoiceTender.PosSalesInvoiceTender)
                                            .FirstOrDefault();
                    foreach (var item in getSalesInvoice.PosSalesInvoiceProduct)
                    {
                        item.PosProductBatch = database.PosProductBatchDbSet.Where(b => b.Id == item.PosProductBatchId).FirstOrDefault();
                    }

                    if (getSalesInvoice != null)
                    {
                        PosSalesInvoice salesInvoiceCrdDbt = new PosSalesInvoice();
                        salesInvoiceCrdDbt.UserCode = salesInvoice.UserCode;
                        salesInvoiceCrdDbt.UserId = erpManager.UserId;
                        salesInvoiceCrdDbt.PosShiftId = PosShiftId;

                        salesInvoiceCrdDbt.Notes = salesInvoice.Notes;
                        List<PosSalesInvoiceProduct> invoiceProductCrdDbt = new List<PosSalesInvoiceProduct>();
                        bool isCreditedItem = false;
                        ///new item add in invoice
                        foreach (var product in salesInvoice.PosSalesInvoiceProduct)
                        {
                            //var salesInvBill = salesInvoice.PosSalesInvoiceProduct.FirstOrDefault(w => w.PosProductId == sp.PosProductId && w.PosProductBatchId == sp.PosProductBatchId);
                            var salesInvBill = getSalesInvoice.PosSalesInvoiceProduct.FirstOrDefault(w => w.PosProductId == product.PosProductId && w.PosProductBatchId == product.PosProductBatchId);
                            if (salesInvBill == null)
                            {
                                return UpdateInvoice(salesInvoice, erpManager, database);

                            }
                        }
                        /////
                        foreach (var sp in getSalesInvoice.PosSalesInvoiceProduct)
                        {
                            var salesInvBill = salesInvoice.PosSalesInvoiceProduct.FirstOrDefault(w => w.PosProductId == sp.PosProductId && w.PosProductBatchId == sp.PosProductBatchId);
                            if (salesInvBill != null)
                            {
                                decimal crdBillQty = salesInvBill.Qty - sp.Qty;
                                decimal crdVat = salesInvBill.Vat - sp.Vat;
                                decimal crdSd = salesInvBill.Sd - sp.Sd;
                                decimal crdOrDbtDiscount = salesInvBill.SchDiscount - sp.SchDiscount;
                                decimal crdOtherDiscount = salesInvBill.OtherDiscount - sp.OtherDiscount;
                                if (crdBillQty < 0)
                                {
                                    //product or item  decrease from existing bill
                                    PosSalesInvoiceProduct invoiceProduct = new PosSalesInvoiceProduct();
                                    invoiceProduct.PosProductBatchId = sp.PosProductBatchId;
                                    invoiceProduct.PosProductId = sp.PosProductId;
                                    invoiceProduct.Qty = crdBillQty;
                                    invoiceProduct.Vat = crdVat;
                                    invoiceProduct.SchDiscount = crdOrDbtDiscount;
                                    invoiceProduct.Sd = crdSd;
                                    invoiceProduct.OtherDiscount = crdOtherDiscount;
                                    invoiceProduct.Rate = sp.PosProductBatch.SellingRate;
                                    invoiceProduct.CreatedBy = erpManager.UserId;
                                    invoiceProduct.CreatedDate = UTCDateTime.BDDateTime();
                                    if (sp.TotalReturned!=null){
                                        invoiceProduct.TotalReturned = sp.TotalReturned + (invoiceProduct.Qty*-1);
                                        sp.TotalReturned = invoiceProduct.TotalReturned;

                                        // Mark the entity as modified
                                        database.Entry(sp).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        invoiceProduct.TotalReturned = invoiceProduct.Qty * -1;
                                        sp.TotalReturned = invoiceProduct.TotalReturned;

                                        // Mark the entity as modified
                                        database.Entry(sp).State = EntityState.Modified;
                                    }
                                    
                                    if (invoiceProduct.TotalReturned> sp.Qty)
                                    {
                                        //product or item returned several times
                                        var rtr1 = new { LastInv = salesInvoice.InvoiceNumber, rtrCode = -00020 };
                                        return rtr1;
                                    }
                                    //2025
                                    //cannot return item when invoice created befor 14 days
                                    if (sp.CreatedDate != null)
                                    {
                                        var x = sp.CreatedDate.Value;
                                        TimeSpan diffOfDates = DateTime.Now-x;
                                        if ((int)diffOfDates.TotalDays > 14)
                                        {
                                            var rtr1 = new { LastInv = salesInvoice.InvoiceNumber, rtrCode = -00014 };
                                            return rtr1;
                                        }
                                    }
                                
                                        invoiceProductCrdDbt.Add(invoiceProduct);
                                        isCreditedItem = true;
                                }
                                else if (!isCreditedItem && crdBillQty > 0)
                                {
                                    //product or item are not allow to added
                                    var rtr1 = new { LastInv = salesInvoice.InvoiceNumber, rtrCode = -00003 };
                                    return rtr1;
                                }

                            }
                            else
                            {
                                //product or item  delete from existing bill
                                PosSalesInvoiceProduct invoiceProduct = new PosSalesInvoiceProduct();
                                invoiceProduct.PosProductBatchId = sp.PosProductBatchId;
                                invoiceProduct.PosProductId = sp.PosProductId;
                                invoiceProduct.Qty = -sp.Qty;
                                invoiceProduct.Vat = -sp.Vat;
                                invoiceProduct.SchDiscount = -sp.SchDiscount;
                                invoiceProduct.Sd = -sp.Sd;
                                invoiceProduct.Rate = sp.PosProductBatch.SellingRate;
                                invoiceProduct.OtherDiscount = -sp.OtherDiscount;
                                invoiceProduct.CreatedBy = erpManager.UserId;
                                invoiceProduct.CreatedDate = UTCDateTime.BDDateTime();
                                if (sp.TotalReturned != null)
                                {
                                    invoiceProduct.TotalReturned = invoiceProduct.TotalReturned + sp.Qty;
                                    sp.TotalReturned = invoiceProduct.TotalReturned;

                                    // Mark the entity as modified
                                    database.Entry(sp).State = EntityState.Modified;
                                }
                                else
                                {
                                    invoiceProduct.TotalReturned = sp.Qty;
                                    sp.TotalReturned = invoiceProduct.TotalReturned;

                                    // Mark the entity as modified
                                    database.Entry(sp).State = EntityState.Modified;
                                }
                                if (invoiceProduct.TotalReturned > sp.Qty)
                                {
                                    //product or item returned several times
                                    var rtr1 = new { LastInv = salesInvoice.InvoiceNumber, rtrCode = -00020 };
                                    return rtr1;
                                }
                                //2025
                                //cannot return item when invoice created befor 14 days
                                if (sp.CreatedDate != null)
                                {
                                    var x = sp.CreatedDate.Value;
                                    TimeSpan diffOfDates = DateTime.Now - x;
                                    if ((int)diffOfDates.TotalDays > 14)
                                    {
                                        var rtr1 = new { LastInv = salesInvoice.InvoiceNumber, rtrCode = -00014 };
                                        return rtr1;
                                    }
                                }
                                
                                    invoiceProductCrdDbt.Add(invoiceProduct);
                                    isCreditedItem = true;
                                
                            }
                        }
                        salesInvoiceCrdDbt.PosSalesInvoiceProduct = invoiceProductCrdDbt;

                        if (getSalesInvoice.PosSalesInvoiceFreeProduct != null)
                        {
                            List<PosSalesInvoiceFreeProduct> invoiceFreeProductsCrdD = new List<PosSalesInvoiceFreeProduct>();
                            foreach (var fp in getSalesInvoice.PosSalesInvoiceFreeProduct)
                            {
                                if (salesInvoice.PosSalesInvoiceFreeProduct != null)
                                {
                                    //for free product
                                    var salesInvFreePrd = salesInvoice.PosSalesInvoiceFreeProduct.FirstOrDefault(w => w.PosProductId == fp.PosProductId && w.PosProductBatchId == fp.PosProductBatchId);
                                    if (salesInvFreePrd != null)
                                    {
                                        decimal crdQty = salesInvFreePrd.Qty - fp.Qty;
                                        int crdMQty = salesInvFreePrd.ManualQty - fp.ManualQty;
                                        if (crdQty < 0)
                                        {
                                            //when product or item  decrease from existing bill
                                            PosSalesInvoiceFreeProduct salesInvoiceFreeProduct = new PosSalesInvoiceFreeProduct();
                                            salesInvoiceFreeProduct.PosProductId = fp.PosProductId;
                                            salesInvoiceFreeProduct.ManualQty = crdMQty;
                                            salesInvoiceFreeProduct.Qty = crdQty;
                                            salesInvoiceFreeProduct.PosProductBatchId = fp.PosProductBatchId;
                                            invoiceFreeProductsCrdD.Add(salesInvoiceFreeProduct);
                                        }
                                        else if (!isCreditedItem && crdQty > 0)
                                        {
                                            //Free product or item are not allow to added
                                            var rtr1 = new { LastInv = salesInvoice.InvoiceNumber, rtrCode = -000033 };
                                            return rtr1;
                                        }

                                    }
                                    else
                                    {
                                        //product or item  delete from existing bill
                                        PosSalesInvoiceFreeProduct salesInvoiceFreeProduct = new PosSalesInvoiceFreeProduct();
                                        salesInvoiceFreeProduct.PosProductId = fp.PosProductId;
                                        salesInvoiceFreeProduct.ManualQty = -fp.ManualQty;
                                        salesInvoiceFreeProduct.Qty = -fp.Qty;
                                        salesInvoiceFreeProduct.PosProductBatchId = fp.PosProductBatchId;
                                        invoiceFreeProductsCrdD.Add(salesInvoiceFreeProduct);
                                    }
                                }
                                else
                                {
                                    //product or item  delete from existing bill
                                    PosSalesInvoiceFreeProduct salesInvoiceFreeProduct = new PosSalesInvoiceFreeProduct();
                                    salesInvoiceFreeProduct.PosProductId = fp.PosProductId;
                                    salesInvoiceFreeProduct.ManualQty = -fp.ManualQty;
                                    salesInvoiceFreeProduct.Qty = -fp.Qty;
                                    salesInvoiceFreeProduct.PosProductBatchId = fp.PosProductBatchId;
                                    invoiceFreeProductsCrdD.Add(salesInvoiceFreeProduct);
                                }
                            }
                            salesInvoiceCrdDbt.PosSalesInvoiceFreeProduct = invoiceFreeProductsCrdD;
                        }

                        if (isCreditedItem)
                        {
                            //if any item or product reduce/decrease from existing bill
                            //calculate total
                            salesInvoiceCrdDbt.Discount = salesInvoice.Discount - getSalesInvoice.Discount;
                            salesInvoiceCrdDbt.OtherDiscount = salesInvoice.OtherDiscount - getSalesInvoice.OtherDiscount;
                            salesInvoiceCrdDbt.MrpTotal = salesInvoice.MrpTotal - getSalesInvoice.MrpTotal;
                            salesInvoiceCrdDbt.TotalAmt = salesInvoice.TotalAmt - getSalesInvoice.TotalAmt;
                            salesInvoiceCrdDbt.SdTotal = salesInvoice.SdTotal - getSalesInvoice.SdTotal;
                            salesInvoiceCrdDbt.VatTotal = salesInvoice.VatTotal - getSalesInvoice.VatTotal;
                            salesInvoiceCrdDbt.PaidableAmount = salesInvoice.PaidableAmount - getSalesInvoice.PaidableAmount;
                            salesInvoiceCrdDbt.ReceivedAmount = salesInvoiceCrdDbt.PaidableAmount;
                            salesInvoiceCrdDbt.DueAmount = 0;
                            salesInvoiceCrdDbt.PosInvoiceGlobalFileNumberId = getSalesInvoice.PosInvoiceGlobalFileNumberId;
                            salesInvoiceCrdDbt.InvDate = salesInvoice.InvDate;
                            salesInvoiceCrdDbt.PosInvoiceType = salesInvoice.PosInvoiceType == PosInvoiceType.StockTransfar ? PosInvoiceType.CreditStockTransfer : PosInvoiceType.Credit; //for credit invoice

                            string[] tenderType = { "R", "CR", "PA", "CA", "DA" };
                            bool isSelectedPaymentMethod = false;
                            List<PosSalesInvoiceTender> invoiceTendersCrdDbt = new List<PosSalesInvoiceTender>();
                            var tendersList = database.PosTenderDbSet.AsNoTracking().Where(w => tenderType.Contains(w.Type)).Select(s => new { s.Id, s.Type }).ToList();
                            var paidByTender = salesInvoice.PosSalesInvoiceTender.FirstOrDefault(w => !tendersList.Select(s => s.Id).Contains(w.PosTenderId));
                            if (paidByTender != null)
                            {
                                paidByTender.TenderAmount = salesInvoiceCrdDbt.PaidableAmount;
                                invoiceTendersCrdDbt.Add(paidByTender);
                                isSelectedPaymentMethod = true;
                            }
                            foreach (var tp in tendersList.Where(w => tenderType.Take(3).Contains(w.Type)))
                            {
                                PosSalesInvoiceTender invoiceTender = new PosSalesInvoiceTender();
                                if (tp.Type == "R")
                                {
                                    invoiceTender.PosTenderId = tp.Id;
                                    invoiceTender.TenderAmount = ((salesInvoiceCrdDbt.PaidableAmount) - (salesInvoiceCrdDbt.TotalAmt));
                                    invoiceTendersCrdDbt.Add(invoiceTender);
                                }
                                else if (!isSelectedPaymentMethod && tp.Type == "CR")
                                {
                                    invoiceTender.PosTenderId = tp.Id;
                                    invoiceTender.TenderAmount = salesInvoiceCrdDbt.PaidableAmount;
                                    invoiceTendersCrdDbt.Add(invoiceTender);
                                }
                                else if (tp.Type == "PA")
                                {
                                    invoiceTender.PosTenderId = tp.Id;
                                    invoiceTender.TenderAmount = salesInvoiceCrdDbt.PaidableAmount;
                                    invoiceTendersCrdDbt.Add(invoiceTender);
                                }
                            }
                            salesInvoiceCrdDbt.PosSalesInvoiceTender = invoiceTendersCrdDbt;

                        }

                        salesInvoiceCrdDbt.PosBranchId = erpManager.UserOffice.First();
                        long insertBill = new BillingGateway().InsertBilling(salesInvoiceCrdDbt, database, erpManager, customerId);
                        if (insertBill > 0)
                        {
                            //create credit note
                            var rtr = new { LastInv = insertBill, rtrCode = 00004 };
                            return rtr;
                        }
                        else
                        {
                            //credit note generat failed
                            var rtr = new { LastInv = insertBill, rtrCode = -00004 };
                            return rtr;
                        }
                    }
                    else
                    {
                        //invoice not found
                        var rtr = new { LastInv = salesInvoice.InvoiceNumber, rtrCode = -00005 };
                        return rtr;
                    }
                }
                else
                {
                    //insert bill
                    PosInvoiceGlobalFileNumber globalFileNumber = new PosInvoiceGlobalFileNumber();
                    globalFileNumber.CmnCompanyId = erpManager.CmnId;
                    salesInvoice.PosInvoiceGlobalFileNumber = globalFileNumber;
                    long insertbill = new BillingGateway().InsertBilling(salesInvoice, database, erpManager, customerId);
                    if (insertbill > 0)
                    {
                        //successfuly inserted bill
                        var rtr = new { LastInv = insertbill, rtrCode = 00006 };
                        return rtr;
                    }
                    else
                    {
                        //bill insert failed
                        var rtr = new { LastInv = "", rtrCode = -00006 };
                        return rtr;
                    }
                }
            }
        }

        public dynamic InsertOrUpdateBillingCallCenter(PosSalesInvoice salesInvoice, ErpManager erpManager)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                SecUser userLst = database.UserDbSet.AsNoTracking().FirstOrDefault(f => f.Id == erpManager.UserId);
                if (userLst != null)
                {
                    salesInvoice.UserCode = userLst.UserCode;
                }
                salesInvoice.UserId = erpManager.UserId;
                salesInvoice.PosShiftId = null;
                long customerId = 0;
                bool posCustomer = salesInvoice.PosInvoiceType == PosInvoiceType.StockTransfar ? true : false;
                if (salesInvoice.PosCustomerId > 0 && database.PosCustomerDbSet.Any(a => a.Id == salesInvoice.PosCustomerId && a.IsPosBranchCustomer == posCustomer))
                {
                    customerId = salesInvoice.PosCustomerId;
                }
                else
                {
                    //customer not found
                    var rtr1 = new { LastInv = salesInvoice.InvoiceNumber, rtrCode = -00002 };
                    return rtr1;
                }

                if (salesInvoice.PosBranchId == 0)
                    salesInvoice.PosBranchId = erpManager.UserOffice.First();

                if (!UtilityManager.SalesPriceIncOrExcVat.Contains(erpManager.SalesPriceIncOrExcVat))
                {
                    //did not set including/excluding vat on company table
                    return -00007;
                }
                ReCalculatePosSalesInvoice(salesInvoice, database, erpManager.CmnId, erpManager.SalesPriceIncOrExcVat);

                if (salesInvoice.InvoiceNumber > 0)
                {
                    #region hold invoice

                    if (database.PosSalesInvoiceDbSet.Any(a => a.InvoiceNumber == salesInvoice.InvoiceNumber && a.PosInvoiceType == PosInvoiceType.Hold))
                    {
                        return UpdateInvoice(salesInvoice, erpManager, database);
                    }

                    #endregion end of hold invoice

                    var getSalesInvoice = database.PosSalesInvoiceDbSet.Where(l => l.InvoiceNumber == salesInvoice.InvoiceNumber)
                                            .Include(PosSalesInvoiceProduct => PosSalesInvoiceProduct.PosSalesInvoiceProduct)
                                            .Include(PosSalesInvoiceFreeProduct => PosSalesInvoiceFreeProduct.PosSalesInvoiceFreeProduct)
                                            .Include(PosSalesInvoiceTender => PosSalesInvoiceTender.PosSalesInvoiceTender)
                                            .FirstOrDefault();
                    foreach (var item in getSalesInvoice.PosSalesInvoiceProduct)
                    {
                        item.PosProductBatch = database.PosProductBatchDbSet.Where(b => b.Id == item.PosProductBatchId).FirstOrDefault();
                    }


                    if (getSalesInvoice != null)
                    {
                        PosSalesInvoice salesInvoiceCrdDbt = new PosSalesInvoice();
                        salesInvoiceCrdDbt.UserCode = salesInvoice.UserCode;
                        salesInvoiceCrdDbt.UserId = erpManager.UserId;
                        salesInvoiceCrdDbt.Notes = salesInvoice.Notes;
                        List<PosSalesInvoiceProduct> invoiceProductCrdDbt = new List<PosSalesInvoiceProduct>();
                        bool isCreditedItem = false;
                        foreach (var sp in getSalesInvoice.PosSalesInvoiceProduct)
                        {
                            var salesInvBill = salesInvoice.PosSalesInvoiceProduct.FirstOrDefault(w => w.PosProductId == sp.PosProductId && w.PosProductBatchId == sp.PosProductBatchId);
                            if (salesInvBill != null)
                            {
                                decimal crdBillQty = salesInvBill.Qty - sp.Qty;
                                decimal crdVat = salesInvBill.Vat - sp.Vat;
                                decimal crdSd = salesInvBill.Sd - sp.Sd;
                                decimal crdOrDbtDiscount = salesInvBill.SchDiscount - sp.SchDiscount;
                                decimal crdOtherDiscount = salesInvBill.OtherDiscount - sp.OtherDiscount;
                                if (crdBillQty < 0)
                                {
                                    //product or item  decrease from existing bill
                                    PosSalesInvoiceProduct invoiceProduct = new PosSalesInvoiceProduct();
                                    invoiceProduct.PosProductBatchId = sp.PosProductBatchId;
                                    invoiceProduct.PosProductId = sp.PosProductId;
                                    invoiceProduct.Qty = crdBillQty;
                                    invoiceProduct.Vat = crdVat;
                                    invoiceProduct.SchDiscount = crdOrDbtDiscount;
                                    invoiceProduct.Sd = crdSd;
                                    invoiceProduct.OtherDiscount = crdOtherDiscount;
                                    invoiceProduct.Rate = sp.PosProductBatch.SellingRate;
                                    invoiceProductCrdDbt.Add(invoiceProduct);
                                    isCreditedItem = true;
                                }
                                else if (!isCreditedItem && crdBillQty > 0)
                                {
                                    //product or item are not allow to added
                                    var rtr1 = new { LastInv = salesInvoice.InvoiceNumber, rtrCode = -00003 };
                                    return rtr1;
                                }

                            }
                            else
                            {
                                //product or item  delete from existing bill
                                PosSalesInvoiceProduct invoiceProduct = new PosSalesInvoiceProduct();
                                invoiceProduct.PosProductBatchId = sp.PosProductBatchId;
                                invoiceProduct.PosProductId = sp.PosProductId;
                                invoiceProduct.Qty = -sp.Qty;
                                invoiceProduct.Vat = -sp.Vat;
                                invoiceProduct.SchDiscount = -sp.SchDiscount;
                                invoiceProduct.Sd = -sp.Sd;
                                invoiceProduct.Rate = sp.PosProductBatch.SellingRate;
                                invoiceProduct.OtherDiscount = -sp.OtherDiscount;
                                invoiceProductCrdDbt.Add(invoiceProduct);
                                isCreditedItem = true;
                            }
                        }
                        salesInvoiceCrdDbt.PosSalesInvoiceProduct = invoiceProductCrdDbt;

                        if (getSalesInvoice.PosSalesInvoiceFreeProduct != null)
                        {
                            List<PosSalesInvoiceFreeProduct> invoiceFreeProductsCrdD = new List<PosSalesInvoiceFreeProduct>();
                            foreach (var fp in getSalesInvoice.PosSalesInvoiceFreeProduct)
                            {
                                if (salesInvoice.PosSalesInvoiceFreeProduct != null)
                                {
                                    //for free product
                                    var salesInvFreePrd = salesInvoice.PosSalesInvoiceFreeProduct.FirstOrDefault(w => w.PosProductId == fp.PosProductId && w.PosProductBatchId == fp.PosProductBatchId);
                                    if (salesInvFreePrd != null)
                                    {
                                        decimal crdQty = salesInvFreePrd.Qty - fp.Qty;
                                        int crdMQty = salesInvFreePrd.ManualQty - fp.ManualQty;
                                        if (crdQty < 0)
                                        {
                                            //when product or item  decrease from existing bill
                                            PosSalesInvoiceFreeProduct salesInvoiceFreeProduct = new PosSalesInvoiceFreeProduct();
                                            salesInvoiceFreeProduct.PosProductId = fp.PosProductId;
                                            salesInvoiceFreeProduct.ManualQty = crdMQty;
                                            salesInvoiceFreeProduct.Qty = crdQty;
                                            salesInvoiceFreeProduct.PosProductBatchId = fp.PosProductBatchId;
                                            invoiceFreeProductsCrdD.Add(salesInvoiceFreeProduct);
                                        }
                                        else if (!isCreditedItem && crdQty > 0)
                                        {
                                            //Free product or item are not allow to added
                                            var rtr1 = new { LastInv = salesInvoice.InvoiceNumber, rtrCode = -000033 };
                                            return rtr1;
                                        }

                                    }
                                    else
                                    {
                                        //product or item  delete from existing bill
                                        PosSalesInvoiceFreeProduct salesInvoiceFreeProduct = new PosSalesInvoiceFreeProduct();
                                        salesInvoiceFreeProduct.PosProductId = fp.PosProductId;
                                        salesInvoiceFreeProduct.ManualQty = -fp.ManualQty;
                                        salesInvoiceFreeProduct.Qty = -fp.Qty;
                                        salesInvoiceFreeProduct.PosProductBatchId = fp.PosProductBatchId;
                                        invoiceFreeProductsCrdD.Add(salesInvoiceFreeProduct);
                                    }
                                }
                                else
                                {
                                    //product or item  delete from existing bill
                                    PosSalesInvoiceFreeProduct salesInvoiceFreeProduct = new PosSalesInvoiceFreeProduct();
                                    salesInvoiceFreeProduct.PosProductId = fp.PosProductId;
                                    salesInvoiceFreeProduct.ManualQty = -fp.ManualQty;
                                    salesInvoiceFreeProduct.Qty = -fp.Qty;
                                    salesInvoiceFreeProduct.PosProductBatchId = fp.PosProductBatchId;
                                    invoiceFreeProductsCrdD.Add(salesInvoiceFreeProduct);
                                }
                            }
                            salesInvoiceCrdDbt.PosSalesInvoiceFreeProduct = invoiceFreeProductsCrdD;
                        }

                        if (isCreditedItem)
                        {
                            //if any item or product reduce/decrease from existing bill
                            //calculate total
                            salesInvoiceCrdDbt.Discount = salesInvoice.Discount - getSalesInvoice.Discount;
                            salesInvoiceCrdDbt.OtherDiscount = salesInvoice.OtherDiscount - getSalesInvoice.OtherDiscount;
                            salesInvoiceCrdDbt.MrpTotal = salesInvoice.MrpTotal - getSalesInvoice.MrpTotal;
                            salesInvoiceCrdDbt.TotalAmt = salesInvoice.TotalAmt - getSalesInvoice.TotalAmt;
                            salesInvoiceCrdDbt.SdTotal = salesInvoice.SdTotal - getSalesInvoice.SdTotal;
                            salesInvoiceCrdDbt.VatTotal = salesInvoice.VatTotal - getSalesInvoice.VatTotal;
                            salesInvoiceCrdDbt.PaidableAmount = salesInvoice.PaidableAmount - getSalesInvoice.PaidableAmount;
                            salesInvoiceCrdDbt.ReceivedAmount = salesInvoiceCrdDbt.PaidableAmount;
                            salesInvoiceCrdDbt.DueAmount = 0;
                            salesInvoiceCrdDbt.PosInvoiceGlobalFileNumberId = getSalesInvoice.PosInvoiceGlobalFileNumberId;
                            salesInvoiceCrdDbt.InvDate = salesInvoice.InvDate;
                            salesInvoiceCrdDbt.PosInvoiceType = salesInvoice.PosInvoiceType == PosInvoiceType.StockTransfar ? PosInvoiceType.CreditStockTransfer : PosInvoiceType.Credit; //for credit invoice

                            string[] tenderType = { "R", "CR", "PA", "CA", "DA" };
                            bool isSelectedPaymentMethod = false;
                            List<PosSalesInvoiceTender> invoiceTendersCrdDbt = new List<PosSalesInvoiceTender>();
                            var tendersList = database.PosTenderDbSet.AsNoTracking().Where(w => tenderType.Contains(w.Type)).Select(s => new { s.Id, s.Type }).ToList();
                            var paidByTender = salesInvoice.PosSalesInvoiceTender.FirstOrDefault(w => !tendersList.Select(s => s.Id).Contains(w.PosTenderId));
                            if (paidByTender != null)
                            {
                                paidByTender.TenderAmount = salesInvoiceCrdDbt.PaidableAmount;
                                invoiceTendersCrdDbt.Add(paidByTender);
                                isSelectedPaymentMethod = true;
                            }
                            foreach (var tp in tendersList.Where(w => tenderType.Take(3).Contains(w.Type)))
                            {
                                PosSalesInvoiceTender invoiceTender = new PosSalesInvoiceTender();
                                if (tp.Type == "R")
                                {
                                    invoiceTender.PosTenderId = tp.Id;
                                    invoiceTender.TenderAmount = ((salesInvoiceCrdDbt.PaidableAmount) - (salesInvoiceCrdDbt.TotalAmt));
                                    invoiceTendersCrdDbt.Add(invoiceTender);
                                }
                                else if (!isSelectedPaymentMethod && tp.Type == "CR")
                                {
                                    invoiceTender.PosTenderId = tp.Id;
                                    invoiceTender.TenderAmount = salesInvoiceCrdDbt.PaidableAmount;
                                    invoiceTendersCrdDbt.Add(invoiceTender);
                                }
                                else if (tp.Type == "PA")
                                {
                                    invoiceTender.PosTenderId = tp.Id;
                                    invoiceTender.TenderAmount = salesInvoiceCrdDbt.PaidableAmount;
                                    invoiceTendersCrdDbt.Add(invoiceTender);
                                }
                            }
                            salesInvoiceCrdDbt.PosSalesInvoiceTender = invoiceTendersCrdDbt;

                        }

                        salesInvoiceCrdDbt.PosBranchId = erpManager.UserOffice.First();
                        long insertBill = new BillingGateway().InsertBilling(salesInvoiceCrdDbt, database, erpManager, customerId);
                        if (insertBill > 0)
                        {
                            //create credit note
                            var rtr = new { LastInv = insertBill, rtrCode = 00004 };
                            return rtr;
                        }
                        else
                        {
                            //credit note generat failed
                            var rtr = new { LastInv = insertBill, rtrCode = -00004 };
                            return rtr;
                        }
                    }
                    else
                    {
                        //invoice not found
                        var rtr = new { LastInv = salesInvoice.InvoiceNumber, rtrCode = -00005 };
                        return rtr;
                    }
                }
                else
                {
                    //insert bill
                    PosInvoiceGlobalFileNumber globalFileNumber = new PosInvoiceGlobalFileNumber();
                    globalFileNumber.CmnCompanyId = erpManager.CmnId;
                    salesInvoice.PosInvoiceGlobalFileNumber = globalFileNumber;
                    long insertbill = new BillingGateway().InsertBilling(salesInvoice, database, erpManager, customerId);
                    if (insertbill > 0)
                    {
                        //successfuly inserted bill
                        var rtr = new { LastInv = insertbill, rtrCode = 00006 };
                        return rtr;
                    }
                    else
                    {
                        //bill insert failed
                        var rtr = new { LastInv = "", rtrCode = -00006 };
                        return rtr;
                    }
                }
            }
        }


        public int InsertSalesProductWarrantyIssue(List<PosSalesProductWarrantyIssue> warrantyIssue, ErpManager manager)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                foreach (var wi in warrantyIssue)
                {
                    wi.CmnCompanyId = manager.CmnId;
                    wi.CreatedBy = manager.UserId;
                    wi.CreatedDate = UTCDateTime.BDDateTime();
                }
                database.PosSalesProductWarrantyIssueDbSet.AddRange(warrantyIssue);
                return database.SaveChanges();
            }
        }

        //public int UpdateSalesProductWarrantyIssue(List<PosSalesProductWarrantyIssue> warrantyIssue, ErpManager manager)
        //{
        //    using (ConnectionDatabase database = new ConnectionDatabase())
        //    {
        //        foreach (var wi in warrantyIssue)
        //        {
        //            wi.CmnCompanyId = manager.CmnId;
        //            wi.CreatedBy = manager.UserId;
        //            wi.CreatedDate = UTCDateTime.BDDateTime();
        //        }
        //        database.PosSalesProductWarrantyIssueDbSet.AddRange(warrantyIssue);
        //        return database.SaveChanges();
        //    }
        //}


    }
}