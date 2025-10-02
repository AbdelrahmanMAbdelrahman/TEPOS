using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP.Controllers.Manager;
using ERP.Controllers.POS.Gateway;
using ERP.Migrations;
using ERP.Models;
using ERP.Models.POS;
using ERP.Models.VModel;
using ERP.CSharpLib;
using System.Data.Entity;
using ERP.Models.Security;

namespace ERP.Controllers.POS.Manager
{
    public class StockManager
    {
        public int InsertStock(PosStock stock, ErpManager manager)
        {
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())

                using (var trn = database.Database.BeginTransaction())
                {
                    {
                        var userBranch = GetUserBranch(database, manager.UserId);

                        PopulateStockDetails(database, stock);


                        stock.Netvalue = CalculateNetValue(stock);


                        stock.CmnCompanyId = manager.CmnId;
                        stock.CreatedBy = manager.UserId;
                        stock.CreatedDate = UTCDateTime.BDDateTime();
                        stock.PosStockTransactionType = 1;//for purchase received/stock transfer
                                                          //stock.PosInvoiceType = PosInvoiceType.Active;
                        var existingStock = GetExistingStock(database, stock, manager.CmnId, userBranch);


                        var type = GetStockInvoiceType(database, stock, manager, userBranch);


                        if (existingStock == null)
                        {

                            database.PosStockDbSet.Add(stock);
                        }
                        else
                        {

                            CompareAndHandleChanges(database, existingStock, stock);


                            UpdateStock(database, stock, existingStock.Id, manager.UserId);
                        }




                        #region for branch transfer stock
                        HandleBranchTransferStock(database, stock);

                        #endregion

                        int rtr = database.SaveChanges();
                        trn.Commit();
                        return rtr;
                    }

                }
            }


