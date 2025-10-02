using ERP.Controllers.Manager;
using ERP.CSharpLib;
using ERP.Models;
using ERP.Models.Security.Authorization;
using ERP.Models.VModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ERP.Controllers.POS
{
   
    [HasAuthorization]
    public partial class PosDashBoardController : Controller
    {
        readonly ErpManager _erpManager = new ErpManager();

        public ActionResult PosDashBoard()
        {
            int moduleId = Convert.ToInt16(Request.QueryString["moduleId"]);
            if (moduleId == 0)
            {
                Uri uri = new Uri(HttpContext.Request.Url.AbsoluteUri);
                moduleId = Convert.ToInt32(uri.Segments[3]);
            }
            Session["sModuleId"] = moduleId;
            ViewBag.todaysData =new PosDashBoardController().TodaysSalesAndDue();
            return View();
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string culture = (string)Session["CurrentCulture"];

            if (string.IsNullOrEmpty(culture))
            {
                culture = "en-US"; // اللغة الافتراضية
                Session["CurrentCulture"] = culture;
            }

            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

            base.OnActionExecuting(filterContext);
        }

        public ActionResult ChangeCulture(string culture)
        {
            Session["CurrentCulture"] = culture;
            string returnUrl = Request.UrlReferrer?.ToString() ?? Url.Action("Index", "Home");
            return Redirect(returnUrl);
        }

        public ActionResult PosLastTwoMonthSalesBranchWise()
        {
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    DateTime dt = UTCDateTime.BDDate();
                    int currentMonth = dt.Month;
                    int currentYear = dt.Year;
                    int lastOneMonth = dt.AddMonths(-1).Month;
                    int lastOneMonthYear = dt.AddMonths(1).Year;
                    List<int> office = _erpManager.UserOffice.ToList();
                    var salesOfCurrentMonth = (from sal in database.PosSalesInvoiceDbSet
                        join br in database.PosBrancheDbSet on sal.PosBranchId equals br.Id
                        where SqlFunctions.DatePart("Month", sal.InvDate) == currentMonth && SqlFunctions.DatePart("Year", sal.InvDate) == currentYear
                              && office.Contains(sal.PosBranchId) && sal.CmnCompanyId == _erpManager.CmnId && !UtilityManager.PosInvoiceTypeNotInForSalesSide.Contains(sal.PosInvoiceType)
                        group new {br, sal} by new {br.CmnCompanyId, br.Name}
                        into gbBr
                        select new
                        {
                            Branch = gbBr.Key.Name,
                            LastMonth = (decimal) 0,
                            CurrentMonth = gbBr.Sum(s => s.sal.PaidableAmount)

                        }).ToList();
                    var salesOfPreviousOneMonth = (from sal in database.PosSalesInvoiceDbSet
                        join br in database.PosBrancheDbSet on sal.PosBranchId equals br.Id
                        where SqlFunctions.DatePart("Month", sal.InvDate) == lastOneMonth && SqlFunctions.DatePart("Year", sal.InvDate) == lastOneMonthYear
                              && office.Contains(sal.PosBranchId) && sal.CmnCompanyId == _erpManager.CmnId && !UtilityManager.PosInvoiceTypeNotInForSalesSide.Contains(sal.PosInvoiceType)
                        group new {br, sal} by new {br.CmnCompanyId, br.Name}
                        into gbBr
                        select new
                        {
                            Branch = gbBr.Key.Name,
                            LastMonth = gbBr.Sum(s => s.sal.PaidableAmount),
                            CurrentMonth = (decimal) 0
                        }).ToList();
                    var finalDataM = salesOfCurrentMonth.Union(salesOfPreviousOneMonth).ToList();
                    var finalData = (from l in finalDataM
                        group l by new {l.Branch}
                        into gB
                        select new
                        {
                            gB.Key.Branch,
                            LastMonth =gB.Sum(g1 => g1.LastMonth),
                            CurrentMonth = gB.Sum(gs => gs.CurrentMonth)

                        }).ToList();
                    return UtilityManager.JsonResultMax(Json(finalData), JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public VmTodaySalesInfoDashboard TodaysSalesAndDue()
        {

            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                DateTime dte = DateTime.Now.Date;
                List<int> office = _erpManager.UserOffice.ToList();
                var data = (from l in database.PosSalesInvoiceDbSet
                    where l.CmnCompanyId == _erpManager.CmnId && l.InvDate >= dte
                          && office.Contains(l.PosBranchId) && !UtilityManager.PosInvoiceTypeNotInForSalesSide.Contains(l.PosInvoiceType)
                    group l by new {l.InvDate}
                    into gbL
                    select new
                    {
                        TotalInvoice = gbL.Count(),
                        Sales = gbL.Sum(s => s.PaidableAmount),
                        Due = gbL.Sum(s => s.DueAmount)
                    }).FirstOrDefault();

                decimal dueCollection = database.PosCustomerDueCollectionDbSet.Where(w => DbFunctions.TruncateTime(w.Date) == dte && office.Contains(w.PosSalesInvoice.PosBranchId)).Select(s=>s.Amount).DefaultIfEmpty(0).Sum();
                if (data != null)
                    return new VmTodaySalesInfoDashboard()
                    {
                        Due = data.Due,
                        DueCollection = dueCollection,
                        Sales = data.Sales,
                        TotalInvoice = data.TotalInvoice
                    };
                return new VmTodaySalesInfoDashboard()
                {
                    Due = 0,
                    DueCollection = dueCollection,
                    Sales = 0,
                    TotalInvoice = 0
                };


            }
        }

        public ActionResult GetLast30DaysSales()
        {
            try
            {
                DateTime dte = DateTime.Now.AddDays(-30).Date;
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    List<int> office = _erpManager.UserOffice.ToList();
                    var data = (from l in database.PosSalesInvoiceDbSet
                        where l.CmnCompanyId == _erpManager.CmnId && l.InvDate >= dte
                              && office.Contains(l.PosBranchId) && !UtilityManager.PosInvoiceTypeNotInForSalesSide.Contains(l.PosInvoiceType)
                        group l by new {l.InvDate}
                        into gbL
                        select new
                        {
                            Date = gbL.Key.InvDate,
                            Sales = gbL.Sum(s => s.PaidableAmount),
                            Due = gbL.Sum(s =>s.DueAmount)
                        }).ToList();
                    var lst = (from l in data
                        select new
                        {
                            Date = l.Date.ToString("yyyy-MM-dd"),
                            l.Due,
                            l.Sales
                        }).ToList();

                    return UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return null;
        }


        public ActionResult BranchWiseLast30DaysSales()
        {
            try
            {
                DateTime date = DateTime.Now.AddDays(-30).Date;
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    List<int> office = _erpManager.UserOffice.ToList();

                    var listM = (from inv in database.PosSalesInvoiceDbSet
                        where inv.CmnCompanyId == _erpManager.CmnId && inv.InvDate >= date && !UtilityManager.PosInvoiceTypeNotInForSalesSide.Contains(inv.PosInvoiceType)

                              && office.Contains(inv.PosBranchId)
                        group inv by new {inv.InvDate, inv.PosBranchId}
                        into invGroup
                        select new
                        {
                            Date = invGroup.Key.InvDate,
                            Branch = invGroup.FirstOrDefault().PosBranch.Name,
                            Sales = invGroup.Select(s => s.PaidableAmount).DefaultIfEmpty(0).Sum()
                        }).ToList();



                    var lst = new
                    {
                        Data = (from l in listM
                            group l by new {l.Date}
                            into lg
                            select new
                            {
                                Date = lg.Key.Date.ToString("yyyy-MM-dd"),
                                BranchWiseSales = listM.Where(d => d.Date == lg.Key.Date).GroupBy(g => new {g.Date, g.Branch}).Select(s => s.FirstOrDefault()).Select(s => new {s.Branch, s.Sales}).ToList()
                            }).ToList(),
                        BranchList = listM.GroupBy(g => g.Branch).Select(s => s.FirstOrDefault()).Select(s => new {s.Branch}).ToList()
                    };
                    return UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return null;
        }

    }
}