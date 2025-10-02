using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using Rectangle = System.Drawing.Rectangle;

namespace ERP.Report.ReportController
{
    public class ReportManager
    {

        public static string ReportViewer(ReportViewer rptViewer, Document document, string reportName, string userId)
        {

            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(reportName + "_" + userId + ".pdf"), FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            var url = HttpContext.Current.Request.Url;
            var urlBuilder = new System.UriBuilder(url.AbsoluteUri)
            {
                Path = url.Segments[0] + "/" + url.Segments[1] + "/" + url.Segments[2] + "/" + url.Segments[3] + "/" + reportName + "_" + userId + ".pdf",
                Query = null
            };
            var repPath = new System.UriBuilder(url.AbsoluteUri)
            {
                Path = "Report/Pos/Aspx/PrintReport.aspx",
                Query = null
            };
            string rpt = "<script>window.open('" + repPath.ToString() + "?url=" + urlBuilder.ToString() + "&rptName=" + reportName + "','_blank');</script>";
            return rpt;
        }


        public static void ReportViewer(ReportViewer rptViewer,Document document,string userName)
        {
            string fileName = userName + "output.pdf";
            string printName= userName + "print.pdf";
            if (File.Exists(HttpContext.Current.Server.MapPath(fileName)))
                File.Delete(HttpContext.Current.Server.MapPath(fileName));
        

            if (File.Exists(HttpContext.Current.Server.MapPath(printName)))
                File.Delete(HttpContext.Current.Server.MapPath(printName));
            

            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
         FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(fileName),FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();

            //Open existing PDF
            //Document document = new Document(PageSize.LEGAL);
            PdfReader reader = new PdfReader(HttpContext.Current.Server.MapPath(fileName));

            //Getting a instance of new PDF writer
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(HttpContext.Current.Server.MapPath(printName), FileMode.Create));

            //PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, document.PageSize.Height, 0.75f);
            //https://developers.itextpdf.com/question/how-set-zoom-level-pdf-using-itextsharp

            document.Open();
            
            PdfContentByte cb = writer.DirectContent;

            int i = 0;
            int p = 0;
            int n = reader.NumberOfPages;
            iTextSharp.text.Rectangle psize = reader.GetPageSize(1);

            float width = psize.Width;
            float height = psize.Height;

            //Add Page to new document
            while (i < n)
            {
                document.NewPage();
                p++;
                i++;

                PdfImportedPage page1 = writer.GetImportedPage(reader, i);
                cb.AddTemplate(page1, 0, 0);
            }


            //Attach javascript to the document
            //PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
            //writer.SetOpenAction(action);

            PdfAction jAction = PdfAction.JavaScript("this.print(true);\r", writer);
            writer.AddJavaScript(jAction);
            document.Close();
        }

        public static void PageSetup(string rptOrgUrl, string newSavePath, string pageHeight, string pageWidth, string leftMargin, string rightMargin, string topMargin, string bottomMargin)
        {
            XmlDocument doc = new XmlDocument();
            string orgUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/" + rptOrgUrl));
            doc.Load(File.OpenRead(orgUrl));
            doc.GetElementsByTagName("PageHeight")[0].InnerText = pageHeight;
            doc.GetElementsByTagName("PageWidth")[0].InnerText = pageWidth;
            doc.GetElementsByTagName("LeftMargin")[0].InnerText = leftMargin;
            doc.GetElementsByTagName("RightMargin")[0].InnerText = rightMargin;
            doc.GetElementsByTagName("TopMargin")[0].InnerText = topMargin;
            doc.GetElementsByTagName("BottomMargin")[0].InnerText = bottomMargin;
            if (File.Exists(newSavePath))
                File.Delete(newSavePath);
            doc.Save(newSavePath);
        }



    }
}