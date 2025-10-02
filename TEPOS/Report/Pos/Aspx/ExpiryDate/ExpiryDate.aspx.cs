using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.Controllers.Manager;
using ERP.Models.Security.Authorization;
using ERP.Report.Pos.Xsd;
using ERP.Report.Pos.Xsd.DsExpiryDateTableAdapters;
using ERP.Report.ReportController;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;

namespace ERP.Report.Pos.Aspx.ExpiryDate
{
    [HasAuthorization]
    public partial class ExpiryDate : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                ErpManager  erpManager=new ErpManager();
                string userOffice = Request.QueryString["userOffice"];
                string location = Request.QueryString["location"];
                int stockValueAaPar = Convert.ToInt32(Request.QueryString["stockValueAaPar"]);
                int productCategoryId = Convert.ToInt32(Request.QueryString["productCategoryId"]);


                if (string.IsNullOrEmpty(userOffice))
                {
                    userOffice = string.Join(",", erpManager.UserOffice);
                }

                RptExpiryDateTableAdapter adapter = new RptExpiryDateTableAdapter();
                CommandTimeOut.ChangeTimeout(adapter, 0);
                ReportDataSource rds = new ReportDataSource("ExpiryDate", (DataTable)adapter.GetData(userOffice, erpManager.CmnId, productCategoryId, stockValueAaPar));

                ReportViewer2.LocalReport.ReportPath = "Report/Pos/Rdlc/ExpiryDate/RptExpiryDate.rdlc";

                ReportViewer2.LocalReport.EnableExternalImages = true;
                ReportViewer2.LocalReport.DataSources.Clear();
                ReportViewer2.LocalReport.DataSources.Add(rds);
                var param = new ReportParameter[7];
                param[0] = new ReportParameter("stockValueAaPar", stockValueAaPar.ToString());
                param[1] = new ReportParameter("address", erpManager.CompanyAddress);
                param[2] = new ReportParameter("companyName", erpManager.CompanyName);
                param[3] = new ReportParameter("branch", erpManager.BranchNameList(userOffice.Split(',').Select(int.Parse).ToList()));
                param[4] = new ReportParameter("poweredby", erpManager.PoweredBy);
                param[5] = new ReportParameter("cmnImage", UtilityManager.CompanyImageUrl);
                param[6] = new ReportParameter("location", location);
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
                
                ReportManager.ReportViewer(ReportViewer2, dcDocument,userName);

                //Attach pdf to the iframe
                frmPrint.Attributes["src"] =userName+"print.pdf";
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }
    }
}