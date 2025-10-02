using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ERP.Controllers.Manager;
using ERP.Controllers.POS.Gateway;
using ERP.Controllers.POS.Manager;
using ERP.Models;
using ERP.Models.POS;
using ERP.Models.Security.Authorization;
using ERP.Models.VModel;
using ERP.Report.Pos.Xsd.DsBillingTableAdapters;
using ERP.Report.ReportController;
using Newtonsoft.Json;
using Microsoft.Reporting.WebForms;
using ERP.CSharpLib;

namespace ERP.Controllers
{
    [HasAuthorization]
    public class PosController : BaseController
    {
        ErpManager _erpManager = new ErpManager();

        #region Billing

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult Billing(int? id)
        {
            if (_erpManager.UserOffice.Count > 1)
                return RedirectToAction("ErrorRequest", "Home", new {message = "You have no resource permission yet!"});
            ViewBag.Id = id != null ? 1 : 2;
            return View(ViewBag);
        }

        public ActionResult GetStockByBatchWise(long batchId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                rtr = UtilityManager.JsonResultMax(Json(new StockGateway().GetStockByLocationWise(batchId, _erpManager.UserOffice.First(), _erpManager.MainLocationId, _erpManager.CmnId)), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }
            return rtr;
        }

        public ActionResult GetTenderList()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                if (_erpManager.CmnId < 1)
                    return _erpManager.SessionTimeOut;
                rtr = UtilityManager.JsonResultMax(Json(new BillingGateway().GetTenderInfo(_erpManager.CmnId)), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetInvoice(int invoiceNo, int posInvoiceType)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                rtr = UtilityManager.JsonResultMax(posInvoiceType > 0 ?
                    Json(new BillingGateway().GetInvoiceForStockTransfer(invoiceNo, _erpManager))
                    : Json(new BillingGateway().GetInvoice(invoiceNo, _erpManager)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetInvoiceCallCenter(int invoiceNo, int posInvoiceType)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                rtr = UtilityManager.JsonResultMax(posInvoiceType > 0 ?
                    Json(new BillingGateway().GetInvoiceForStockTransfer(invoiceNo, _erpManager))
                    : Json(new BillingGateway().GetInvoiceCallCenter(invoiceNo, _erpManager)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }
        public ActionResult GetBilingItem(string prdCodeOrBarCode, bool isItemForStockTransfer, int PosBranchId = 0)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                rtr = UtilityManager.JsonResultMax(isItemForStockTransfer ?
                    Json(new BillingGateway().GetBillingItemForStockTransfer(_erpManager.CmnId, prdCodeOrBarCode, _erpManager, PosBranchId))
                    : Json(new BillingGateway().GetBillingItem(_erpManager.CmnId, prdCodeOrBarCode, _erpManager, PosBranchId)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetBillingScheme(DateTime date, string billingItem, long customerId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                List<VmBilling> vmBilling = (List<VmBilling>) JsonConvert.DeserializeObject(billingItem, typeof (List<VmBilling>));
                rtr = UtilityManager.JsonResultMax(Json(new SchemeManager().GetFreeAndDiscountItem(date, vmBilling, customerId, _erpManager.UserOffice.First())), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult InsertOrUpdateBilling(string invoice)
         {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                PosSalesInvoice obj = JsonConvert.DeserializeObject<PosSalesInvoice>(invoice);
                rtr = UtilityManager.JsonResultMax(Json(new BillingManager().InsertOrUpdateBilling(obj, _erpManager)), JsonRequestBehavior.AllowGet);
                if(((System.Web.Mvc.JsonResult)rtr).Data.ToString()=="0")
                {
                    rtr = _erpManager.ExceptionDefault(new DbUpdateException());
                }
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
        [HasAuthorization(AccessLevel = 2)]
        public ActionResult InsertOrUpdateBillingCallCenter(string invoice)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                PosSalesInvoice obj = JsonConvert.DeserializeObject<PosSalesInvoice>(invoice);
                rtr = UtilityManager.JsonResultMax(Json(new BillingManager().InsertOrUpdateBillingCallCenter(obj, _erpManager)), JsonRequestBehavior.AllowGet);
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

        [HasAuthorization(AccessLevel = 4)]
        public ActionResult CancelInvoice(long invoiceNo, int posInvoiceType,string UserCode)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                rtr = UtilityManager.JsonResultMax(Json(new BillingManager().CancelInvoice(invoiceNo, posInvoiceType, UserCode, _erpManager)), JsonRequestBehavior.AllowGet);

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

        [HasAuthorization(AccessLevel = 4)]
        public ActionResult CancelPurchaseInvoice(string invoiceNo, int posInvoiceType)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            
            try
            {
                rtr = UtilityManager.JsonResultMax(Json(new BillingManager().CancelPurchaseInvoice(invoiceNo, posInvoiceType, _erpManager)), JsonRequestBehavior.AllowGet);

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


        public ActionResult GenerateBill(int invoiceNo)
        {
            try
            {
                RptBillingProductsTableAdapter billingProductsTableAdapter = new RptBillingProductsTableAdapter();
                RptBillingSalesInvoiceInfoTableAdapter billingSalesInvoiceInfoTableAdapter = new RptBillingSalesInvoiceInfoTableAdapter();
                RptBillingTenderInfoTableAdapter billingTenderInfoTableAdapter = new RptBillingTenderInfoTableAdapter();
                CommandTimeOut.ChangeTimeout(billingProductsTableAdapter, 0);
                CommandTimeOut.ChangeTimeout(billingSalesInvoiceInfoTableAdapter, 0);
                CommandTimeOut.ChangeTimeout(billingTenderInfoTableAdapter, 0);


                LocalReport report = new LocalReport();
                report.ReportPath = Server.MapPath("~/Report/Pos/Rdlc/Billing/RptBilling.rdlc");

                ReportDataSource rdsBilling = new ReportDataSource("Billing");
                rdsBilling.Value = billingProductsTableAdapter.GetData(invoiceNo);
                report.DataSources.Add(rdsBilling);

                ReportDataSource rdsBillingInfo = new ReportDataSource("BillingInfo");
                rdsBillingInfo.Value = billingSalesInvoiceInfoTableAdapter.GetData(invoiceNo);
                report.DataSources.Add(rdsBillingInfo);

                ReportDataSource rdsTender = new ReportDataSource("BillingTender");
                rdsTender.Value = billingTenderInfoTableAdapter.GetData(invoiceNo);
                report.DataSources.Add(rdsTender);
                string roundOff = "0.00", changeAmt = "0.00", paidBy = "";
                foreach (var l in billingTenderInfoTableAdapter.GetData(invoiceNo))
                {
                    if (l.Type == "R")
                    {
                        roundOff = l.TenderAmount.ToString();
                    }
                    else if (l.Type == "CA")
                    {
                        changeAmt = l.TenderAmount.ToString();
                    }
                    else if (l.Type == "CR" || l.Type == "NA")
                    {
                        paidBy += l.Name + ", ";
                    }
                }


                var param = new ReportParameter[12];
                param[0] = new ReportParameter("Company", _erpManager.CompanyName);
                param[1] = new ReportParameter("BranchAddress", _erpManager.BranchAddress);
                param[2] = new ReportParameter("User", _erpManager.UserName);
                param[3] = new ReportParameter("Phone", _erpManager.CompanyPhone);
                param[4] = new ReportParameter("Branch", _erpManager.BranchName);
                param[5] = new ReportParameter("VatRegNo", _erpManager.VatRegNo);
                param[6] = new ReportParameter("RoundOffvalue", roundOff);
                param[7] = new ReportParameter("PaidBy", paidBy.TrimEnd(','));
                param[8] = new ReportParameter("ChangeAmt", changeAmt);
                param[9] = new ReportParameter("FooterText", _erpManager.BillingFooterText);
                param[10] = new ReportParameter("PoweredBy", _erpManager.PoweredBy);
                param[11] = new ReportParameter("TerminalNo", _erpManager.TerminalId);
                report.SetParameters(param);

                string path = Server.MapPath("~/Constants/Billing/" + _erpManager.CmnId + "/" + _erpManager.UserId);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path); //create folder
                }
                //delete all file from path directory
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }

                if (!System.IO.File.Exists(path + "/" + UTCDateTime.BDDate().ToString("ddMMyyyy") + invoiceNo + ".pdf"))
                {
                    Byte[] mybytes = report.Render("PDF"); //for exporting to PDF
                    using (FileStream fs = System.IO.File.Create(path + "/" + UTCDateTime.BDDate().ToString("MMddyyyy") + invoiceNo + ".pdf"))
                    {
                        fs.Write(mybytes, 0, mybytes.Length);
                    }
                }

                string rtrPath = "\\Constants\\Billing\\" + _erpManager.CmnId + "\\" + _erpManager.UserId + "\\" + UTCDateTime.BDDate().ToString("MMddyyyy") + invoiceNo + ".pdf";
                return Json(rtrPath, JsonRequestBehavior.DenyGet);

            }
            catch (Exception ex)
            {
                return Json(0, JsonRequestBehavior.DenyGet);
            }
        }

        public ActionResult GenerateBillHTML(int invoiceNo, int billingType)
        {
            try
            {

                return UtilityManager.JsonResultMax(Json(new BillingGateway().GetPosBillingPrintData(_erpManager, invoiceNo, billingType)), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(0, JsonRequestBehavior.DenyGet);
            }
        }

        public ActionResult PrintStockTransferInvoice()
        {
            return View();
        }


        public ActionResult GetLastInvoiceNo()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                return UtilityManager.JsonResultMax(Json(new BillingManager().GetLastInvoiceNo(_erpManager.CmnId, _erpManager.UserId)), JsonRequestBehavior.DenyGet);
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
                rtr = UtilityManager.JsonResultMax(Json(new BillingGateway().GetInvoiceByType(invoiceType, _erpManager)), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }
            return rtr;
        }
        public ActionResult GetInvoiceByTypeCallCenter(int invoiceType)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                rtr = UtilityManager.JsonResultMax(Json(new BillingGateway().GetInvoiceByTypeCallCenter(invoiceType, _erpManager)), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }
            return rtr;
        }
        public ActionResult GetProductInfo()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
               
                   rtr = UtilityManager.JsonResultMax(Json(new BillingGateway().GetProductInfo(_erpManager)), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }
            return rtr;
        }

        public ActionResult GetProductInfoCallCenter(int PosBranchId,string Parcode, string Name,string ScientificName)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {

                rtr = UtilityManager.JsonResultMax(Json(new BillingGateway().GetProductInfoCallCenter(_erpManager, PosBranchId, Parcode, Name, ScientificName)), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }
            return rtr;
        }

        public ActionResult GetSalesInvoicesForProduct( string Name)
        {
           
            ActionResult rtr;
            try
            {

                rtr = UtilityManager.JsonResultMax(Json(new BillingGateway().GetSalesInvoicesForProduct(Name,_erpManager)), JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }
            return rtr;
        }
        #endregion Billing

        #region Customer Due Collection

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult CustomerDueCollection()
        {
            return View();
        }

        public ActionResult GetCustomerDueInfo(int customerId, int invoiceNumber = 0)
        {
            try
            {
                using (var context = new ConnectionDatabase())
                {
                    if (invoiceNumber != 0)
                    {
                        var dueInfo = context.PosSalesInvoiceDbSet.Where(o => o.InvoiceNumber == invoiceNumber && o.DueAmount > 0 && !o.IsDuePaid && !UtilityManager.PosInvoiceTypeNotInForSalesSide.Contains(o.PosInvoiceType))
                            .Select(o => new
                            {
                                SalesInvoiceId = o.Id,
                                o.InvDate,
                                o.InvoiceNumber,
                                o.DueAmount,
                                TotalCollected = o.PosCustomerDueCollections.Select(c => c.Amount).DefaultIfEmpty(0).Sum()
                            }).ToList();
                        return Json(dueInfo, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var dueInfo = context.PosSalesInvoiceDbSet.Where(o => o.PosCustomerId == customerId && o.DueAmount > 0 && !o.IsDuePaid && !UtilityManager.PosInvoiceTypeNotInForSalesSide.Contains(o.PosInvoiceType))
                            .Select(o => new
                            {
                                SalesInvoiceId = o.Id,
                                o.InvDate,
                                o.InvoiceNumber,
                                o.DueAmount,
                                TotalCollected = o.PosCustomerDueCollections.Select(c => c.Amount).DefaultIfEmpty(0).Sum()
                            }).ToList();
                        return Json(dueInfo, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception exception)
            {
                return Json(0);
            }
        }

        public ActionResult SaveCustomerDueCollection(string customerDueCollections)
        {
            try
            {
                using (var context = new ConnectionDatabase())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        List<CustomerDueCollectionDto> customerDueCollectionsList = JsonConvert.DeserializeObject<List<CustomerDueCollectionDto>>(customerDueCollections);
                        int companyId = _erpManager.CmnId;
                        foreach (var cdc in customerDueCollectionsList)
                        {
                            var posSalesInvoice = context.PosSalesInvoiceDbSet.Find(cdc.PosSalesInvoiceId);
                            if (posSalesInvoice != null)
                            {
                                var totalDue = posSalesInvoice.DueAmount;
                                var totalCollection = context.PosCustomerDueCollectionDbSet.Where(c => c.PosSalesInvoiceId == cdc.PosSalesInvoiceId).Select(c => c.Amount).DefaultIfEmpty(0).Sum();
                                totalCollection += cdc.Amount;
                                if (totalCollection >= totalDue)
                                {
                                    posSalesInvoice.IsDuePaid = true;
                                }

                                PosCustomerDueCollection dueCollection = new PosCustomerDueCollection();
                                dueCollection.PosSalesInvoiceId = cdc.PosSalesInvoiceId;
                                dueCollection.Amount = cdc.Amount;
                                dueCollection.Date = UTCDateTime.BDDateTime();
                                dueCollection.CreatedDate = UTCDateTime.BDDateTime();
                                dueCollection.CmnCompanyId = companyId;

                                context.PosCustomerDueCollectionDbSet.Add(dueCollection);
                            }
                        }
                        int rowAffected = context.SaveChanges();
                        transaction.Commit();

                        if (rowAffected > 0)
                            return Json(200, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            return Json(0);
        }

        #endregion


        #region Warranty Issue

        public ActionResult GetWarrantyProduct(long invoiceNo)
        {
            try
            {
                return UtilityManager.JsonResultMax(Json(new WarrantyManager().GetWarrantyProduct(invoiceNo)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return _erpManager.ExceptionDefault(e);
            }
        }


        public ActionResult InsertOrUpdateWarrantyIssue(List<PosSalesProductWarrantyIssue> salesInvoiceWarranty)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    var lstOfSalesInvoiceProductIdFrmInput = salesInvoiceWarranty.Select(s => s.PosSalesInvoiceProductId).ToList();
                    var getInsertedLst = database.PosSalesProductWarrantyIssueDbSet.Where(w => lstOfSalesInvoiceProductIdFrmInput.Contains(w.PosSalesInvoiceProductId)).ToList();
                    var insertedWarrantyItem = salesInvoiceWarranty.Where(w => !getInsertedLst.Select(s => s.PosSalesInvoiceProductId).ToList().Contains(w.PosSalesInvoiceProductId)).ToList();
                    int inrtr = InsertSalesProductWarrantyIssue(insertedWarrantyItem.Where(w=>w.SerialOrImeiNo!=null).ToList());
                    int uprtr = UpdateSalesProductWarrantyIssue(getInsertedLst, salesInvoiceWarranty, database);

                    if (inrtr > 0 && uprtr > 0)
                        //two transction insert & update success
                        return Json(1, JsonRequestBehavior.DenyGet);
                   
                    if (inrtr > 0)
                        //only insert success
                        return Json(2, JsonRequestBehavior.DenyGet);

                    if (uprtr > 0)
                        //only update success
                        return Json(3, JsonRequestBehavior.DenyGet);
                    //no tranaction
                    return Json(-4, JsonRequestBehavior.DenyGet);
                }
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

        [HasAuthorization(AccessLevel = 4)]
        public int InsertSalesProductWarrantyIssue(List<PosSalesProductWarrantyIssue> salesInvoiceWarranty)
        {
           return new BillingManager().InsertSalesProductWarrantyIssue(salesInvoiceWarranty, _erpManager);
        }

        [HasAuthorization(AccessLevel = 5)]
        public int UpdateSalesProductWarrantyIssue(List<PosSalesProductWarrantyIssue> getWarrantyFromDb, List<PosSalesProductWarrantyIssue> getWarrantyFromInput, ConnectionDatabase database)
        {
            foreach (var wdb in getWarrantyFromDb)
            {
                var wFromInput = getWarrantyFromInput.FirstOrDefault(f => f.PosSalesInvoiceProductId == wdb.PosSalesInvoiceProductId);
                if (wFromInput != null)
                {
                    wdb.SerialOrImeiNo = wFromInput.SerialOrImeiNo;
                    wdb.WarrantyPeriod = wFromInput.WarrantyPeriod;
                    wdb.WarrantyType = wFromInput.WarrantyType;
                    wdb.Remarks = wFromInput.Remarks;
                    wdb.ModifiedBy = _erpManager.UserId;
                    wdb.ModifideDate = UTCDateTime.BDDateTime();
                }
            }
            return database.SaveChanges();
        }


        #endregion

        //BillingCallCenter
        [HasAuthorization(AccessLevel = 1)]
        public ActionResult BillingCallCenter(int? id)
        {
            //if (_erpManager.UserOffice.Count > 1)
            //    return RedirectToAction("ErrorRequest", "Home", new { message = "You have no resource permission yet!" });
            ViewBag.Id = id != null ? 1 : 2;
            return View(ViewBag);
        }


    }
}