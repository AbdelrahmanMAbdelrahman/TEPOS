using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.Controllers.Manager;
using ERP.Models;
using ERP.Models.Security.Authorization;
using ERP.Report.Pos.Xsd.DsPurchaseTableAdapters;
using ERP.Report.Pos.Xsd.DsSaleTableAdapters;
using ERP.Report.ReportController;
using iTextSharp.text;
using Microsoft.Reporting.WebForms;


namespace ERP.Report.Pos.Aspx.Purchase
{
    [HasAuthorization]
    public partial class PurchaseReceipt : System.Web.UI.Page
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
                        int branchId = Convert.ToInt32(Request.QueryString["branchId"]);
                        int PosSupplierid = Convert.ToInt32(Request.QueryString["PosSupplierid"]);
                        //int ProductId = Convert.ToInt32(Request.QueryString["ProductId"]);
                        string InvoiceNo = Request.QueryString["InvoiceNo"];
                        string ProductName = Request.QueryString["ProductName"];
                        var company = context.CompanyDbSet.FirstOrDefault(o => o.Id == companyId);
                        string companyName = company != null ? company.Name : "Unknown Company";
                        string companyAddress = company != null ? company.Address : "Unknown Address";
                        int posInvoiceType = Convert.ToInt32(Request.QueryString["PosInvoiceType"]);

                        RptPurchaseReceiptTableAdapter tableAdapter = new RptPurchaseReceiptTableAdapter();
                        DataTable dataTable = tableAdapter.GetData(dateFrom, dateTo, branchId,companyId, PosSupplierid, ProductName, InvoiceNo, posInvoiceType);

                        ReportViewer2.LocalReport.ReportPath = "Report/Pos/Rdlc/Purchase/PurchaseReceipt.rdlc";
                        ReportDataSource dataSource = new ReportDataSource("dsPurchaseReceipt", dataTable);
                        
                        ReportViewer2.LocalReport.EnableExternalImages = true;
                        ReportViewer2.LocalReport.DataSources.Clear();
                        ReportViewer2.LocalReport.DataSources.Add(dataSource);

                        var parameters = new List<ReportParameter>
                        {
                            new ReportParameter("CompanyName", companyName),
                            new ReportParameter("CompanyAddress", companyAddress),
                            new ReportParameter("CompanyLogo", UtilityManager.CompanyImageUrl),
                            new ReportParameter("DateFrom", dateFrom.ToString("dd-MMM-yyyy").ToUpper()),
                            new ReportParameter("DateTo", dateTo.ToString("dd-MMM-yyyy").ToUpper()),
                            new ReportParameter("poweredby",erpManager.PoweredBy),
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
            string userName = new ErpManager().UserName;
            Document dcDocument = new Document();

            ReportManager.ReportViewer(ReportViewer2, dcDocument, userName);

            //Attach pdf to the iframe
            frmPrint.Attributes["src"] = userName + "print.pdf";
        }

    }
}