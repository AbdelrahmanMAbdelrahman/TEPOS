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
    public partial class DailySalesValue : System.Web.UI.Page
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
                RptDailySalesValueTableAdapter tableAdapter = new RptDailySalesValueTableAdapter();
                DataTable dataTable = tableAdapter.GetData(dateFrom, dateTo, branchId, erpManager.CmnId);

                ReportViewer2.LocalReport.ReportPath = "Report/Pos/Rdlc/Sales/DailySalesValue.rdlc";
                ReportDataSource dataSource = new ReportDataSource("dsDailySalesValue", dataTable);

                ReportViewer2.LocalReport.EnableExternalImages = true;
                ReportViewer2.LocalReport.DataSources.Clear();
                ReportViewer2.LocalReport.DataSources.Add(dataSource);

                var parameters = new List<ReportParameter>
                        {
                            new ReportParameter("CompanyName", erpManager.CompanyName),
                            new ReportParameter("CompanyAddress", erpManager.CompanyAddress),
                           new ReportParameter("CompanyLogo", UtilityManager.CompanyImageUrl),
                            new ReportParameter("DateFrom", dateFrom.ToString("dd-MMM-yyyy").ToUpper()),
                            new ReportParameter("DateTo", dateTo.ToString("dd-MMM-yyyy").ToUpper()),
                            new ReportParameter("poweredby",erpManager.PoweredBy),
                           new ReportParameter("branch", erpManager.BranchNameList(branchId.Split(',').Select(int.Parse).ToList()))

            };
                ReportViewer2.LocalReport.SetParameters(parameters);
                ReportViewer2.LocalReport.Refresh();
             
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                Document dcDocument = new Document();
                string report = ReportManager.ReportViewer(ReportViewer2, dcDocument, "DailySalesValue", new ErpManager().UserName);
                Response.Write(report);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }
    }
}