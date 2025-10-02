using System.Collections.Generic;
using System.Linq;
using ERP.Controllers.Manager;
using ERP.Models;
using ERP.Models.POS;
using ERP.Models.VModel;
using ERP.Report.Pos.Xsd.DsBillingTableAdapters;
using ERP.Report.ReportController;
using ERP.CSharpLib;
using ERP.Models.Security;
using iTextSharp.text.pdf;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using static iTextSharp.text.pdf.AcroFields;
using System.Reflection;
using System.Data.SqlClient;
using System.ComponentModel.Design;

namespace ERP.Controllers.POS.Gateway
{
    public class BillingGateway
    {
        public IEnumerable<dynamic> GetTenderInfo(int cmnId)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                var lst = database.PosTenderDbSet.Where(w => w.CmnCompanyId == cmnId).OrderByDescending(o => o.Order).Select(s => new
                {
                    s.Name,
                    s.Type,
                    PosTenderId = s.Id,
                    Remarks=""
                }).ToList();
                return lst;
            }

        }

        public dynamic GetBillingItemForStockTransfer(int cmnId, string prdCodeOrBarCode, ErpManager erpManager, int PosBranchId = 0)
        {
            var barcodeBatchWithQty = UtilityManager.BarcodeBatchAndQtySeparator(prdCodeOrBarCode, "#",";");
           
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                SecUser userLst = database.UserDbSet.AsNoTracking().FirstOrDefault(f => f.Id == erpManager.UserId);
                var usrBranch = database.UserBranchDbSet.AsNoTracking().Where(w => w.SecUserId == userLst.Id).Select(s => s.PosBranchId).FirstOrDefault();

                if (PosBranchId == 0)
                {
                    PosBranchId = usrBranch;
                }
                string branchId = string.Join(",", erpManager.UserOffice);
                VmGetBillingaItem itemLst = (from l in database.Database.SqlQuery<VmGetBillingaItem>("GetPosBillingItem @OfficeId='" + branchId + "', @ProductCodeOrBarCode='" + barcodeBatchWithQty.Barcode + "',@CmnId=" + erpManager.CmnId + ",@UnitPriceIsSellingRate=" + false+",@BatchName='"+barcodeBatchWithQty.BatchName+"'")
                    select new VmGetBillingaItem()
                    {
                        PosProductId = l.PosProductId,
                        ProductCode = l.ProductCode,
                        Name = l.Name,
                        Stock = (decimal) 0,
                        PosProductBatchId = l.PosProductBatchId,
                        UnitPrice = l.UnitPrice,
                        PurchaseRate = l.PurchaseRate,
                        Vat = 0,
                        SalesPriceIncOrExcVat = 0,
                        Qty = (decimal) barcodeBatchWithQty.Qty,
                        Discount = (decimal) 0,
                        PosUomGroupId = l.PosUomGroupId,
                        Sd = 0,
                        IsPriceChangeable = false,
                        BatchName = l.BatchName,
                        BatchCount = l.BatchCount
                       ,PosBranchId=l.PosBranchId
                    }).Where(e => e.PosBranchId == PosBranchId).FirstOrDefault();

                if (itemLst != null)
                {
                    itemLst.UomDetails = database.PosVwUomDetailsDbSet.Where(l2 => l2.PosUomGroupId == itemLst.PosUomGroupId).Select(ss => new VmUomDetails {UomCode = ss.UomCode, ConversionFactor = ss.ConversionFactor, IsBaseUom = ss.IsBaseUom}).ToList();
                    itemLst.Stock = database.Database.SqlQuery<VmGetPosCurrentStockByLocationOnly>("GetPosCurrentStockByLocationOnly @OfficeId='" + branchId + "',@Location="+erpManager.MainLocationId+",@BatchId=" + itemLst.PosProductBatchId + ",@CmnId=" + erpManager.CmnId).Sum(s => s.StockQty);
                }

                if (itemLst != null)
                    return itemLst;
                return "";
            }
        }

        public dynamic GetBillingItem(int cmnId, string prdCodeOrBarCode, ErpManager erpManager,int PosBranchId=0)
        {
            var barcodeBatchWithQty = UtilityManager.BarcodeBatchAndQtySeparator(prdCodeOrBarCode, "#",";");
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                SecUser userLst = database.UserDbSet.AsNoTracking().FirstOrDefault(f => f.Id == erpManager.UserId);
                var usrBranch = database.UserBranchDbSet.AsNoTracking().Where(w => w.SecUserId == userLst.Id).Select(s => s.PosBranchId).FirstOrDefault();

                if (PosBranchId == 0)
                {
                    PosBranchId = usrBranch;
                }
               // string branchId = string.Join(",", erpManager.UserOffice);
                VmGetBillingaItem itemLst = (from l in database.Database.SqlQuery<VmGetBillingaItem>("GetPosBillingItem @OfficeId='" + PosBranchId.ToString() + "', @ProductCodeOrBarCode='" + barcodeBatchWithQty.Barcode + "',@CmnId=" + erpManager.CmnId + ",@UnitPriceIsSellingRate=" + true+",@BatchName='"+barcodeBatchWithQty.BatchName+"'")
                    select new VmGetBillingaItem()
                    {
                        PosProductId = l.PosProductId,
                        ProductCode = l.ProductCode,
                        Name = l.Name,
                        Stock = (decimal) 0.00,
                        PosProductBatchId = l.PosProductBatchId,
                        UnitPrice = l.UnitPrice,
                        PurchaseRate = l.PurchaseRate,
                        Vat = l.Vat,
                        SalesPriceIncOrExcVat = erpManager.SalesPriceIncOrExcVat,
                        Qty = (decimal) barcodeBatchWithQty.Qty,
                        Discount = (decimal) 0.00,
                        OtherDiscount =(decimal) 0.00,
                        PosUomGroupId = l.PosUomGroupId,
                        Sd = l.Sd,
                        IsPriceChangeable = l.IsPriceChangeable,
                        BatchName = l.BatchName,
                        BatchCount = l.BatchCount
                        ,PosBranchId=l.PosBranchId
                       }).Where(e=>e.PosBranchId== PosBranchId).FirstOrDefault();


                if (itemLst != null)
                {
                    itemLst.UomDetails = database.PosVwUomDetailsDbSet.Where(l2 => l2.PosUomGroupId == itemLst.PosUomGroupId).Select(ss => new VmUomDetails {UomCode = ss.UomCode, ConversionFactor = ss.ConversionFactor, IsBaseUom = ss.IsBaseUom}).ToList();
                   itemLst.Stock = database.Database.SqlQuery<VmGetPosCurrentStockByLocationOnly>("GetPosCurrentStockByLocationOnly @OfficeId='" + PosBranchId.ToString() + "',@Location="+erpManager.MainLocationId+",@BatchId=" + itemLst.PosProductBatchId + ",@CmnId=" + erpManager.CmnId).Sum(s => s.StockQty);
                }

                if (itemLst != null)
                    return itemLst;
                return "";
            }
        }

        public dynamic GetProductInfo(ErpManager erpManager)
        {

            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                
                int cmnId = erpManager.CmnId;
                List<int> userOffice = erpManager.UserOffice;
                var item = (from l in database.PosVwPurchaseReceiptDbSet
                            where userOffice.Contains(l.PosBranchId) && l.CmnCompanyId == cmnId && l.BranchName== erpManager.BranchName
                            select new
                            {
                                l.ProductCode,
                                l.ProductBarCode,
                                l.ProductName,
                                l.BatchName,
                                l.SellingRate,
                                l.PosProductBatchId,
                                l.Vat,
                                l.Mrp,
                                l.PosBranchId
                               ,l.BranchName
                               ,l.Stock
                               ,l.PosProductScientificName
                            }).Distinct()/*.Where(e=>e.Stock>0)*/.ToList();
                    
                
                //for (int i = 0; i < item.Count; i++)
                //{
                //    var matchingItem = item.FirstOrDefault(s => s.PosProductBatchId== item[i].PosProductBatchId);
                //    var Stock = database.Database.SqlQuery<VmGetPosCurrentStockByLocationOnly>("GetPosCurrentStockByLocationOnly @OfficeId='" + item[i].PosBranchId + "',@Location=" + erpManager.MainLocationId + ",@BatchId=" + item[i].PosProductBatchId + ",@CmnId=" + erpManager.CmnId).Sum(s => s.StockQty);
                    
                //    if (Stock == 0)
                //    {
                //        item.Remove(matchingItem);
                //        continue;
                //    }
                //}
               return item;
            }
        }
        public dynamic GetSalesInvoicesForProduct(string productName, ErpManager erpManager)
        {

            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                var branch = database.PosBrancheDbSet.AsNoTracking().Where(w => w.Name == erpManager.BranchName).FirstOrDefault();

             int companyId=erpManager.CmnId;
                List<VmSalesInvoice> invoices = new List<VmSalesInvoice>();
                var parameters = new[]
        {
            new SqlParameter("@ProductName", productName ),
            new SqlParameter("@PosBranchId", branch.Id),
            new SqlParameter("@CmnCompanyId", companyId)
        };

                invoices = (from i in database.Database.SqlQuery<VmSalesInvoice>("EXEC [dbo].[GetSalesInvoicesForProduct] @ProductName, @PosBranchId, @CmnCompanyId"
                                           , parameters)

                               select new VmSalesInvoice()
                               { 

                                             ReceiptNo = i.ReceiptNo,
                                             Date = i.Date,
                                             Item = i.Item,
                                             Barcode = i.Barcode,
                                             CustomerName = i.CustomerName,
                                             Phone = i.Phone
                                         }).Distinct().ToList();
                return invoices;
            }
        }
    
             


        public dynamic GetProductInfoCallCenter(ErpManager erpManager, int PosBranchId,
            string Parcode, string Name,string ScientificName)
        {
            
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                if (PosBranchId == -1)
                {
                    var branch = database.PosBrancheDbSet.AsNoTracking().Where(w => w.Name == erpManager.BranchName).FirstOrDefault();
                    PosBranchId = branch.Id;
                }

                int cmnId = erpManager.CmnId;
                List<int> userOffice = erpManager.UserOffice;
                //List<VmGetProductWithStock> EmptyItem = new List<VmGetProductWithStock>();
                //foreach (var item in userOffice)
                //{

                List<VmGetProductWithStock> itemLst = new List<VmGetProductWithStock>();
                if (PosBranchId != 0)
                {
                    itemLst = (from l in database.Database.SqlQuery<VmGetProductWithStock>("GetProductWithStock @PosBranchId='" + PosBranchId + "', @Parcode='" + Parcode + "',@Name='" + Name + "',@ScientificName='" + ScientificName + "'")
                                                           select new VmGetProductWithStock()
                                                           {
                                                               ProductCode = l.ProductCode,
                                                               ProductBarCode = l.ProductBarCode,
                                                               ProductName = l.ProductName,
                                                               BatchName = l.BatchName,
                                                               SellingRate = l.SellingRate,
                                                               PosProductBatchId = l.PosProductBatchId,
                                                               //Vat = l.Vat,
                                                               //Mrp = l.Mrp,
                                                               PosBranchId = l.PosBranchId,
                                                               PosProductId = l.PosProductId,
                                                               BranchName = l.BranchName,
                                                               Stock = l.Stock,
                                                               PosProductScientificName = l.PosProductScientificName
                                                               //}).Distinct()/*.Where(e => e.Stock>0)*/.ToList();
                                                           }).Distinct().ToList();
                    //EmptyItem.AddRange(itemLst);
                }
                else
                {
                    itemLst = (from l in database.Database.SqlQuery<VmGetProductWithStock>("GetProductWithStockWithoutBatch @PosBranchId='" + PosBranchId + "', @Parcode='" + Parcode + "',@Name='" + Name + "',@ScientificName='" + ScientificName + "'")
                                                           select new VmGetProductWithStock()
                                                           {
                                                               ProductCode = l.ProductCode,
                                                               ProductName = l.ProductName,
                                                               BatchName = l.BatchName,
                                                              // PosProductBatchId = l.PosProductBatchId,
                                                               //Vat = l.Vat,
                                                               //Mrp = l.Mrp,
                                                               PosBranchId = l.PosBranchId,
                                                               PosProductId = l.PosProductId,
                                                               BranchName = l.BranchName,
                                                               Stock = l.Stock,
                                                               PosProductScientificName = l.PosProductScientificName
                                                               //}).Distinct()/*.Where(e => e.Stock>0)*/.ToList();
                                                           }).Distinct().ToList();
                }
                return itemLst;
            }
                
            
        }

        public dynamic GetInvoice(int invoiceNo, ErpManager erpManager)
        {
            List<PosInvoiceType> posInvType =new List<PosInvoiceType>{PosInvoiceType.StockTransfar,PosInvoiceType.CreditStockTransfer};
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                string branchId = string.Join(",", erpManager.UserOffice);
                int brnId = erpManager.UserOffice.First();
                VmGetInvoice item = (from l in database.PosSalesInvoiceDbSet
                    where l.InvoiceNumber == invoiceNo && !posInvType.Contains(l.PosInvoiceType) && l.PosBranchId== brnId
                    && l.PosInvoiceType != PosInvoiceType.Cancel
                                     select new VmGetInvoice
                    {
                        InvoiceNumber = l.InvoiceNumber,
                        PosInvoiceType = l.PosInvoiceType,
                        Discount = l.Discount,
                        OtherDiscount = l.OtherDiscount,
                        InvDate = l.InvDate,
                        DueAmount = l.DueAmount,
                        VatTotal = l.VatTotal,
                        TotalAmt = l.TotalAmt,
                        MrpTotal = l.MrpTotal,
                        SdTotal = l.SdTotal,
                        CustomerNo = l.PosCustomerId,
                        PosDeliveryManId = l.PosDeliveryManId,
                        PosBranchId=l.PosBranchId,
                        UserCode = l.UserCode,
                        Notes = l.Notes,
                        Remarks = l.Remarks,
                        SalesInviceProduct = l.PosSalesInvoiceProduct.Select(s => new VmSalesInviceProduct
                        {
                            ProductCode = s.PosProduct.Code,
                            Name = s.PosProduct.Name,
                            PosProductId = s.PosProductId,
                            PosProductBatchId = s.PosProductBatchId,
                            Qty = s.Qty,
                            Sd = s.Sd,
                            Vat = s.Vat,
                            VatPar=s.PosProduct.Vat,
                            SdPar = s.PosProduct.Sd,
                            Discount = s.SchDiscount,
                            OtherDiscount = s.OtherDiscount,
                            UnitPrice = s.Rate,
                            PurchaseRate=s.PosProductBatch.PurchaseRate,
                            UomDetails = s.PosProduct.PosUomGroup.PosVwUomDetail.Where(l2 => l2.PosUomGroupId == s.PosProduct.PosUomGroupId).Select(ss => new VmUomDetails {UomCode = ss.UomCode, ConversionFactor = ss.ConversionFactor, IsBaseUom = ss.IsBaseUom}).ToList(),
                            BatchName=s.PosProductBatch.BatchName,
                            Stock = 0,
                            IsPriceChangeable = s.PosProduct.IsPriceChangeable
                        }).ToList(),
                        SalesInvoiceFreeProduct = l.PosSalesInvoiceFreeProduct.Select(s => new VmSalesInvoiceFreeProduct
                        {
                            Code = s.PosProduct.Code,
                            PosProductId = s.PosProductId,
                            PosProductBatchId = s.PosProductBatchId,
                            ManualQty = s.ManualQty,
                            Name = s.PosProduct.Name,
                            Batch = s.PosProductBatch.BatchName,
                            Qty = s.Qty
                        }).ToList(),
                        SalesInvoiceTender = l.PosSalesInvoiceTender.Select(s => new VmSalesInvoiceTender
                        {
                            PosTenderId = s.PosTenderId,
                            TenderAmount = s.TenderAmount,
                            Remarks = s.Remarks
                        }).ToList()
                    }).FirstOrDefault();
                if (item != null)
                {
                    foreach (var sp in item.SalesInviceProduct)
                    {
                        sp.Stock = database.Database.SqlQuery<VmGetPosCurrentStockByLocationOnly>("GetPosCurrentStockByLocationOnly @OfficeId='" + branchId + "',@Location="+erpManager.MainLocationId+",@BatchId=" + sp.PosProductBatchId + ",@CmnId=" + erpManager.CmnId).Sum(s => s.StockQty);
                       
                    }
                    return item;
                }
                return "";
            }
        }
        public dynamic GetInvoiceCallCenter(int invoiceNo, ErpManager erpManager)
        {
            List<PosInvoiceType> posInvType = new List<PosInvoiceType> { PosInvoiceType.StockTransfar, PosInvoiceType.CreditStockTransfer };
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                string branchId = string.Join(",", erpManager.UserOffice);
                int brnId = erpManager.UserOffice.First();
                VmGetInvoice item = (from l in database.PosSalesInvoiceDbSet
                                     where l.InvoiceNumber == invoiceNo && !posInvType.Contains(l.PosInvoiceType)
                                     && l.PosInvoiceType!=PosInvoiceType.Cancel
                                     select new VmGetInvoice
                                     {
                                         InvoiceNumber = l.InvoiceNumber,
                                         PosInvoiceType = l.PosInvoiceType,
                                         Discount = l.Discount,
                                         OtherDiscount = l.OtherDiscount,
                                         InvDate = l.InvDate,
                                         DueAmount = l.DueAmount,
                                         VatTotal = l.VatTotal,
                                         TotalAmt = l.TotalAmt,
                                         MrpTotal = l.MrpTotal,
                                         SdTotal = l.SdTotal,
                                         CustomerNo = l.PosCustomerId,
                                         PosDeliveryManId = l.PosDeliveryManId,
                                         PosBranchId = l.PosBranchId,
                                         //UserCode = l.UserCode,
                                         Notes = l.Notes,
                                         Remarks = l.Remarks,
                                         SalesInviceProduct = l.PosSalesInvoiceProduct.Select(s => new VmSalesInviceProduct
                                         {
                                             ProductCode = s.PosProduct.Code,
                                             Name = s.PosProduct.Name,
                                             PosProductId = s.PosProductId,
                                             PosProductBatchId = s.PosProductBatchId,
                                             Qty = s.Qty,
                                             Sd = s.Sd,
                                             Vat = s.Vat,
                                             VatPar = s.PosProduct.Vat,
                                             SdPar = s.PosProduct.Sd,
                                             Discount = s.SchDiscount,
                                             OtherDiscount = s.OtherDiscount,
                                             UnitPrice = s.Rate,
                                             PurchaseRate = s.PosProductBatch.PurchaseRate,
                                             UomDetails = s.PosProduct.PosUomGroup.PosVwUomDetail.Where(l2 => l2.PosUomGroupId == s.PosProduct.PosUomGroupId).Select(ss => new VmUomDetails { UomCode = ss.UomCode, ConversionFactor = ss.ConversionFactor, IsBaseUom = ss.IsBaseUom }).ToList(),
                                             BatchName = s.PosProductBatch.BatchName,
                                             Stock = 0,
                                             IsPriceChangeable = s.PosProduct.IsPriceChangeable
                                         }).ToList(),
                                         SalesInvoiceFreeProduct = l.PosSalesInvoiceFreeProduct.Select(s => new VmSalesInvoiceFreeProduct
                                         {
                                             Code = s.PosProduct.Code,
                                             PosProductId = s.PosProductId,
                                             PosProductBatchId = s.PosProductBatchId,
                                             ManualQty = s.ManualQty,
                                             Name = s.PosProduct.Name,
                                             Batch = s.PosProductBatch.BatchName,
                                             Qty = s.Qty
                                         }).ToList(),
                                         SalesInvoiceTender = l.PosSalesInvoiceTender.Select(s => new VmSalesInvoiceTender
                                         {
                                             PosTenderId = s.PosTenderId,
                                             TenderAmount = s.TenderAmount,
                                             Remarks = s.Remarks
                                         }).ToList()
                                     }).FirstOrDefault();
                if (item != null)
                {
                    foreach (var sp in item.SalesInviceProduct)
                    {
                        sp.Stock = database.Database.SqlQuery<VmGetPosCurrentStockByLocationOnly>("GetPosCurrentStockByLocationOnly @OfficeId='" + branchId + "',@Location=" + erpManager.MainLocationId + ",@BatchId=" + sp.PosProductBatchId + ",@CmnId=" + erpManager.CmnId).Sum(s => s.StockQty);

                    }
                    return item;
                }
                return "";
            }
        }

        public dynamic GetInvoiceForStockTransfer(int invoiceNo,ErpManager erpManager)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                List<PosInvoiceType> posInvType = new List<PosInvoiceType> {PosInvoiceType.CreditStockTransfer,PosInvoiceType.StockTransfar };
                string branchId = string.Join(",", erpManager.UserOffice);
                VmGetInvoice item = (from l in database.PosSalesInvoiceDbSet
                                     where l.InvoiceNumber == invoiceNo && posInvType.Contains(l.PosInvoiceType)
                                     select new VmGetInvoice
                                     {
                                         InvoiceNumber = l.InvoiceNumber,
                                         PosInvoiceType = l.PosInvoiceType,
                                         Discount = l.Discount,
                                         OtherDiscount = l.OtherDiscount,
                                         InvDate = l.InvDate,
                                         DueAmount = l.DueAmount,
                                         VatTotal = l.VatTotal,
                                         TotalAmt = l.TotalAmt,
                                         MrpTotal = l.MrpTotal,
                                         SdTotal = l.SdTotal,
                                         CustomerNo = l.PosCustomerId,
                                         PosDeliveryManId = l.PosDeliveryManId,
                                         UserCode = l.UserCode,
                                         Notes = l.Notes,
                                         Remarks = l.Remarks,
                                         SalesInviceProduct = l.PosSalesInvoiceProduct.Select(s => new VmSalesInviceProduct
                                         {
                                             ProductCode = s.PosProduct.Code,
                                             Name = s.PosProduct.Name,
                                             PosProductId = s.PosProductId,
                                             PosProductBatchId = s.PosProductBatchId,
                                             Qty = s.Qty,
                                             Sd = s.Sd,
                                             Vat = s.Vat,
                                             VatPar = 0,
                                             SdPar =0,
                                             Discount = s.SchDiscount,
                                             OtherDiscount = s.OtherDiscount,
                                             UnitPrice = s.Rate,
                                             PurchaseRate=0,
                                             UomDetails = s.PosProduct.PosUomGroup.PosVwUomDetail.Where(l2 => l2.PosUomGroupId == s.PosProduct.PosUomGroupId).Select(ss => new VmUomDetails { UomCode = ss.UomCode, ConversionFactor = ss.ConversionFactor, IsBaseUom = ss.IsBaseUom }).ToList(),
                                             BatchName = s.PosProductBatch.BatchName,
                                             Stock = 0,
                                             IsPriceChangeable = s.PosProduct.IsPriceChangeable
                                         }).ToList(),
                                         SalesInvoiceFreeProduct = l.PosSalesInvoiceFreeProduct.Select(s => new VmSalesInvoiceFreeProduct
                                         {
                                             Code = s.PosProduct.Code,
                                             PosProductId = s.PosProductId,
                                             PosProductBatchId = s.PosProductBatchId,
                                             ManualQty = s.ManualQty,
                                             Name = s.PosProduct.Name,
                                             Batch = s.PosProductBatch.BatchName,
                                             Qty = s.Qty
                                         }).ToList(),
                                         SalesInvoiceTender = l.PosSalesInvoiceTender.Select(s => new VmSalesInvoiceTender
                                         {
                                             PosTenderId = s.PosTenderId,
                                             TenderAmount = s.TenderAmount,
                                             Remarks = s.Remarks
                                         }).ToList()
                                     }).FirstOrDefault();
                if (item != null)
                {
                    foreach (var sp in item.SalesInviceProduct)
                    {
                        sp.Stock = database.Database.SqlQuery<VmGetPosCurrentStockByLocationOnly>("GetPosCurrentStockByLocationOnly @OfficeId='" + branchId + "',@Location=" + erpManager.MainLocationId + ",@BatchId=" + sp.PosProductBatchId + ",@CmnId=" + erpManager.CmnId).Sum(s => s.StockQty);
                    }
                    return item;
                }
                return "";
            }
        }


        public long InsertBilling(PosSalesInvoice salesInvoice, ConnectionDatabase database, ErpManager erpManager, long customerId)
        {

            salesInvoice.CmnCompanyId = erpManager.CmnId;
            salesInvoice.CreatedBy = erpManager.UserId;
            salesInvoice.CreatedDate = UTCDateTime.BDDateTime();
            salesInvoice.PosCustomerId = customerId;
            database.PosSalesInvoiceDbSet.Add(salesInvoice);
            if (database.SaveChanges() > 0)
            {
                return salesInvoice.InvoiceNumber;
            }
            return 0;
        }

        public dynamic GetInvoiceByType(int invoiceType, ErpManager erpM)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                int cmnId = erpM.CmnId;
                int usrId = erpM.UserId;
                return database.PosSalesInvoiceDbSet.Where(w => w.PosInvoiceType ==(PosInvoiceType) invoiceType && w.CmnCompanyId == cmnId && /*w.CreatedBy == usrId*/ w.PosBranch.Name==erpM.BranchName && w.PosInvoiceType != PosInvoiceType.Cancel)
                    .Select(s => new
                {
                    Id = s.InvoiceNumber,
                    Name = s.InvoiceNumber
                    
                }).ToList();
            }

        }
        public dynamic GetInvoiceByTypeCallCenter(int invoiceType, ErpManager erpM)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                int cmnId = erpM.CmnId;
                int usrId = erpM.UserId;
                return database.PosSalesInvoiceDbSet.Where(w => w.PosInvoiceType == (PosInvoiceType)invoiceType && w.CmnCompanyId == cmnId && w.PosInvoiceType!=PosInvoiceType.Cancel).Select(s => new
                {
                    Id = s.InvoiceNumber,
                    Name = s.InvoiceNumber
                }).ToList();
            }

        }


        /// <summary>
        /// billingType 1=stock transfer
        /// billingType 2=Billing invoice
        /// </summary>
        /// <param name="erpManager"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="billingType"></param>
        /// <returns></returns>
        public VmPosBillingPrint GetPosBillingPrintData(ErpManager erpManager, long invoiceNo,int billingType)
        {
            
            RptBillingProductsTableAdapter billingProductsTableAdapter = new RptBillingProductsTableAdapter();
            RptBillingSalesInvoiceInfoTableAdapter billingSalesInvoiceInfoTableAdapter = new RptBillingSalesInvoiceInfoTableAdapter();
            RptBillingTenderInfoTableAdapter billingTenderInfoTableAdapter = new RptBillingTenderInfoTableAdapter();
            CommandTimeOut.ChangeTimeout(billingProductsTableAdapter, 0);
            CommandTimeOut.ChangeTimeout(billingSalesInvoiceInfoTableAdapter, 0);
            CommandTimeOut.ChangeTimeout(billingTenderInfoTableAdapter, 0);

            decimal roundOff = 0, changeAmt = 0;
            string paidBy = "";

            VmPosBillingPrint billingData = new VmPosBillingPrint();
            var tenderInfo = billingTenderInfoTableAdapter.GetData(invoiceNo);
            billingData.ProductList = (from l in billingProductsTableAdapter.GetData(invoiceNo)
                select new VmProductList
                {
                    Name = l.Name,
                    Qty = l.Qty,
                    SchDiscount = l.SchDiscount,
                    Sd = l.Sd,
                    UnitPrice =billingType==1? l.PurchaseRate: l.SellingRate,
                    Vat = l.Vat,
                    Code = l.Code,
                    PosProductBatchId = l.PosProductBatchId,
                    PurchaseRate = l.PurchaseRate,
                    Batch = l.Batch,
                    SerialOrImeiNo = l.SerialOrImeiNo,
                    WarrantyPeriod = l.WarrantyPeriod
                }).ToList();
            billingData.Invoice = (from l in billingSalesInvoiceInfoTableAdapter.GetData(invoiceNo)
                select new VmInvoice
                {
                    CustomerName = l.CustomerName,
                    Phone = l.Phone,
                    Address = l.Address,
                    Address2 = l.Address2,
                    Discount = l.Discount,
                    DueAmount = l.DueAmount,
                    InvDate = l.InvDate.ToString("dd-MM-yyyy"),
                    InvoiceNumber = l.InvoiceNumber,
                    MrpTotal = l.MrpTotal,
                    PaidableAmount = l.PaidableAmount,
                    PosInvoiceType = l.PosInvoiceType,
                    ReceivedAmount = l.ReceivedAmount,
                    SdTotal = l.SdTotal,
                    TotalAmt = l.TotalAmt,
                    VatTotal = l.VatTotal,
                    PerProductWiseDiscount = billingData.ProductList.Select(s => s.SchDiscount).DefaultIfEmpty(0).Sum()
                    ,NameOfUserCode = l.NameOfUserCode
                    ,Notes = l.Notes
                    ,DeliveryManName = l.DeliveryManName
        }).First();
            billingData.TenderInfo = (from l in tenderInfo
                select new VmTenderInfo
                {
                    Name = l.Name,
                    TenderAmount = l.TenderAmount,
                    Type = l.Type
                }).ToList();

            foreach (var l in tenderInfo)
            {
                if (l.Type == "R")
                {
                    roundOff = l.TenderAmount;
                }
                else if (l.Type == "CA")
                {
                    changeAmt = l.TenderAmount;
                }
                else if (l.Type == "CR" || l.Type == "NA")
                {
                    paidBy += l.Name + ", ";
                }
            }


            billingData.Company = erpManager.CompanyName;
            billingData.BranchAddress = erpManager.BranchAddress;
            billingData.User = erpManager.UserName;
            billingData.Phone = erpManager.CompanyPhone;
            billingData.Branch = erpManager.BranchName;
            billingData.VatRegNo = erpManager.VatRegNo;
            billingData.RoundOffvalue = roundOff;
            billingData.PaidBy = paidBy.Trim(',');
            billingData.ChangeAmt = changeAmt;
            billingData.FooterText = erpManager.BillingFooterText;
            billingData.PoweredBy = erpManager.PoweredBy;
           
            int officeId = erpManager.UserOffice.First();
            var bilPrintTemplateId=new ConnectionDatabase().PosBillprintTemplateOfBrancheDbSet.Where(w => w.PosBranchId == officeId && w.TemplateType == billingType).Select(s => new {s.PosBillprintTemplateId}).FirstOrDefault();
            if (bilPrintTemplateId != null)
            {
                billingData.BillprintMessage = new VmBillprintMessage()
                {
                    BillPrintTemplateId = bilPrintTemplateId.PosBillprintTemplateId,
                    Status = "1"
                };

            }
            else
            {
                billingData.BillprintMessage = new VmBillprintMessage()
                {
                    BillPrintTemplateId = 3,
                    Status = "-1"
                };
            }
            return billingData;

        }
    }

}