            catch (Exception ex)
            {

                throw new InvalidOperationException("Failed to insert or update stock.", ex);
            }
                
            
        }
        private int GetUserBranch(ConnectionDatabase database, int userId)
        {
            return database.UserBranchDbSet
                .Where(w => w.SecUserId == userId)
                .Select(s => s.PosBranchId)
                .FirstOrDefault();
        }

        private void PopulateStockDetails(ConnectionDatabase database, PosStock stock)
        {
            foreach (var detail in stock.PosStockDetail)
            {
                stock.LessDiscunt += detail.Discount;
                stock.TotalTax += detail.PurchaseTax;

                var batch = database.PosProductBatchDbSet
                    .Where(f => f.Id == detail.PosProductBatchId)
                    .Select(s => new { s.PosProductId, s.PurchaseRate, s.BarCode })
                    .First();

                detail.PosProductId = batch.PosProductId;
                stock.Netvalue += detail.Qty * detail.PurchaseRate;
            }
        }
        private decimal CalculateNetValue(PosStock stock)
        {
            return (stock.Netvalue + (stock.OtherCharges ?? 0) + (stock.TotalTax ?? 0)) -
                   ((stock.LessDiscunt ?? 0) + (stock.OtherDiscount ?? 0));
        }
        private PosStock GetExistingStock(ConnectionDatabase database, PosStock stock, long companyId, long userBranch)
        {
            return database.PosStockDbSet
                .Where(e => e.CompanyInvNo == stock.CompanyInvNo &&
                            e.InvReferenceNo == stock.InvReferenceNo &&
                            e.PosStockTransactionType == stock.PosStockTransactionType &&
                            e.CmnCompanyId == companyId &&
                            e.PosBranchId == userBranch)
                .FirstOrDefault();
        }
        private PosInvoiceType? GetStockInvoiceType(ConnectionDatabase database, PosStock stock, ErpManager manager, int userBranch)
        {
            return database.PosStockDbSet
                .Where(e => e.CompanyInvNo == stock.CompanyInvNo &&
                            e.InvReferenceNo == stock.InvReferenceNo &&
                            e.PosStockTransactionType == stock.PosStockTransactionType &&
                            e.CmnCompanyId == manager.CmnId &&
                            e.PosBranchId == userBranch)
                .Select(f => f.PosInvoiceType)
                .FirstOrDefault();
        }

        private void CompareAndHandleChanges(ConnectionDatabase database, PosStock existingStock, PosStock incomingStock)
        {
            var existingDetails = database.PosStockDetailDbSet
                .Where(d => d.PosStockId == existingStock.Id)
                .ToList();

            foreach (var incoming in incomingStock.PosStockDetail)
            {
                var existing = existingDetails.FirstOrDefault(e => e.PosProductId == incoming.PosProductId);

                if (existing != null)
                {
                   
                    existing.Qty = incoming.Qty;
                    existing.PurchaseRate = incoming.PurchaseRate;
                    existing.Discount = incoming.Discount;
                    existing.PurchaseTax = incoming.PurchaseTax;
                }
                else
                {
                   
                    incoming.PosStockId = existingStock.Id;
                    database.PosStockDetailDbSet.Add(incoming);
                }
            }
            var incomingProductIds = incomingStock.PosStockDetail.Select(i => i.PosProductId).ToList();
            var toDelete = existingDetails.Where(e => !incomingProductIds.Contains(e.PosProductId)).ToList();

            foreach (var item in toDelete)
            {
                database.PosStockDetailDbSet.Remove(item);
            }

            //var detailsToRemove = new List<PosStockDetail>();

            //foreach (var existing in existingDetails)
            //{
            //    bool found = false;

            //    foreach (var incoming in incomingStock.PosStockDetail)
            //    {
            //        if (incoming.PosProductId == existing.PosProductId)
            //        {
            //            found = true;
            //            break;
            //        }
            //    }

            //    if (!found)
            //    {
            //        detailsToRemove.Add(existing);
            //    }
            //}

            //foreach (var removed in detailsToRemove)
            //{
            //    database.PosStockDetailDbSet.Remove(removed);
            //}

        }

        private void UpdateStock(ConnectionDatabase database, PosStock stock, long existingId, int userId)
        {
            var existingStock = database.PosStockDbSet.Find(existingId);

            if (existingStock != null)
            {
                existingStock.Id = existingId;
                existingStock.ModifiedBy = userId;
                existingStock.ModifideDate = UTCDateTime.BDDateTime();
                existingStock.PosInvoiceType = stock.PosInvoiceType;

                database.Entry(existingStock).State = EntityState.Modified;


                database.Entry(existingStock).Property(o => o.PosBranchId).IsModified = false;
                database.Entry(existingStock).Property(o => o.CmnCompanyId).IsModified = false;
                database.Entry(existingStock).Property(o => o.NetPayable).IsModified = false;
                database.Entry(existingStock).Property(o => o.Netvalue).IsModified = false;
                database.Entry(existingStock).Property(o => o.CreatedBy).IsModified = false;
                database.Entry(existingStock).Property(o => o.CreatedDate).IsModified = false;
            }
        }
        private void HandleBranchTransferStock(ConnectionDatabase database, PosStock stock)
        {
            long companyInv;
            if (!string.IsNullOrEmpty(stock.CompanyInvNo) && long.TryParse(stock.CompanyInvNo, out  companyInv))

               
            {
                var salesInvoice = database.PosSalesInvoiceDbSet
                    .FirstOrDefault(f => !f.IsReceiveTransferStock &&
                                         f.PosInvoiceType == PosInvoiceType.StockTransfar &&
                                         f.InvoiceNumber == companyInv);

                if (salesInvoice != null)
                {
                    salesInvoice.IsReceiveTransferStock = true;
                }
            }
        }

        public int InsertLocationTransfer(VmLocationTransfer locationTransfer, ErpManager manager)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                if (string.IsNullOrEmpty(locationTransfer.CompanyInvNo))
                    return -1010; //company inv no is required
                using (var trn = database.Database.BeginTransaction())
                {

                    List<PosStockDetail> locationTransferTo = new List<PosStockDetail>();
                    List<PosStockDetail> locationTransferFrom = new List<PosStockDetail>();
                    decimal totalAmt = 0;
                    foreach (var sd in locationTransfer.LocationTransferDetails)
                    {
                        decimal stockQty = (new StockGateway().GetStockByLocationWise(sd.PosProductBatchId,locationTransfer.FromPosBranchId ,locationTransfer.FromLocation, manager.CmnId));
                        if  (sd.Qty>stockQty)
                        {
                            return -1011;
                        }
                        var productInfo = database.PosProductBatchDbSet.Where(w => w.Id == sd.PosProductBatchId).Select(s => new {s.PosProductId, s.PurchaseRate}).First();

                        totalAmt += (sd.Qty*productInfo.PurchaseRate);
                        PosStockDetail stockDetailTo = new PosStockDetail()
                        {
                            Discount = 0,
                            PosProductBatchId = sd.PosProductBatchId,
                            PosProductId = productInfo.PosProductId,
                            PosStockTypeId = locationTransfer.ToTocation,
                            PurchaseTax = 0,
                            Qty = sd.Qty
                        };
                        //add to Location TransferTo
                        locationTransferTo.Add(stockDetailTo);

                        //add to Location TransferFrom
                        PosStockDetail stockDetailForm = new PosStockDetail()
                        {
                            Discount = 0,
                            PosProductBatchId = sd.PosProductBatchId,
                            PosProductId = productInfo.PosProductId,
                            PosStockTypeId = locationTransfer.FromLocation,
                            PurchaseTax = 0,
                            Qty =- sd.Qty
                        };
                        locationTransferFrom.Add(stockDetailForm);

                    }
                    PosStock stockTrnsFrom = new PosStock()
                    {
                        InvReferenceNo = "LocationTransfer From",
                        InvDate = locationTransfer.InvDate,
                        InvReceiveDate = UTCDateTime.BDDate(),
                        CompanyInvNo = locationTransfer.CompanyInvNo,
                        PosBranchId = locationTransfer.FromPosBranchId,
                        PosStockTransactionType = 2, //2 for transfer from /reduce from
                        PosSupplierId = locationTransfer.PosSupplierId,
                        LessDiscunt = 0,
                        TotalTax = 0,
                        Netvalue = -totalAmt,
                        NetPayable = -totalAmt,
                        OtherDiscount = 0,
                        OtherCharges = 0,
                        CmnCompanyId = manager.CmnId,
                        CreatedBy = manager.UserId,
                        CreatedDate = UTCDateTime.BDDateTime(),
                        PosStockDetail = locationTransferFrom,
                        Remarks = locationTransfer.Remarks
                        , PosInvoiceType = PosInvoiceType.StockTransfar
                    };
                    database.PosStockDbSet.Add(stockTrnsFrom);
                    int rtr = database.SaveChanges();

                    PosStock stockTrnsTo = new PosStock()
                    {
                        InvReferenceNo = "LocationTransfer To",
                        InvDate = locationTransfer.InvDate,
                        InvReceiveDate = UTCDateTime.BDDate(),
                        CompanyInvNo = locationTransfer.CompanyInvNo,
                        PosBranchId = locationTransfer.ToPosBranchId,
                        PosStockTransactionType = 3, //3 for ==Transfer to location/Add To 
                        PosSupplierId = locationTransfer.PosSupplierId,
                        LessDiscunt = 0,
                        TotalTax = 0,
                        Netvalue = totalAmt,
                        NetPayable = totalAmt,
                        OtherDiscount = 0,
                        OtherCharges = 0,
                        CmnCompanyId = manager.CmnId,
                        CreatedBy = manager.UserId,
                        CreatedDate = UTCDateTime.BDDateTime(),
                        PosStockDetail = locationTransferTo,
                        Remarks = locationTransfer.Remarks
                        ,PosInvoiceType = PosInvoiceType.StockTransfar

                    };
                    database.PosStockDbSet.Add(stockTrnsTo);
                    rtr += database.SaveChanges();

                    trn.Commit();
                    return rtr;
                }
            }
        }


        public int InsertStockAdjusrment(VmStockAdjustment stocjAdjust, ErpManager manager)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                if (string.IsNullOrEmpty(stocjAdjust.CompanyInvNo))
                    return -1010; //company inv no is required
                using (var trn = database.Database.BeginTransaction())
                {

                    List<PosStockDetail> stockDetails = new List<PosStockDetail>();
                    decimal totalAmt = 0;
                    foreach (var sd in stocjAdjust.StockAdjustmentDetails)
                    {//2025 make stock adjust make 0
                        if (sd.Qty != 0)
                        {
                            var productInfo = database.PosProductBatchDbSet.Where(w => w.Id == sd.PosProductBatchId).Select(s => new {s.PosProductId, s.PurchaseRate}).First();
                            decimal tmpQty = stocjAdjust.TranactionType == 5 ? -sd.Qty : sd.Qty;
                            totalAmt += (tmpQty*productInfo.PurchaseRate);
                            PosStockDetail posStockDetail = new PosStockDetail()
                            {
                                Discount = 0,
                                PosProductBatchId = sd.PosProductBatchId,
                                PosProductId = productInfo.PosProductId,
                                PosStockTypeId = stocjAdjust.Location,
                                PurchaseTax = 0,
                                Qty = tmpQty
                            };
                            stockDetails.Add(posStockDetail);
                        }
                        if (sd.Qty == 0)
                        {
                            var productInfo = database.PosProductBatchDbSet.Where(w => w.Id == sd.PosProductBatchId).Select(s => new { s.PosProductId, s.PurchaseRate }).First();
                            //decimal tmpQty = stocjAdjust.TranactionType == 5 ? -sd.Qty : sd.Qty;
                            decimal tmpQty = sd.StockOnHand;
                            if (tmpQty < 0)
                            {
                                tmpQty = tmpQty * -1;
                            }
                            totalAmt += (tmpQty * productInfo.PurchaseRate);
                            PosStockDetail posStockDetail = new PosStockDetail()
                            {
                                Discount = 0,
                                PosProductBatchId = sd.PosProductBatchId,
                                PosProductId = productInfo.PosProductId,
                                PosStockTypeId = stocjAdjust.Location,
                                PurchaseTax = 0,
                                Qty = tmpQty
                            };
                            stockDetails.Add(posStockDetail);
                        }
                    }

                    PosStock stock = new PosStock()
                    {

                        InvReferenceNo = stocjAdjust.TranactionType == 5 ? "Reduce Stock" : "Add Stock",
                        InvDate = stocjAdjust.InvDate,
                        InvReceiveDate = UTCDateTime.BDDate(),
                        CompanyInvNo = stocjAdjust.CompanyInvNo,
                        PosBranchId = stocjAdjust.PosBranchId,
                        PosStockTransactionType = stocjAdjust.TranactionType,
                        PosSupplierId = stocjAdjust.PosSupplierId,
                        LessDiscunt = 0,////
                        TotalTax = 0,
                        Netvalue = -totalAmt,
                        NetPayable = -totalAmt,
                        OtherDiscount = 0,
                        OtherCharges = 0,
                        CmnCompanyId = manager.CmnId,
                        CreatedBy = manager.UserId,
                        CreatedDate = UTCDateTime.BDDateTime(),
                        PosStockDetail = stockDetails,
                        Remarks = stocjAdjust.Remarks
                        ,
                        PosInvoiceType = PosInvoiceType.Active
                    };
                    database.PosStockDbSet.Add(stock);
                    int rtr = database.SaveChanges();
                    trn.Commit();
                    return rtr;
                }
            }
        }


        public dynamic MakeBranchZeroStock(ErpManager erpManager)
        {
           
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                SecUser userLst = database.UserDbSet.AsNoTracking().FirstOrDefault(f => f.Id == erpManager.UserId);
                var usrBranch = database.UserBranchDbSet.AsNoTracking().Where(w => w.SecUserId == userLst.Id).Select(s => s.PosBranchId).FirstOrDefault();
                VmMakeBranchZeroStock itemLst = (from l in database.Database.SqlQuery<VmMakeBranchZeroStock>("MakeBranchZeroStock @OfficeId='" + usrBranch + "', @UserId='" + erpManager.UserId + "',@CmnId='" + erpManager.CmnId + "'")
                                                 select new VmMakeBranchZeroStock()
                                                 {
                                                     MessageSave = l.MessageSave
                                                 }).FirstOrDefault();
                return itemLst;

            }
        }


    }
}