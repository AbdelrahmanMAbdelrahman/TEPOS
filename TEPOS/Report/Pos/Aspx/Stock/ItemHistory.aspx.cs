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
using ERP.Report.Pos.Xsd.DsSaleTableAdapters;
using ERP.Report.Pos.Xsd.DsStockTableAdapters;
using ERP.Report.ReportController;
using iTextSharp.text;
using Microsoft.Reporting.WebForms;

namespace ERP.Report.Pos.Aspx.Stock
{
    [HasAuthorization]
    public partial class ItemHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    ErpManager erpManager = new ErpManager();
                    int companyId = erpManager.CmnId;

                    DateTime dateFrom = Convert.ToDateTime(Request.QueryString["dateFrom"].ToString());
                    DateTime dateTo = Convert.ToDateTime(Request.QueryString["dateTo"].ToString());
                    string branchId = Request.QueryString["branchId"];
                    int categoryId = Convert.ToInt32(Request.QueryString["categoryId"]);
                    int productId = Convert.ToInt32(Request.QueryString["productId"]);

                    string companyName = erpManager.CompanyName;
                    string companyAddress = erpManager.CompanyAddress;

                    RptItemsHistoryTableAdapter tableAdapter = new RptItemsHistoryTableAdapter();
                    if (string.IsNullOrEmpty(branchId))
                    {
                        branchId = string.Join(",", erpManager.UserOffice);
                    }
                    DataTable dataTable = tableAdapter.GetData(dateFrom, dateTo, branchId, companyId, productId, categoryId);

                    ReportViewer2.LocalReport.ReportPath = "Report/Pos/Rdlc/Stock/ItemHistory.rdlc";
                    ReportDataSource dataSource = new ReportDataSource("dsItemHistory", dataTable);

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
                            new ReportParameter("PoweredBy", erpManager.PoweredBy),
                        };
                    ReportViewer2.LocalReport.SetParameters(parameters);
                    ReportViewer2.LocalReport.Refresh();

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
