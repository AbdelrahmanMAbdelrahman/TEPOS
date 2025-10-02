using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Controllers.Manager;
using ERP.Controllers.POS.Gateway;
using ERP.Controllers.POS.Manager;
using ERP.CSharpLib;
using ERP.Models;
using ERP.Models.POS;
using ERP.Models.Security.Authorization;
using ERP.Models.VModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;


namespace ERP.Controllers.POS
{
    [HasAuthorization]
    public class PosStockController : BaseController
    {
        ErpManager _erpManager = new ErpManager();

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult PurchaseReceipt()
        {
            return View();
        }

        public ActionResult GetProductInfoForPurchasReceipt()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                StockGateway gateway = new StockGateway();
                rtr = UtilityManager.JsonResultMax(Json(gateway.GetProductInfoForPurchasReceipt(_erpManager.CmnId)), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }
        public ActionResult GetProductInfoForPurchasReceiptByItem(string Parcode, string Name)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                StockGateway gateway = new StockGateway();
                rtr = UtilityManager.JsonResultMax(Json(gateway.GetProductInfoForPurchasReceiptByItem(_erpManager.CmnId,Parcode, Name)), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }
        public ActionResult GetPurchaseReceiptByinvNo(string invReferenceNo)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                StockGateway gateway = new StockGateway();
                var receiptData = gateway.GetPurchaseReceiptByinvNo(invReferenceNo, _erpManager.CmnId, _erpManager.BranchName);
                receiptData=receiptData.Where(e => (e.PosInvoiceType != PosInvoiceType.Cancel) && (e.BranchName== _erpManager.BranchName)).ToList();
                rtr = UtilityManager.JsonResultMax(Json(receiptData), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetInvoiceByType(int invoiceType)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                rtr = UtilityManager.JsonResultMax
                    (Json(new  StockGateway().GetInvoiceByType(invoiceType, _erpManager)), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }
            return rtr;
        }
        public ActionResult DeletePurchaseReceiptById(int invoiceId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;

            try
            {
                StockGateway gateway = new StockGateway();
                bool isDeleted = gateway.DeletePurchaseReceiptById(invoiceId, _erpManager.CmnId);

                if (isDeleted)
                {
                    rtr = UtilityManager.JsonResultMax(
                        Json(new { Success = true, Message = "Invoice deleted successfully." }),
                        JsonRequestBehavior.AllowGet
                    );
                }
                else
                {
                    rtr = UtilityManager.JsonResultMax(
                        Json(new { Success = false, Message = "Invoice not found or couldn't be deleted." }),
                        JsonRequestBehavior.AllowGet
                    );
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetPurchaseReceipt(string companyInvNo)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                StockGateway gateway = new StockGateway();
                rtr = UtilityManager.JsonResultMax(Json(gateway.GetPurchaseReceipt(companyInvNo, _erpManager.CmnId, _erpManager.BranchName)), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetProductDetailsForPurchaseReceipt(string productCode)
        
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                StockGateway gateway = new StockGateway();
                rtr = UtilityManager.JsonResultMax(Json(gateway.GetProductDetailsForPurchaseReceipt(_erpManager.CmnId, productCode)), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult InsertPurchaseReceipt(string vmStock)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                StockManager manager = new StockManager();
                PosStock stock = (PosStock)JsonConvert.DeserializeObject(vmStock, typeof(PosStock));
               // stock.PosInvoiceType = PosInvoiceType.Active;
                rtr = UtilityManager.JsonResultMax(Json(manager.InsertStock(stock, _erpManager)), JsonRequestBehavior.AllowGet);

            }
            catch (DbUpdateException e)
            {
                rtr = _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetBatchByProductCode(string productCode)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
   
           {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    int cmnId = _erpManager.CmnId;
                    var lst = (from l in database.PosProductBatchDbSet
                               orderby l.DateTo descending
                               where l.PosProduct.Code == productCode && l.CmnCompanyId == cmnId
                               select new
                               {
                                   l.Id,
                                   Name = l.BatchName,
                                   l.PurchaseRate,
                                   l.SellingRate,
                                   l.DateTo
                               }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        //Adel 17/8/2024
        public ActionResult DetectBatchId(string BatchObj)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                VmItemBatch batchDetails = (VmItemBatch)JsonConvert.DeserializeObject(BatchObj, typeof(VmItemBatch));
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    int cmnId = _erpManager.CmnId;
                    var lst = (from l in database.PosProductBatchDbSet
                               orderby l.SellingRate descending
                               where l.PosProduct.Code == batchDetails.ProductCode && l.DateTo == batchDetails.DateTo 
                               && l.SellingRate == batchDetails.SellingRate && l.CmnCompanyId == cmnId
                               select new
                               {
                                   l.Id,
                                   l.PosProductId,
                                   Name = l.BatchName,
                                   l.CmnCompany,
                                   l.BarCode,
                                   l.PurchaseRate
                                   ,l.SellingRate
                               }).FirstOrDefault();
                    if (lst == null)
                    {
                        var BatchData = (from l in database.PosProductBatchDbSet
                                         where l.PosProduct.Code == batchDetails.ProductCode && l.CmnCompanyId == cmnId
                                         select new
                                         {
                                             l.PosProductId,
                                             Name = l.BatchName,
                                             l.BarCode
                                         }).FirstOrDefault();

                        //create new Batch
                        PosProductBatch AddedproductBatch = new PosProductBatch();
                        AddedproductBatch.PosProductId = BatchData.PosProductId;
                        // AddedproductBatch.BatchName = BatchData.Name;
                        dynamic data = JObject.Parse(BatchObj);
                        if (data.DateTo != null)
                        {
                            DateTime dto = Convert.ToDateTime(data.DateTo);
                            AddedproductBatch.BatchName = dto.ToString("yyyyMMdd") + "-" + batchDetails.SellingRate;
                        }
                        else
                        {
                            AddedproductBatch.BatchName ="99999999-"+ batchDetails.SellingRate.ToString();

                        }
                        AddedproductBatch.Mrp = batchDetails.PurchaseRate.ToString();
                        AddedproductBatch.CmnCompanyId = _erpManager.CmnId;
                        AddedproductBatch.PurchaseRate = batchDetails.PurchaseRate;
                        AddedproductBatch.SellingRate = batchDetails.SellingRate;
                        AddedproductBatch.Weight = 0;
                        AddedproductBatch.BarCode = batchDetails.ProductCode;
                        AddedproductBatch.CreatedBy = _erpManager.UserId;
                        AddedproductBatch.CreatedDate = UTCDateTime.BDDateTime();
                        AddedproductBatch.DateTo = batchDetails.DateTo;
                        var InsertedBatchData = database.PosProductBatchDbSet.Add(AddedproductBatch);
                        int tmp = database.SaveChanges();
                        return UtilityManager.JsonResultMax(Json(InsertedBatchData.Id), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return UtilityManager.JsonResultMax(Json(lst.Id), JsonRequestBehavior.AllowGet);
                    }

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public PosProductBatch GetPosProductBatchByProductCode(string productCode)
        {

            PosProductBatch rtr = new PosProductBatch();

            return null;
        }
        public ActionResult GetStockTransfarInvoiceItem(long invoiceNo)
        {
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    var invSts = (from inv in database.PosSalesInvoiceDbSet
                                  join cus in database.PosCustomerDbSet on inv.PosCustomerId equals cus.Id
                                  where inv.InvoiceNumber == invoiceNo && inv.PosInvoiceType == PosInvoiceType.StockTransfar && _erpManager.UserOffice.Contains(cus.PosBranchId)
                                  select new { inv.IsReceiveTransferStock }).FirstOrDefault();
                    if (invSts != null)
                    {
                        if (!invSts.IsReceiveTransferStock)
                        {
                            return UtilityManager.JsonResultMax(
                                Json(new
                                {
                                    Status = 1,
                                    Result = new BillingGateway().GetPosBillingPrintData(_erpManager, invoiceNo, 1)
                                }), JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return UtilityManager.JsonResultMax(
                                Json(new
                                {
                                    Status = "InvoiceReceived",
                                    Result = ""
                                }), JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return UtilityManager.JsonResultMax(
                            Json(new
                            {
                                Status = "InvalidInvoice",
                                Result = ""
                            }), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }

        [AllowAnonymous]
        public ActionResult GoToPurchaseReceiptForStockTransfer(long invNo)
        {
            TempData["invoiceNumber"] = invNo;
            return RedirectToAction("PurchaseReceipt", "PosStock");
        }


        #region location transfer

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult LocationTransfer()
        {
            return View();
        }

        public ActionResult GetProductDetailsForLocationTransfer(string productCode, int fromLocation, int officeId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                StockGateway gateway = new StockGateway();
                rtr = UtilityManager.JsonResultMax(Json(gateway.GetProductDetailsForStockTransfer(_erpManager.CmnId, productCode, fromLocation, officeId)), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }
            return rtr;
        }

        public ActionResult GetStockByLocationWise(int batchId, int officeId, int fromLocation)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                rtr = UtilityManager.JsonResultMax(Json(new StockGateway().GetStockByLocationWise(batchId, officeId, fromLocation, _erpManager.CmnId)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }
            return rtr;
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult InsertLocationTransfer(string vmLocationTransfer)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                StockManager manager = new StockManager();
                VmLocationTransfer locationTransfer = (VmLocationTransfer)JsonConvert.DeserializeObject(vmLocationTransfer, typeof(VmLocationTransfer));
                rtr = UtilityManager.JsonResultMax(Json(manager.InsertLocationTransfer(locationTransfer, _erpManager)), JsonRequestBehavior.AllowGet);

            }
            catch (DbUpdateException e)
            {
                rtr = _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }



        #endregion


        #region Stock Adjustment

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult StockAdjustment()
        {
            return View();
        }


        [HasAuthorization(AccessLevel = 2)]
        public ActionResult InsertStockAdjustment(string vmStockAdjustment)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                StockManager manager = new StockManager();
                VmStockAdjustment stockAdjustment = (VmStockAdjustment)JsonConvert.DeserializeObject(vmStockAdjustment, typeof(VmStockAdjustment));
                rtr = UtilityManager.JsonResultMax(Json(manager.InsertStockAdjusrment(stockAdjustment, _erpManager)), JsonRequestBehavior.AllowGet);

            }
            catch (DbUpdateException e)
            {
                rtr = _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        #endregion
        [HasAuthorization(AccessLevel = 1)]
        public ActionResult MakeBranchZeroStock()
        {
            return View();
        }
        [HasAuthorization(AccessLevel = 2)]
        public ActionResult SetBranchZeroStock()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                StockManager manager = new StockManager();
                rtr = UtilityManager.JsonResultMax(Json(manager.MakeBranchZeroStock(_erpManager)), JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException e)
            {
                rtr = _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

    }
}