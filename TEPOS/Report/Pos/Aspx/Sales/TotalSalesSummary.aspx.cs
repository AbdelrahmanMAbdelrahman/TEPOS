using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.Controllers.Manager;
using ERP.Report.Pos.Xsd;
using ERP.Report.Pos.Xsd.DsSaleTableAdapters;
using ERP.Report.ReportController;
using iTextSharp.text;
using Microsoft.Reporting.WebForms;
using ERP.CSharpLib;

namespace ERP.Report.Pos.Aspx.Sales
{
    public partial class TotalSalesSummary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ErpManager erpManager = new ErpManager();
                DateTime dateFrom = Convert.ToDateTime(Request.QueryString["dateFrom"]);
                DateTime dateTo = Convert.ToDateTime(Request.QueryString["dateTo"]);
                string branchId = Request.QueryString["branchId"];
                if (string.IsNullOrEmpty(branchId))
                {
                    branchId = string.Join(",", erpManager.UserOffice);
              
                }
                RptTotalSalesSummaryOfCategoryWiseTableAdapter categoryAdapter = new RptTotalSalesSummaryOfCategoryWiseTableAdapter();
                RptTotalSalesSummaryTableAdapter salesSummaryTableAdapter=new RptTotalSalesSummaryTableAdapter();
                RptTotalSalesSummaryOfInvoiceStatisticsTableAdapter statisticsTableAdapter=new RptTotalSalesSummaryOfInvoiceStatisticsTableAdapter();
                RptTotalSalesSummaryOfTenderDetailsTableAdapter tenderDetailsTableAdapter=new RptTotalSalesSummaryOfTenderDetailsTableAdapter();
                CommandTimeOut.ChangeTimeout(categoryAdapter, 0);
                CommandTimeOut.ChangeTimeout(salesSummaryTableAdapter, 0);
                CommandTimeOut.ChangeTimeout(statisticsTableAdapter, 0);
                CommandTimeOut.ChangeTimeout(tenderDetailsTableAdapter, 0);

                ReportDataSource rds0 = new ReportDataSource("TotalSalesSummary", (DataTable)salesSummaryTableAdapter.GetData(dateFrom, dateTo, branchId, erpManager.CmnId));
                ReportDataSource rds1 = new ReportDataSource("CatagoryWise", (DataTable)categoryAdapter.GetData(dateFrom, dateTo, branchId, erpManager.CmnId));
                ReportDataSource rds2 = new ReportDataSource("InvStatic", (DataTable)statisticsTableAdapter.GetData(dateFrom, dateTo, branchId, erpManager.CmnId));
                ReportDataSource rds3 = new ReportDataSource("TenderDetails", (DataTable)tenderDetailsTableAdapter.GetData(dateFrom, dateTo, branchId, erpManager.CmnId));
                ReportViewer2.LocalReport.ReportPath = "Report/Pos/Rdlc/Sales/TotalSalesSummary.rdlc";

                ReportViewer2.LocalReport.EnableExternalImages = true;
                ReportViewer2.LocalReport.DataSources.Clear();
                ReportViewer2.LocalReport.DataSources.Add(rds0);
                ReportViewer2.LocalReport.DataSources.Add(rds1);
                ReportViewer2.LocalReport.DataSources.Add(rds2);
                ReportViewer2.LocalReport.DataSources.Add(rds3);

                var param = new ReportParameter[8];
                param[0] = new ReportParameter("CompanyName", erpManager.CompanyName);
                param[1] = new ReportParameter("CompanyAddress", erpManager.CompanyAddress);
                param[2] = new ReportParameter("DateFrom", dateFrom.ToString("dd-MM-yyyy"));
                param[3] = new ReportParameter("DateTo", dateTo.ToString("dd-MM-yyyy"));
                param[4] = new ReportParameter("cmnImage", UtilityManager.CompanyImageUrl);
                param[5] = new ReportParameter("poweredby", erpManager.PoweredBy);
                param[6] = new ReportParameter("branch", erpManager.BranchNameList(branchId.Split(',').Select(int.Parse).ToList()));
                param[7] = new ReportParameter("CurrentDateTime", UTCDateTime.BDDateTime().ToString("dd-MM-yyyy hh:mm:ss tt"));

                ReportViewer2.LocalReport.SetParameters(param);
                ReportViewer2.LocalReport.Refresh();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                Document dcDocument = new Document();
                string report = ReportManager.ReportViewer(ReportViewer2, dcDocument, "TotalSalesSummary", new ErpManager().UserName);
                Response.Write(report);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }
    }
}