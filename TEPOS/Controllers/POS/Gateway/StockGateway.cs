using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using ERP.Controllers.Manager;
using ERP.Models;
using ERP.Models.POS;
using ERP.Models.VModel;

namespace ERP.Controllers.POS.Gateway
{
    public class StockGateway
    {
        //sara 23-12-2024
        public List<PosVwPurchaseReceipt> GetPurchaseReceiptByinvNo(string invReferenceNo, int cmnId ,string BranchName)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                return database.PosVwPurchaseReceiptDbSet
                    .Where(w => w.InvReferenceNo == invReferenceNo && w.CmnCompanyId == cmnId && w.BranchName== BranchName)
                    .ToList();
            }
        }

        public bool DeletePurchaseReceiptById(int invoiceId, int cmnId)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                var invoice = database.PosVwPurchaseReceiptDbSet
                    .FirstOrDefault(w => w.Id == invoiceId && w.CmnCompanyId == cmnId);

                if (invoice != null)
                {
                    database.PosVwPurchaseReceiptDbSet.Remove(invoice);
                    database.SaveChanges();
                    return true; 
                }

                return false; 
            }
        }
        // sara 
        public dynamic GetInvoiceByType(int invoiceType, ErpManager erpM)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                int cmnId = erpM.CmnId;
                int usrId = erpM.UserId;
                return database.PosStockDbSet
                    .Where(w => w.PosInvoiceType == (PosInvoiceType)invoiceType
                    && w.CmnCompanyId == cmnId &&  w.PosBranch.Name == erpM.BranchName
                    && w.PosInvoiceType != PosInvoiceType.Cancel).Select(s => new
                {
                    Id = s.InvReferenceNo,
                    Name = s.InvReferenceNo

                }).ToList();
            }

        }

        public List<PosVwPurchaseReceipt> GetPurchaseReceipt( string companyInvNo,int cmnId, string BranchName)
        {
            using (ConnectionDatabase database=new ConnectionDatabase())
            {
                return database.PosVwPurchaseReceiptDbSet.Where(w => w.CompanyInvNo == companyInvNo && w.CmnCompanyId==cmnId && w.BranchName == BranchName).ToList();
            }
        }

        public List<PosVwProductWithBatch> GetProductInfoForPurchasReceipt(int cmnId)
        {
            using (ConnectionDatabase database=new ConnectionDatabase())
            {
                return database.PosVwProductWithBatchDbSet.Where(w => w.CmnCompanyId == cmnId).ToList();
            }
        }
        public List<PosVwProductWithBatch> GetProductInfoForPurchasReceiptByItem(int cmnId  ,string Parcode,string Name)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                return database.PosVwProductWithBatchDbSet.Where(w => w.CmnCompanyId == cmnId & (w.BarCode.Contains(Parcode) ||w.Name.Contains(Name))).ToList();
            }
        }


        public VmProductDetails GetProductDetailsForPurchaseReceipt(int cmnId,string productCode)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                var lst = (from l in database.PosProductBatchDbSet
                    where l.CmnCompanyId == cmnId && (l.PosProduct.Code==productCode || l.BarCode== productCode)
                    orderby l.DateTo descending
                    select new VmProductDetails
                    {
                        Code = l.PosProduct.Code,
                        PosUomMasterId = l.PosProduct.PosUomGroup.PosUomGroupDetails.FirstOrDefault(f => f.PosUomGroupId == l.PosProduct.PosUomGroupId && f.PosUomMaster.IsBaseUom == true).PosUomMasterId,
                        PosUomGroupId = l.PosProduct.PosUomGroupId,
                        PosProductBatchId = l.Id,
                        PurchaseRate = l.PurchaseRate,
                        SellingRate=l.SellingRate,
                        DateTo = l.DateTo.ToString(),
                        Name = l.PosProduct.Name,
                        PosProductHasExpire=l.PosProduct.PosProductHasExpire

                    }).FirstOrDefault();
                return lst;
            }
        }

        public VmProductDetailsForStockTransfer GetProductDetailsForStockTransfer(int cmnId, string productCode,int fromLocation,int officeId)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                var lst = (from l in database.PosProductBatchDbSet
                           where l.CmnCompanyId == cmnId && (l.PosProduct.Code == productCode || l.BarCode==productCode)
                           orderby l.SellingRate descending
                           select new VmProductDetailsForStockTransfer
                           {
                               Code = l.PosProduct.Code,
                               PosProductBatchId = l.Id,
                               PurchaseRate = l.PurchaseRate,
                               SellingRate=l.SellingRate,
                               Name = l.PosProduct.Name,
                               StockOnHand= 0
                           }).FirstOrDefault();
                if (lst != null)
                {
                    lst.StockOnHand = database.Database.SqlQuery<VmGetPosCurrentStockByLocationOnly>("GetPosCurrentStockByLocationOnly @OfficeId='" + officeId.ToString() + "',@Location=" + fromLocation + ",@BatchId=" + lst.PosProductBatchId + ",@CmnId=" + cmnId).Sum(s => s.StockQty);
                }
                return lst;
            }
        }
     

        public decimal GetStockByLocationWise(long batchId, int officeId, int fromLocation,int cmnId)
        {
            using (ConnectionDatabase database =new ConnectionDatabase())
            {
                return database.Database.SqlQuery<VmGetPosCurrentStockByLocationOnly>("GetPosCurrentStockByLocationOnly @OfficeId='" + officeId.ToString() + "',@Location=" + fromLocation + ",@BatchId=" + batchId + ",@CmnId=" + cmnId).Sum(s => s.StockQty);
            }
        }

    }
}