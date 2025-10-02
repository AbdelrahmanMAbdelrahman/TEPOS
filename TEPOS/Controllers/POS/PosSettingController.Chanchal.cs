using ERP.Controllers.POS.Manager;
using ERP.CSharpLib;
using ERP.Models;
using ERP.Models.POS;
using ERP.Models.Security;
using ERP.Models.Security.Authorization;
using ERP.Models.VModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace ERP.Controllers.POS
{
    [HasAuthorization]
    public partial class PosSettingController : Controller
    {

        #region Branch

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult Branch()
        {
            return View();
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult SaveBranch(PosBranch branch)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {

                rtr = Json(PosSettingManager.SaveBranch(branch, _erpManager));
            }
            catch (DbUpdateException ex)
            {
                rtr = _erpManager.DbUpdateExceptionEntity(ex);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateBranch(PosBranch branch)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    branch.ModifiedBy = _erpManager.UserId;
                    branch.ModifideDate = UTCDateTime.BDDateTime();

                    dbContext.Entry(branch).State = EntityState.Modified;
                    dbContext.Entry(branch).Property(o => o.CmnCompanyId).IsModified = false;
                    dbContext.Entry(branch).Property(o => o.CreatedBy).IsModified = false;
                    dbContext.Entry(branch).Property(o => o.CreatedDate).IsModified = false;

                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
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
        public ActionResult DeleteBranch(int branchId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    PosBranch branchToDelete = dbContext.PosBrancheDbSet.Find(branchId);

                    if (branchToDelete != null) dbContext.PosBrancheDbSet.Remove(branchToDelete);
                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
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

        public ActionResult GetAllBranches()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                int cmnId = _erpManager.CmnId;
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    List<int> bl;
                    var usrBranch = dbContext.UserBranchDbSet.Where(w => w.SecUserId == _erpManager.UserId).Select(s => s.PosBranchId).ToList();
                    if (usrBranch.Any())
                    {
                        bl = usrBranch;
                    }
                    else
                    {
                        bl = dbContext.PosBrancheDbSet.Where(w => w.CmnCompanyId == _erpManager.CmnId).Select(s => s.Id).ToList();
                    }
                    var branches = dbContext.PosBrancheDbSet.Where(o => o.CmnCompanyId == cmnId && bl.Contains(o.Id)).Select(o => new
                    {
                        BranchId = o.Id,
                        o.Name,
                        o.Address,
                        o.Phone,
                        o.Mobile,
                        o.Email,
                        o.Fax,
                        o.Remarks
                    }).ToList();
                    rtr = Json(branches, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }


        #endregion
        [HasAuthorization(AccessLevel = 1)]
        public ActionResult Shift()
        {
            return View();
        }


        [HasAuthorization(AccessLevel = 2)]
        public ActionResult GetCloseShiftData()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                int cmnId = _erpManager.CmnId;
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    decimal OpeningBallance = dbContext.PosShiftDbSet.Where(w => w.UserId == _erpManager.UserId).Where(e => e.EndTime == null).Select(s => s.OpeningBallance).FirstOrDefault();
                    DateTime EndTime = UTCDateTime.BDDateTime();
                    DateTime? StartTime = dbContext.PosShiftDbSet.Where(w => w.UserId == _erpManager.UserId).Where(e => e.EndTime==null).Select(i=>i.StartTime).FirstOrDefault();
                    VmShift Shift = new VmShift();
                    Shift.PosShift.OpeningBallance = OpeningBallance;
                    Shift.PosShift.StartTime = StartTime;
                    Shift.PosShift.EndTime = EndTime;
                    // add branch name
                    //if (Shift.PosShift.PosBranch == null)
                    //{
                    //    Shift.PosShift.PosBranch = new PosBranch();
                    //}

                    //Shift.PosShift.PosBranch.Name = _erpManager.BranchName;

                    PosBranch branch = new PosBranch();
                   
                    branch.Name = _erpManager.BranchName;
                   
                    Shift.PosShift.PosBranch = branch;
                    Shift.UserName= _erpManager.UserName;


                    long PosShiftID = dbContext.PosShiftDbSet.Where(w => w.UserId == _erpManager.UserId && w.EndTime == null).Select(s => s.Id).FirstOrDefault();
                    if (PosShiftID != 0)
                    {
                        //TotalSalesCash
                        var TotalSalesCash = dbContext.PosSalesInvoiceDbSet.Where(o => (o.UserId == _erpManager.UserId || o.ModifiedBy == _erpManager.UserId)
                && o.PosInvoiceType != Controllers.Manager.PosInvoiceType.Hold
                && o.PosInvoiceType != Controllers.Manager.PosInvoiceType.Cancel
                && ((o.CreatedDate >= StartTime && o.CreatedDate <= EndTime)
                    || (o.ModifideDate >= StartTime && o.ModifideDate <= EndTime))).SelectMany(e => e.PosSalesInvoiceTender).Where(i => i.PosTenderId == 4);
                        if (TotalSalesCash.Count() != 0)
                        {
                            decimal? TotalSalesCashSum = TotalSalesCash.Sum(x => x.TenderAmount);
                            Shift.PosShift.TotalSalesCash = TotalSalesCashSum;
                        }
                        //cancel invoice not sale in shift
                        //TotalSalesCash
                        var TotalSalesCashCancel = dbContext.PosSalesInvoiceDbSet.Where(o => (o.ModifiedBy == _erpManager.UserId)
                   && o.PosInvoiceType == Controllers.Manager.PosInvoiceType.Cancel
                    && (o.ModifideDate >= StartTime && o.ModifideDate <= EndTime)).SelectMany(e => e.PosSalesInvoiceTender).Where(i => i.PosTenderId == 4);
                        if (TotalSalesCashCancel.Count() != 0)
                        {
                            decimal? TotalSalesCashSumCancel = TotalSalesCashCancel.Sum(x => x.TenderAmount);
                            Shift.PosShift.TotalSalesCashCancel = TotalSalesCashSumCancel;
                        }
                        //TotalSalesVisa
                        var TotalSalesVisa = dbContext.PosSalesInvoiceDbSet.Where(o => (o.UserId == _erpManager.UserId || o.ModifiedBy == _erpManager.UserId)
                    && o.PosInvoiceType != Controllers.Manager.PosInvoiceType.Hold
                    && o.PosInvoiceType != Controllers.Manager.PosInvoiceType.Cancel
                    && ((o.CreatedDate >= StartTime && o.CreatedDate <= EndTime)
                    || (o.ModifideDate >= StartTime && o.ModifideDate <= EndTime))).SelectMany(e=>e.PosSalesInvoiceTender).Where(i=>i.PosTenderId==22);
                        if(TotalSalesVisa.Count()!=0)
                        {
                            decimal? TotalSalesVisaSum = TotalSalesVisa.Sum(x => x.TenderAmount);
                            Shift.PosShift.TotalSalesVisa = TotalSalesVisaSum;
                        }
                        //cancel invoice not sale in shift
                        //TotalSalesVisa
                        var TotalSalesVisaCancel = dbContext.PosSalesInvoiceDbSet.Where(o => (o.ModifiedBy == _erpManager.UserId)
                   && o.PosInvoiceType == Controllers.Manager.PosInvoiceType.Cancel
                    && (o.ModifideDate >= StartTime && o.ModifideDate <= EndTime)).SelectMany(e => e.PosSalesInvoiceTender).Where(i => i.PosTenderId == 22);
                        if (TotalSalesVisaCancel.Count() != 0)
                        {
                            decimal? TotalSalesVisaSumCancel = TotalSalesVisaCancel.Sum(x => x.TenderAmount);
                            Shift.PosShift.TotalSalesVisaCancel = TotalSalesVisaSumCancel;
                        }
                        

                        //TotalSalesBank
                        var TotalSalesBank = dbContext.PosSalesInvoiceDbSet.Where(o => (o.UserId == _erpManager.UserId || o.ModifiedBy == _erpManager.UserId)
                    && o.PosInvoiceType != Controllers.Manager.PosInvoiceType.Hold
                    && o.PosInvoiceType != Controllers.Manager.PosInvoiceType.Cancel
                    && ((o.CreatedDate >= StartTime && o.CreatedDate <= EndTime)
                    || (o.ModifideDate >= StartTime && o.ModifideDate <= EndTime))).SelectMany(e => e.PosSalesInvoiceTender).Where(i => i.PosTenderId == 8);
                        if (TotalSalesBank.Count() != 0)
                        {
                            decimal? TotalSalesBankSum = TotalSalesBank.Sum(x => x.TenderAmount);
                        }
                        //cancel invoice not sale in shift
                        //TotalSalesBank
                        var TotalSalesBankCancel = dbContext.PosSalesInvoiceDbSet.Where(o => (o.ModifiedBy == _erpManager.UserId)
                   && o.PosInvoiceType == Controllers.Manager.PosInvoiceType.Cancel
                    && (o.ModifideDate >= StartTime && o.ModifideDate <= EndTime)).SelectMany(e => e.PosSalesInvoiceTender).Where(i => i.PosTenderId == 8);
                        if (TotalSalesBankCancel.Count() != 0)
                        {
                            decimal? TotalSalesBankSumCancel = TotalSalesBankCancel.Sum(x => x.TenderAmount);
                            Shift.PosShift.TotalSalesBankCancel = TotalSalesBankSumCancel;

                        }
                        //TotalSales
                        var TotalSales = dbContext.PosSalesInvoiceDbSet.Where(o => (o.UserId == _erpManager.UserId || o.ModifiedBy == _erpManager.UserId)
                     && o.PosInvoiceType != Controllers.Manager.PosInvoiceType.Hold
                     && o.PosInvoiceType != Controllers.Manager.PosInvoiceType.Cancel
                     && ((o.CreatedDate >= StartTime && o.CreatedDate <= EndTime)
                        || (o.ModifideDate >= StartTime && o.ModifideDate <= EndTime)));
                        if (TotalSales.Count() != 0)
                        {
                            decimal? TotalSalesSum = TotalSales.Sum(x => x.TotalAmt);
                            Shift.PosShift.TotalSales = TotalSalesSum;
                        }
                        //cancel invoice not sale in shift
                        //TotalSales
                        var TotalSalesCancel = dbContext.PosSalesInvoiceDbSet.Where(o => (o.ModifiedBy == _erpManager.UserId)
                   && o.PosInvoiceType == Controllers.Manager.PosInvoiceType.Cancel
                    && (o.ModifideDate >= StartTime && o.ModifideDate <= EndTime));
                        if (TotalSalesCancel.Count() != 0)
                        {
                            decimal? TotalSalesSumCancel = TotalSalesCancel.Sum(x => x.TotalAmt);
                            Shift.PosShift.TotalSalesCancel = TotalSalesSumCancel;
                        }

                        if (Shift.PosShift.TotalSalesBank==null && Shift.PosShift.TotalSalesCash == null && Shift.PosShift.TotalSalesVisa == null && TotalSales==null)
                        {
                            rtr = Json(new { StartTime = StartTime, EndTime= EndTime, OpeningBallance = Shift.PosShift.OpeningBallance , error = "No Invoices Is Created in Shift" }, JsonRequestBehavior.AllowGet);
                            return rtr;
                        }
                        Shift.PosShift.Id = PosShiftID;

                        //1-1-2025
                        
                            dbContext.Entry(Shift.PosShift).State = EntityState.Modified;
                            dbContext.Entry(Shift.PosShift).Property(o => o.StartTime).IsModified = false;
                            dbContext.Entry(Shift.PosShift).Property(o => o.EndTime).IsModified = false;
                            dbContext.Entry(Shift.PosShift).Property(o => o.UserId).IsModified = false;
                            dbContext.Entry(Shift.PosShift).Property(o => o.OpeningBallance).IsModified = false;
                            dbContext.Entry(Shift.PosShift).Property(o => o.TerminalId).IsModified = false;
                            dbContext.Entry(Shift.PosShift).Property(o => o.PosBranchId).IsModified = false;
                            dbContext.Entry(Shift.PosShift).Property(o => o.CmnCompanyId).IsModified = false;
                            dbContext.Entry(Shift.PosShift).Property(o => o.CreatedBy).IsModified = false;
                            dbContext.Entry(Shift.PosShift).Property(o => o.CreatedDate).IsModified = false;

                            int feedback = dbContext.SaveChanges();

                            ///

                            rtr = Json(Shift, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        rtr = Json(new { error = "you not have shift To close" }, JsonRequestBehavior.AllowGet);
                        return rtr;
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("System.Decimal"))
                {
                    rtr = Json(new { error = "Must Enter Decimal Value" }, JsonRequestBehavior.AllowGet);
                    return rtr;
                }
                 rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;

        }
         [HasAuthorization(AccessLevel = 2)]
        public ActionResult OpenShift() { 

            VmShift Shift=new VmShift();
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                int cmnId = _erpManager.CmnId;

                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    int PosBranchId = dbContext.PosBrancheDbSet.Where(w => w.Name == _erpManager.BranchName).Select(s => s.Id).First();
                    Shift.PosShift.CmnCompanyId = cmnId;
                    Shift.PosShift.CreatedBy = _erpManager.UserId;
                    Shift.PosShift.UserId = _erpManager.UserId;
                    Shift.PosShift.CreatedDate = UTCDateTime.BDDateTime();
                    Shift.PosShift.StartTime = UTCDateTime.BDDateTime();
                    Shift.PosShift.PosBranchId = PosBranchId;
                    decimal RemainingCash = dbContext.PosShiftDbSet.Where(w => w.UserId == _erpManager.UserId).OrderByDescending(e=>e.EndTime).Select(s => s.RemainingCash).FirstOrDefault();
                    Shift.PosShift.OpeningBallance = RemainingCash;
                    long EndDateNULL = dbContext.PosShiftDbSet.Where(w => w.UserId == _erpManager.UserId && w.EndTime == null).Select(e=>e.Id).FirstOrDefault();
                    if(EndDateNULL != 0)
                    {
                        rtr = Json(new { error = "you have shift not close" }, JsonRequestBehavior.AllowGet);                       
                        return rtr;
                    }
                    dbContext.PosShiftDbSet.Add(Shift.PosShift);
                    dbContext.SaveChanges();
                    rtr = Json(new { StartTime = UTCDateTime.BDDateTime().ToString(), OpeningBallance = Shift.PosShift.OpeningBallance }, JsonRequestBehavior.AllowGet);
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
        public ActionResult CloseShift(string Shift)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                // am i need to include objects 
                PosShift obj = JsonConvert.DeserializeObject<PosShift>(Shift);
                int cmnId = _erpManager.CmnId;

                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    var HoldInvoices = dbContext.PosSalesInvoiceDbSet.Where(o => o.UserId == _erpManager.UserId
                      && o.PosInvoiceType == Controllers.Manager.PosInvoiceType.Hold);

                    if (HoldInvoices.Count() != 0)
                    {
                        rtr = Json(new { error = "You Can not Close Shift there are Hold Invoices" }, JsonRequestBehavior.AllowGet);
                        return rtr;
                    }
                   

                   long id = dbContext.PosShiftDbSet.Where(w => w.UserId == _erpManager.UserId && w.EndTime == null).Select(s => s.Id).FirstOrDefault();
                    if (id != 0)
                    {
                        DateTime? StartTimeFromDB = dbContext.PosShiftDbSet.Where(w => w.UserId == _erpManager.UserId).Where(e => e.EndTime == null).Select(i => i.StartTime).FirstOrDefault();

                        obj.Id = id;
                        obj.ModifiedBy = _erpManager.UserId;
                        // erb has branch name
                     //   obj.PosBranch.Name = _erpManager.BranchName;
                        obj.ModifideDate = UTCDateTime.BDDateTime();
                        DateTime EndTime = UTCDateTime.BDDateTime();
                        obj.EndTime = EndTime;
                        dbContext.Entry(obj).State = EntityState.Modified;
                        dbContext.Entry(obj).Property(o => o.StartTime).IsModified = false;
                        dbContext.Entry(obj).Property(o => o.UserId).IsModified = false;
                        dbContext.Entry(obj).Property(o => o.OpeningBallance).IsModified = false;
                        dbContext.Entry(obj).Property(o => o.TerminalId).IsModified = false;
                        dbContext.Entry(obj).Property(o => o.PosBranchId).IsModified = false;
                        dbContext.Entry(obj).Property(o => o.CmnCompanyId).IsModified = false;
                        dbContext.Entry(obj).Property(o => o.CreatedBy).IsModified = false;
                        dbContext.Entry(obj).Property(o => o.CreatedDate).IsModified = false;

                        int feedback = dbContext.SaveChanges();
                        var shiftAfterSave = dbContext.PosShiftDbSet.SingleOrDefault(w => w.Id == id);
                        shiftAfterSave.StartTime = obj.StartTime;
                        shiftAfterSave.OpeningBallance = obj.OpeningBallance;

                        rtr = Json(shiftAfterSave, JsonRequestBehavior.AllowGet);
                       // rtr = Json(new { StartTime = StartTimeFromDB.ToString(),EndTime = UTCDateTime.BDDateTime().ToString() }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        rtr = Json(new { error = "you not have shift To close" }, JsonRequestBehavior.AllowGet);
                        return rtr;
                    }
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

        #region Customer

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult Customer()
        {
            return View();
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult SaveCustomer(VmCustomer customer)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                int cmnId = _erpManager.CmnId;
                
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    int PosBranchId = dbContext.PosBrancheDbSet.Where(w => w.Name == _erpManager.BranchName).Select(s => s.Id).First();
                    customer.PosCustomer.IsDefaultPosCustomer = false;
                    customer.PosCustomer.CmnCompanyId = cmnId;
                    customer.PosCustomer.CreatedBy = _erpManager.UserId;
                    customer.PosCustomer.CreatedDate = UTCDateTime.BDDateTime();
                    
                    customer.PosCustomer.PosBranchId = PosBranchId;
                    dbContext.PosCustomerDbSet.Add(customer.PosCustomer);
                    dbContext.SaveChanges();
                    rtr = Json(new {Id= customer.PosCustomer.Id, CustomerNo=customer.PosCustomer.CustomerNo}, JsonRequestBehavior.AllowGet);
                }
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException.InnerException.Message.Contains("UK_PosCustomer_Phone_CmnCompanyId"))
                {
                    return Json("PhoneNoExists",JsonRequestBehavior.DenyGet);
                }
                rtr = _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult SaveCustomerCallCenter(VmCustomer customer, int PosBranchId=0)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                int cmnId = _erpManager.CmnId;
                if(PosBranchId!=0)
                {
                    customer.PosCustomer.PosBranchId=PosBranchId;
                }
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    customer.PosCustomer.IsDefaultPosCustomer = false;
                    customer.PosCustomer.CmnCompanyId = cmnId;
                    customer.PosCustomer.CreatedBy = _erpManager.UserId;
                    customer.PosCustomer.CreatedDate = UTCDateTime.BDDateTime();

                    dbContext.PosCustomerDbSet.Add(customer.PosCustomer);
                    dbContext.SaveChanges();
                    rtr = Json(new { Id = customer.PosCustomer.Id, CustomerNo = customer.PosCustomer.CustomerNo }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException.InnerException.Message.Contains("UK_PosCustomer_Phone_CmnCompanyId"))
                {
                    return Json("PhoneNoExists", JsonRequestBehavior.DenyGet);
                }
                rtr = _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetCustomers()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    var data = (from cus in dbContext.PosCustomerDbSet
                                join cls in dbContext.PosCustomerClassDbSet on cus.PosCustomerClassId equals cls.Id
                                join rgn in dbContext.PosRegionDbSet on cus.PosRegionId equals rgn.Id
                                join city in dbContext.PosCityOrNearestZoneDbSet on cus.PosCityOrNearestZoneId equals city.Id
                                join brn in dbContext.PosBrancheDbSet on cus.PosBranchId equals brn.Id
                                select new
                                {
                                    cus.Id,
                                    cus.CustomerNo,
                                    cus.PosCustomerClassId,
                                    ClassName = cls.Name,
                                    cus.Phone,
                                    cus.FirstName,
                                    cus.LastName,
                                    cus.AdditionalPhone,
                                    cus.Address,
                                    cus.Address2,
                                    cus.PosRegionId,
                                    RegionName = rgn.Name,
                                    cus.PosCityOrNearestZoneId,
                                    City = city.Name,
                                    cus.IsPointAllowable,
                                    cus.IsDueAllowable,
                                    cus.IsDefaultPosCustomer,
                                    cus.IsPosBranchCustomer,
                                    cus.PosBranchId,
                                    Branch = brn.Name
                                }).ToList();

                    rtr = Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateCustomer(VmCustomer customer, int id)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    customer.PosCustomer.Id = id;
                    customer.PosCustomer.ModifiedBy = _erpManager.UserId;
                    customer.PosCustomer.ModifideDate = UTCDateTime.BDDateTime();

                    dbContext.Entry(customer.PosCustomer).State = EntityState.Modified;

                    dbContext.Entry(customer.PosCustomer).Property(o => o.CustomerNo).IsModified = false;
                    dbContext.Entry(customer.PosCustomer).Property(o => o.CmnCompanyId).IsModified = false;
                    dbContext.Entry(customer.PosCustomer).Property(o => o.CreatedBy).IsModified = false;
                    dbContext.Entry(customer.PosCustomer).Property(o => o.CreatedDate).IsModified = false;

                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
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
        public ActionResult DeleteCustomer(int customerId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                int cmnId = _erpManager.CmnId;
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    PosCustomer customerToDelete = dbContext.PosCustomerDbSet.Find(customerId);

                    if (customerToDelete != null) dbContext.PosCustomerDbSet.Remove(customerToDelete);
                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
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

        #endregion



        #region DeliveryMan

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult DeliveryMan()
        {
            return View();
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult SaveDeliveryMan(VmDeliveryMan DeliveryMan)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                int cmnId = _erpManager.CmnId;
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    DeliveryMan.posDeliveryMan.IsDefaultPosDeliveryMan = false;
                    DeliveryMan.posDeliveryMan.CmnCompanyId = cmnId;
                    DeliveryMan.posDeliveryMan.CreatedBy = _erpManager.UserId;
                    DeliveryMan.posDeliveryMan.CreatedDate = UTCDateTime.BDDateTime();

                    dbContext.PosDeliveryManDbSet.Add(DeliveryMan.posDeliveryMan);
                    dbContext.SaveChanges();
                    rtr = Json(new { Id = DeliveryMan.posDeliveryMan.Id}, JsonRequestBehavior.AllowGet);
                }
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException.InnerException.Message.Contains("UK_PosDeliveryMan_Phone"))
                {
                    return Json("PhoneNoExists", JsonRequestBehavior.DenyGet);
                }
                //if (e.InnerException.InnerException.Message.Contains("UK_PosDeliveryMan_DeliveryManCode"))
                //{
                //    return Json("DeliveryManCodeExists", JsonRequestBehavior.DenyGet);
                //}

                if (e.InnerException.InnerException.Message.Contains("UK_PosDeliveryMan_NationalID"))
                {
                    return Json("Delivery Man Exists", JsonRequestBehavior.DenyGet);
                }
                rtr = _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetDeliveryMans()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    List<int> branch = _erpManager.UserOffice;
                    
                    var data = (from cus in dbContext.PosDeliveryManDbSet
                                //join cls in dbContext.PosCustomerClassDbSet on cus. equals cls.Id
                                //join rgn in dbContext.PosRegionDbSet on cus.PosRegionId equals rgn.Id
                                //join city in dbContext.PosCityOrNearestZoneDbSet on cus.PosCityOrNearestZoneId equals city.Id
                                join brn in dbContext.PosBrancheDbSet on cus.PosBranchId equals brn.Id
                                select new
                                {
                                    cus.Id,
                                    //cus.DeliveryManCode,
                                    //cus.posDeliveryManClassId,
                                    //ClassName = cls.Name,
                                    cus.Phone,
                                    cus.DeliveryManName_en,
                                    cus.DeliveryManName_ar,
                                    cus.AdditionalPhone,
                                    cus.Address,
                                    cus.NationalID,
                                    //cus.PosRegionId,
                                    //RegionName = rgn.Name,
                                    //cus.PosCityOrNearestZoneId,
                                    //City = city.Name,
                                    //cus.IsPointAllowable,
                                    //cus.IsDueAllowable,
                                    //cus.IsDefaultposDeliveryMan,
                                    //cus.IsPosBranchDeliveryMan,
                                    cus.PosBranchId,
                                    Branch = brn.Name
                                }).Where(c => branch.Contains(c.PosBranchId)).ToList();

                    rtr = Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateDeliveryMan(VmDeliveryMan DeliveryMan, int id)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    DeliveryMan.posDeliveryMan.Id = id;
                    DeliveryMan.posDeliveryMan.ModifiedBy = _erpManager.UserId;
                    DeliveryMan.posDeliveryMan.ModifideDate = UTCDateTime.BDDateTime();

                    dbContext.Entry(DeliveryMan.posDeliveryMan).State = EntityState.Modified;

                    //dbContext.Entry(DeliveryMan.posDeliveryMan).Property(o => o.DeliveryManCode).IsModified = false;
                    dbContext.Entry(DeliveryMan.posDeliveryMan).Property(o => o.CmnCompanyId).IsModified = false;
                    dbContext.Entry(DeliveryMan.posDeliveryMan).Property(o => o.CreatedBy).IsModified = false;
                    dbContext.Entry(DeliveryMan.posDeliveryMan).Property(o => o.CreatedDate).IsModified = false;

                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
                }
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException.InnerException.Message.Contains("UK_PosDeliveryMan_Phone"))
                {
                    return Json("PhoneNoExists", JsonRequestBehavior.DenyGet);
                }
                if (e.InnerException.InnerException.Message.Contains("UK_PosDeliveryMan_NationalID"))
                {
                    return Json("Delivery Man Exists", JsonRequestBehavior.DenyGet);
                }
                rtr = _erpManager.DbUpdateExceptionEntity(e);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        [HasAuthorization(AccessLevel = 4)]
        public ActionResult DeleteDeliveryMan(int DeliveryManId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                int cmnId = _erpManager.CmnId;
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    PosDeliveryMan DeliveryManToDelete = dbContext.PosDeliveryManDbSet.Find(DeliveryManId);

                    if (DeliveryManToDelete != null) dbContext.PosDeliveryManDbSet.Remove(DeliveryManToDelete);
                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
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

        #endregion



        #region Customer city/Zone

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult SaveCustomerCityOrZone(VmCustomer customer)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                int cmnId = _erpManager.CmnId;
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    customer.PosCityOrNearestZone.CmnCompanyId = cmnId;
                    customer.PosCityOrNearestZone.CreatedBy = _erpManager.UserId;
                    customer.PosCityOrNearestZone.CreatedDate = UTCDateTime.BDDateTime();

                    dbContext.PosCityOrNearestZoneDbSet.Add(customer.PosCityOrNearestZone);
                    dbContext.SaveChanges();
                    rtr = Json(customer.PosCityOrNearestZone.Id, JsonRequestBehavior.AllowGet);
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

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateCustomerCityOrZone(VmCustomer customer, int id)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    customer.PosCityOrNearestZone.Id = id;
                    customer.PosCityOrNearestZone.ModifiedBy = _erpManager.UserId;
                    customer.PosCityOrNearestZone.ModifideDate = UTCDateTime.BDDateTime();

                    dbContext.Entry(customer.PosCityOrNearestZone).State = EntityState.Modified;
                    dbContext.Entry(customer.PosCityOrNearestZone).Property(o => o.CmnCompanyId).IsModified = false;
                    dbContext.Entry(customer.PosCityOrNearestZone).Property(o => o.CreatedBy).IsModified = false;
                    dbContext.Entry(customer.PosCityOrNearestZone).Property(o => o.CreatedDate).IsModified = false;

                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
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

        #endregion


        #region Customer Region

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult SaveRegion(VmCustomer customer)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                int cmnId = _erpManager.CmnId;
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    customer.PosRegion.CmnCompanyId = cmnId;
                    customer.PosRegion.CreatedBy = _erpManager.UserId;
                    customer.PosRegion.CreatedDate = UTCDateTime.BDDateTime();

                    dbContext.PosRegionDbSet.Add(customer.PosRegion);
                    dbContext.SaveChanges();
                    rtr = Json(customer.PosRegion.Id, JsonRequestBehavior.AllowGet);
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

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateRegion(VmCustomer customer, int id)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    customer.PosRegion.Id = id;
                    customer.PosRegion.ModifiedBy = _erpManager.UserId;
                    customer.PosRegion.ModifideDate = UTCDateTime.BDDateTime();

                    dbContext.Entry(customer.PosRegion).State = EntityState.Modified;
                    dbContext.Entry(customer.PosRegion).Property(o => o.CmnCompanyId).IsModified = false;
                    dbContext.Entry(customer.PosRegion).Property(o => o.CreatedBy).IsModified = false;
                    dbContext.Entry(customer.PosRegion).Property(o => o.CreatedDate).IsModified = false;

                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
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

        #endregion


        #region Customer Class

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult SaveCustomerClass(VmCustomer customer)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                int cmnId = _erpManager.CmnId;
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    customer.PosCustomerClass.CmnCompanyId = cmnId;
                    customer.PosCustomerClass.Status = true;
                    customer.PosCustomerClass.CreatedBy = _erpManager.UserId;
                    customer.PosCustomerClass.CreatedDate = UTCDateTime.BDDateTime();

                    dbContext.PosCustomerClassDbSet.Add(customer.PosCustomerClass);
                    dbContext.SaveChanges();
                    rtr = Json(customer.PosCustomerClass.Id, JsonRequestBehavior.AllowGet);
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

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateCustomerClass(VmCustomer customer, int id)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    customer.PosCustomerClass.Id = id;
                    customer.PosCustomerClass.ModifiedBy = _erpManager.UserId;
                    customer.PosCustomerClass.ModifideDate = UTCDateTime.BDDateTime();

                    dbContext.Entry(customer.PosCustomerClass).State = EntityState.Modified;
                    dbContext.Entry(customer.PosCustomerClass).Property(o => o.Status).IsModified = false;
                    dbContext.Entry(customer.PosCustomerClass).Property(o => o.CmnCompanyId).IsModified = false;
                    dbContext.Entry(customer.PosCustomerClass).Property(o => o.CreatedBy).IsModified = false;
                    dbContext.Entry(customer.PosCustomerClass).Property(o => o.CreatedDate).IsModified = false;

                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
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

        #endregion


        #region Tender

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult Tender()
        {
            return View();
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult SaveTender(VmTender tender)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                   // tender.PosTender.Type = "NA";
                    tender.PosTender.IsEditable = true;
                    tender.PosTender.CmnCompanyId = _erpManager.CmnId;
                    tender.PosTender.CreatedBy = _erpManager.UserId;
                    tender.PosTender.CreatedDate = UTCDateTime.BDDateTime();

                    dbContext.PosTenderDbSet.Add(tender.PosTender);
                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
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

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateTender(VmTender tender, int id)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    tender.PosTender.Id = id;
                    tender.PosTender.ModifiedBy = _erpManager.UserId;
                    tender.PosTender.ModifideDate = UTCDateTime.BDDateTime();
                    dbContext.Entry(tender.PosTender).State = EntityState.Modified;
                    //dbContext.Entry(tender.PosTender).Property(o => o.Type).IsModified = false;
                    dbContext.Entry(tender.PosTender).Property(o => o.IsEditable).IsModified = false;
                    dbContext.Entry(tender.PosTender).Property(o => o.CmnCompanyId).IsModified = false;
                    dbContext.Entry(tender.PosTender).Property(o => o.CreatedBy).IsModified = false;
                    dbContext.Entry(tender.PosTender).Property(o => o.CreatedDate).IsModified = false;

                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
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
        public ActionResult DeleteTender(int tenderId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {

                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    PosTender tenderToDelete = dbContext.PosTenderDbSet.Find(tenderId);

                    if (tenderToDelete != null) dbContext.PosTenderDbSet.Remove(tenderToDelete);
                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
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

        public ActionResult GetTenders()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {

                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    var data = (from tndr in dbContext.PosTenderDbSet
                                join tdrT in dbContext.PosTenderTypeDbSet on tndr.PosTenderTypeId equals tdrT.Id
                                select new
                                {
                                    Id = tndr.IsEditable ? tndr.Id : 0,
                                    tndr.PosTenderTypeId,
                                    TenderName = tdrT.Name,
                                    tndr.Name,
                                    tndr.Order,
                                    tndr.IsEditable,
                                    tndr.Type
                                }).ToList();

                    rtr = Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        #endregion


        #region Tender type

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult SaveTenderType(VmTender tender)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    tender.PosTenderType.IsEditable = false;
                    tender.PosTenderType.CmnCompanyId = _erpManager.CmnId;
                    tender.PosTenderType.CreatedBy = _erpManager.UserId;
                    tender.PosTenderType.CreatedDate = UTCDateTime.BDDateTime();
                    dbContext.PosTenderTypeDbSet.Add(tender.PosTenderType);
                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
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

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateTenderType(VmTender tender, int id)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    tender.PosTenderType.Id = id;
                    tender.PosTenderType.ModifiedBy = _erpManager.UserId;
                    tender.PosTenderType.ModifideDate = UTCDateTime.BDDateTime();

                    dbContext.Entry(tender.PosTenderType).State = EntityState.Modified;
                    dbContext.Entry(tender.PosTenderType).Property(o => o.IsEditable).IsModified = false;
                    dbContext.Entry(tender.PosTenderType).Property(o => o.CmnCompanyId).IsModified = false;
                    dbContext.Entry(tender.PosTenderType).Property(o => o.CreatedBy).IsModified = false;
                    dbContext.Entry(tender.PosTenderType).Property(o => o.CreatedDate).IsModified = false;

                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
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
        #endregion


        #region Unit Conversion

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult UnitConversion()
        {
            return View();
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult SaveUnitGroupDetails(VmUnitConversion groupDetail)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                int cmnId = _erpManager.CmnId;
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    groupDetail.UomGroupDetail.CmnCompanyId = cmnId;
                    dbContext.PosUomGroupDetailDbSet.Add(groupDetail.UomGroupDetail);
                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
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
        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateUnitGroupDetails(VmUnitConversion groupDetail, int id)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    groupDetail.UomGroupDetail.Id = id;
                    dbContext.Entry(groupDetail.UomGroupDetail).State = EntityState.Modified;
                    dbContext.Entry(groupDetail.UomGroupDetail).Property(o => o.CmnCompanyId).IsModified = false;
                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
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
        public ActionResult DeleteUnitGroupDetails(int ugId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    PosUomGroupDetail groupDetail = dbContext.PosUomGroupDetailDbSet.Find(ugId);

                    if (groupDetail != null) dbContext.PosUomGroupDetailDbSet.Remove(groupDetail);
                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
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

        public ActionResult GetUnitGroupDetails()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    var data = (from grpDtl in dbContext.PosUomGroupDetailDbSet
                                join unitGrp in dbContext.PosUomGroupDbSet on grpDtl.PosUomGroupId equals unitGrp.Id
                                join unitMstr in dbContext.PosUomMasterDbSet on grpDtl.PosUomMasterId equals unitMstr.Id
                                select new
                                {
                                    grpDtl.Id,
                                    grpDtl.PosUomGroupId,
                                    GroupName = unitGrp.Name,
                                    grpDtl.PosUomMasterId,
                                    UnitMasterCode = unitMstr.UomCode,
                                    grpDtl.ConversionFactor
                                }).ToList();

                    rtr = Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }


        #endregion


        #region Supplier

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult Supplier()
        {
            return View();
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult SaveSupplier(PosSupplier supplier)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    supplier.CmnCompanyId = _erpManager.CmnId;
                    dbContext.PosSupplierDbSet.Add(supplier);
                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateSupplier(PosSupplier supplier)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    supplier.ModifiedBy = _erpManager.UserId;
                    supplier.ModifideDate = UTCDateTime.BDDateTime();

                    dbContext.Entry(supplier).State = EntityState.Modified;
                    dbContext.Entry(supplier).Property(o => o.CmnCompanyId).IsModified = false;
                    dbContext.Entry(supplier).Property(o => o.CreatedBy).IsModified = false;
                    dbContext.Entry(supplier).Property(o => o.CreatedDate).IsModified = false;

                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        [HasAuthorization(AccessLevel = 4)]
        public ActionResult DeleteSupplier(int supplierId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {

                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    PosSupplier supplierToDelete = dbContext.PosSupplierDbSet.Find(supplierId);

                    if (supplierToDelete != null) dbContext.PosSupplierDbSet.Remove(supplierToDelete);
                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetAllSuppliers()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                int cmnId = _erpManager.CmnId;
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    var suppliers = dbContext.PosSupplierDbSet.Where(o => o.CmnCompanyId == cmnId).Select(o => new
                    {
                        supplierId = o.Id,
                        o.Name,
                        o.Address,
                        o.Phone,
                        o.Fax,
                        o.Email
                    }).ToList();
                    rtr = Json(suppliers, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        #endregion


        #region UOM Group

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult SaveUnitGroup(VmUnitConversion unitGroup)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                int cmnId = _erpManager.CmnId;
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    unitGroup.UomGroup.CmnCompanyId = cmnId;
                    dbContext.PosUomGroupDbSet.Add(unitGroup.UomGroup);
                    dbContext.SaveChanges();
                    rtr = Json(unitGroup.UomGroup.Id, JsonRequestBehavior.AllowGet);
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
        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateUnitGroup(VmUnitConversion unitroup, int id)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    unitroup.UomGroup.Id = id;
                    unitroup.UomGroup.ModifiedBy = _erpManager.UserId;
                    unitroup.UomGroup.ModifideDate = UTCDateTime.BDDateTime();
                    dbContext.Entry(unitroup.UomGroup).State = EntityState.Modified;
                    dbContext.Entry(unitroup.UomGroup).Property(o => o.CreatedDate).IsModified = false;
                    dbContext.Entry(unitroup.UomGroup).Property(o => o.CreatedBy).IsModified = false;
                    dbContext.Entry(unitroup.UomGroup).Property(o => o.CmnCompanyId).IsModified = false;
                    int feedback = dbContext.SaveChanges();
                    rtr = Json(feedback, JsonRequestBehavior.AllowGet);
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
        #endregion


        #region Product Category

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult ProductCategory()
        {
            return View();
        }

        [HasAuthorization(AccessLevel = 2)]
        public ActionResult SaveProductCategory(PosProductCategory productCategory)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (var context = new ConnectionDatabase())
                {
                    productCategory.CmnCompanyId = _erpManager.CmnId;
                    productCategory.CreatedBy = _erpManager.UserId;
                    productCategory.CreatedDate = UTCDateTime.BDDateTime();

                    context.PosProductCategoryDbSet.Add(productCategory);
                    int rowInserted = context.SaveChanges();
                    rtr = Json(rowInserted, JsonRequestBehavior.AllowGet);
                }
            }
            catch (DbUpdateException ex)
            {
                rtr = _erpManager.DbUpdateExceptionEntity(ex);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateProductCategory(PosProductCategory productCategory)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase context = new ConnectionDatabase())
                {
                    productCategory.ModifiedBy = _erpManager.UserId;
                    productCategory.ModifideDate = UTCDateTime.BDDateTime();

                    context.Entry(productCategory).State = EntityState.Modified;
                    context.Entry(productCategory).Property(o => o.CmnCompanyId).IsModified = false;
                    context.Entry(productCategory).Property(o => o.CreatedBy).IsModified = false;
                    context.Entry(productCategory).Property(o => o.CreatedDate).IsModified = false;

                    int feedback = context.SaveChanges();
                    return Json(feedback, JsonRequestBehavior.AllowGet);
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
        public ActionResult DeleteProductCategory(int categoryId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase context = new ConnectionDatabase())
                {
                    PosProductCategory categoryToDelete = context.PosProductCategoryDbSet.Find(categoryId);

                    if (categoryToDelete != null) context.PosProductCategoryDbSet.Remove(categoryToDelete);
                    int feedback = context.SaveChanges();
                    return Json(feedback, JsonRequestBehavior.AllowGet);
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

        public ActionResult GetAllProductCategories()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                int cmnId = _erpManager.CmnId;
                using (ConnectionDatabase context = new ConnectionDatabase())
                {
                    var categories = context.PosProductCategoryDbSet
                                    .Where(p => p.CmnCompanyId == cmnId)
                                    .Select(p => new { CategoryId =p.Id, p.Name, p.Description }).ToList();
                    return Json(categories, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }


        #endregion


        #region Billing Report Text

        [HasAuthorization(AccessLevel = 1)]
        public ActionResult BillingReportText()
        {
            return View();
        }


        [HasAuthorization(AccessLevel = 2)]
        public ActionResult SaveBillingReportText(PosBillingReportText billingReportText)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (var context = new ConnectionDatabase())
                {
                    billingReportText.PoweredBy = "<b>Powered By: www.Trend-egypt.com</b>";
                    billingReportText.CmnCompanyId = _erpManager.CmnId;
                    billingReportText.CreatedBy = _erpManager.UserId;
                    billingReportText.CreatedDate = UTCDateTime.BDDateTime();

                    context.PosBillingReportTextDbSet.Add(billingReportText);
                    int rowInserted = context.SaveChanges();
                    rtr = Json(rowInserted, JsonRequestBehavior.AllowGet);
                }
            }
            catch (DbUpdateException ex)
            {
                rtr = _erpManager.DbUpdateExceptionEntity(ex);
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }


        [HasAuthorization(AccessLevel = 3)]
        public ActionResult UpdateBillingReportText(PosBillingReportText billingReportText)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase context = new ConnectionDatabase())
                {
                    PosBillingReportText entityToUpdate = context.PosBillingReportTextDbSet.Find(billingReportText.Id);
                    if (entityToUpdate == null)
                        return Json(0, JsonRequestBehavior.DenyGet);

                    entityToUpdate.PosBranchId = billingReportText.PosBranchId;
                    entityToUpdate.Text = billingReportText.Text;
                    entityToUpdate.ModifiedBy = _erpManager.UserId;
                    entityToUpdate.ModifideDate = UTCDateTime.BDDateTime();

                    int feedback = context.SaveChanges();
                    return Json(feedback, JsonRequestBehavior.AllowGet);
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
        public ActionResult DeleteBillingReportText(int billingReportTextId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase context = new ConnectionDatabase())
                {
                    PosBillingReportText categoryToDelete = context.PosBillingReportTextDbSet.Find(billingReportTextId);

                    if (categoryToDelete != null) context.PosBillingReportTextDbSet.Remove(categoryToDelete);
                    int feedback = context.SaveChanges();
                    return Json(feedback, JsonRequestBehavior.AllowGet);
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


        public ActionResult GetAllBillingReportTexts()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                int cmnId = _erpManager.CmnId;
                using (ConnectionDatabase context = new ConnectionDatabase())
                {
                    var categories = context.PosBillingReportTextDbSet
                                    .Where(p => p.CmnCompanyId == cmnId)
                                    .Select(p => new {BillingReportTextId = p.Id, p.PosBranchId, BranchName = p.PosBranch.Name, p.Text }).ToList();
                    return Json(categories, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }


        #endregion
    }

    public class VmShift
    {
        PosShift _Shift = new PosShift();
         public PosShift PosShift
        {
            get { return _Shift; }
            set { _Shift = value; }

        }
        public string UserName { get; set; }
    }

}