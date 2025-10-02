using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using ERP.Models;
using ERP.Models.Manager;

namespace ERP.Controllers.Manager
{

    public class UtilityManager
    {
        public static List<PosInvoiceType> PosInvoiceTypeNotInForSalesSide = new List<PosInvoiceType>
        {
         PosInvoiceType.Cancel,/*2 for cancel invoice*/
           PosInvoiceType.Hold,/*3 for hold invoice*/
            PosInvoiceType.StockTransfar, /*for stock branch/Office Transfer*/
            PosInvoiceType.CreditStockTransfer /*for cancel stock branch/Office Transfer */
        };
        public static List<PosInvoiceType> PosInvoiceTypeNotInForStockSide = new List<PosInvoiceType>
        {
           PosInvoiceType.Cancel/*2 for cancel invoice*/,
            PosInvoiceType.Hold/*3 for hold invoice*/
        };

        public static List<int> SalesPriceIncOrExcVat = new List<int>
        {
            1/*1 for Including vat*/,
            2/*3 for Excluding vat*/
        };


        /// <summary>
        /// get company image url from database company wise
        /// </summary>
        /// <returns></returns>
        public static string CompanyImageUrl
        {
            get
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {

                    int cmnId = new ErpManager().CmnId;

                    // ActionResult rtr = null;
                    var cmnImage = (from cmn in database.CompanyDbSet
                                    where cmn.Id == cmnId
                                    select new
                                    {
                                        cmn.Logo
                                    }).FirstOrDefault();
                    if (cmnImage != null)
                    {
                        return new Uri(HttpContext.Current.Server.MapPath("~" + cmnImage.Logo)).AbsoluteUri;
                    }
                    return "";

                }
            }
        }


        public static JsonResult JsonResultMax(JsonResult ret, JsonRequestBehavior allowGet)
        {
            var jsonMaxLengthResult = ret;
            jsonMaxLengthResult.MaxJsonLength = int.MaxValue;
            jsonMaxLengthResult.JsonRequestBehavior = allowGet;
            return jsonMaxLengthResult;
        }


        public static VmBarcodeAndQtySeparator BarcodeBatchAndQtySeparator(string barcode, string qtySeparator, string batchSeparator)
        {
            int length = barcode.Length;
            VmBarcodeAndQtySeparator barcodeAndQtySeparators = new VmBarcodeAndQtySeparator();
            string qty = null, batchName = null;


            for (int i = length; i >= 1; i--)
            {
                int ind = i - 1;
                if (barcode[ind].ToString() != qtySeparator)
                {
                    qty = barcode[ind] + qty;
                }
                else
                {
                    barcodeAndQtySeparators.Barcode = barcode.Remove(length - (qty.Length + 1));
                    break;
                }
            }
            if (barcodeAndQtySeparators.Barcode == null)
            {
                barcodeAndQtySeparators.Barcode = qty;
                qty = null;
            }
            int n;
            bool isNumVal = int.TryParse(qty, out n);
            barcodeAndQtySeparators.Qty = isNumVal ? n : 1;

            //for batch seperator
            string barcodeForBatchSep = barcodeAndQtySeparators.Barcode;
            string barCodeTemp = null;
            for (int i = barcodeForBatchSep.Length; i >= 1; i--)
            {
                int ind = i - 1;
                if (barcodeForBatchSep[ind].ToString() != batchSeparator)
                {
                    batchName = barcodeForBatchSep[ind] + batchName;
                }
                else
                {
                    barCodeTemp = barcodeForBatchSep.Remove(barcodeForBatchSep.Length - (batchName.Length + 1));
                    break;
                }
            }

            if (barCodeTemp == null)
            {
                barcodeAndQtySeparators.Barcode = batchName;
                barcodeAndQtySeparators.BatchName = null;
            }
            else
            {
                barcodeAndQtySeparators.BatchName = batchName;
                barcodeAndQtySeparators.Barcode = barCodeTemp;
            }


            return barcodeAndQtySeparators;
        }

    }

    
}