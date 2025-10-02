using System;
using System.Data;
using ERP.Controllers.Manager;
using ERP.Models.Security.Authorization;
using ERP.Report.Pos.Xsd.DsSaleTableAdapters;
using ERP.Report.ReportController;
using iTextSharp.text;
using Microsoft.Reporting.WebForms;

namespace ERP.Report.Pos.Aspx.Sales
{
    [HasAuthorization]
    public partial class ReportViewerCmn : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ErpManager erpManager = new ErpManager();
                DateTime dateFrom = Convert.ToDateTime(Request.QueryString["dateFrom"]);
                DateTime dateTo = Convert.ToDateTime(Request.QueryString["dateTo"]);
                string branchId = Request.QueryString["branchId"];
                string invoiceNo = Request.QueryString["invoiceNo"];
                string ProductName = Request.QueryString["ProductName"];
                int productCaregoryId = Convert.ToInt32(Request.QueryString["productCaregoryId"]);
                bool isRelatedAllInvoice = Convert.ToBoolean(Request.QueryString["isRelatedAllInvoice"]);
                bool IsInsite = Convert.ToBoolean(Request.QueryString["IsInsite"]);
                bool IsDelivery = Convert.ToBoolean(Request.QueryString["IsDelivery"]);
                bool IsCallCenter = Convert.ToBoolean(Request.QueryString["IsCallCenter"]);
                long invNo;
                bool isBoolInvNo = long.TryParse(invoiceNo, out invNo);
                if (!isBoolInvNo)
                {
                    invNo = 0;
                }
                if (string.IsNullOrEmpty(branchId))
                {
                    branchId = string.Join(",", erpManager.UserOffice);
                }
                int posInvoiceType = Convert.ToInt32(Request.QueryString["PosInvoiceType"]);

                RptDetailedSalesTableAdapter adapter = new RptDetailedSalesTableAdapter();

                CommandTimeOut.ChangeTimeout(adapter, 0);
                ReportDataSource rds = new ReportDataSource("DataSet1", (DataTable) adapter.GetData(dateFrom, dateTo, branchId, erpManager.CmnId, invNo, isRelatedAllInvoice, productCaregoryId, IsInsite, IsDelivery, IsCallCenter, ProductName, posInvoiceType));
                ReportViewer2.LocalReport.ReportPath = "Report/Pos/Rdlc/Sales/DetailedSales.rdlc";

                ReportViewer2.LocalReport.EnableExternalImages = true;
                ReportViewer2.LocalReport.DataSources.Clear();
                ReportViewer2.LocalReport.DataSources.Add(rds);

                var param = new ReportParameter[6];
                param[0] = new ReportParameter("CompanyName", erpManager.CompanyName);
                param[1] = new ReportParameter("CompanyAddress", erpManager.CompanyAddress);
                param[2] = new ReportParameter("DateFrom", dateFrom.ToString("dd-MM-yyyy"));
                param[3] = new ReportParameter("DateTo", dateTo.ToString("dd-MM-yyyy"));
                param[4] = new ReportParameter("cmnImage", UtilityManager.CompanyImageUrl);
                param[5] = new ReportParameter("poweredby", erpManager.PoweredBy);

                ReportViewer2.LocalReport.SetParameters(param);
                ReportViewer2.LocalReport.Refresh();
            }
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string userName = new ErpManager().UserName;
                Document dcDocument = new Document();

                ReportManager.ReportViewer(ReportViewer2, dcDocument, userName);

                //Attach pdf to the iframe
                frmPrint.Attributes["src"] = userName + "print.pdf";
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }
    }
}