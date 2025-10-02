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
using ERP.Models;

namespace ERP.Report.Pos.Aspx.Sales
{
    public partial class BranchSalesSummary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    using (var context = new ConnectionDatabase())
                    {
                        ErpManager erpManager = new ErpManager();
                        int companyId = erpManager.CmnId;

                        DateTime dateFrom = Convert.ToDateTime(Request.QueryString["dateFrom"].ToString());
                        DateTime dateTo = Convert.ToDateTime(Request.QueryString["dateTo"].ToString());
                        string branchId = Request.QueryString["branchId"];
                        if (string.IsNullOrEmpty(branchId))
                        {
                            branchId = string.Join(",", erpManager.UserOffice);

                        }
                        int productCaregoryId = Convert.ToInt32(Request.QueryString["productCaregoryId"]);
                        string ProductName = Request.QueryString["ProductName"];
                        var company = context.CompanyDbSet.FirstOrDefault(o => o.Id == companyId);
                        string companyName = company != null ? company.Name : "Unknown Company";
                        string companyAddress = company != null ? company.Address : "Unknown Address";

                        RptBranchSalesSummaryTableAdapter salesSummaryTableAdapter = new RptBranchSalesSummaryTableAdapter();
                        DataTable dataTable = salesSummaryTableAdapter.GetData(dateFrom, dateTo, branchId, erpManager.CmnId);
                        RptBranchSalesSummaryBySellerTableAdapter salesSummaryBySellerTableAdapter = new RptBranchSalesSummaryBySellerTableAdapter();
                        DataTable dataTable2 = salesSummaryBySellerTableAdapter.GetData(dateFrom, dateTo, branchId, erpManager.CmnId);


                        ReportViewer2.LocalReport.ReportPath = "Report/Pos/Rdlc/Sales/BranchSalesSummary.rdlc";
                        ReportDataSource dataSource = new ReportDataSource("dsBranchSales", dataTable);
                        ReportDataSource dataSource2 = new ReportDataSource("dsBranchSalesBySeller", dataTable2);
                        ReportViewer2.LocalReport.EnableExternalImages = true;
                        ReportViewer2.LocalReport.DataSources.Clear();
                        ReportViewer2.LocalReport.DataSources.Add(dataSource);
                        ReportViewer2.LocalReport.DataSources.Add(dataSource2);

                        var parameters = new List<ReportParameter>
                        {
                            new ReportParameter("CompanyName", companyName),
                            new ReportParameter("CompanyAddress", companyAddress),
                            new ReportParameter("CompanyLogo", UtilityManager.CompanyImageUrl),
                            new ReportParameter("DateFrom", dateFrom.ToString("dd-MMM-yyyy").ToUpper()),
                            new ReportParameter("DateTo", dateTo.ToString("dd-MMM-yyyy").ToUpper()),
                            new ReportParameter("poweredby", erpManager.PoweredBy),
                             new ReportParameter("CmnId", erpManager.CmnId.ToString()),

                        };
                        ReportViewer2.LocalReport.SetParameters(parameters);
                        ReportViewer2.LocalReport.Refresh();

                    }
                }
                catch (Exception exception)
                {
                    return;
                }
            }
        
                
    }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                Document dcDocument = new Document();
                string report = ReportManager.ReportViewer(ReportViewer2, dcDocument, "BranchSalesSummary", new ErpManager().UserName);
                Response.Write(report);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }
    }
}