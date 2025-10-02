using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ERP.Controllers.Manager;
using ERP.CSharpLib;
using ERP.Models;
using ERP.Models.POS;
using ERP.Models.VModel;


namespace ERP.Controllers.POS.Gateway
{
    public class ItemRegisterGateway
    {

        public static int Delete(int productId)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                PosProduct posProduct = new PosProduct {Id = productId};
                database.Entry(posProduct).State = EntityState.Deleted;
                return database.SaveChanges();
            }
        }

        public static int Update(PosProduct posProduct, List<VmItemBatch> itemBatch, ErpManager erpManager)
        {
            int rtr = 0;
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                using (var tran = database.Database.BeginTransaction())
                {
                    database.Entry(posProduct).State = EntityState.Modified;
                    database.Entry(posProduct).Property(p => p.CmnCompanyId).IsModified = false;
                    database.Entry(posProduct).Property(p => p.CreatedDate).IsModified = false;
                    database.Entry(posProduct).Property(p => p.CreatedBy).IsModified = false;
                    database.SaveChanges();

                    var batchIdList = itemBatch.Select(s => s.Id).ToArray();
                   List<PosProductBatch> batchEditList = database.PosProductBatchDbSet.Where(w =>w.PosProductId==posProduct.Id && batchIdList.Contains(w.Id)).ToList();
                    if (batchEditList.Any())
                    {
                        foreach (var eb in batchEditList)
                        {
                            foreach (var nb in itemBatch)
                            {
                                if (eb.Id == nb.Id)
                                {
                                    eb.BatchName = nb.BatchName;
                                    eb.Mrp = nb.Mrp;
                                    eb.PurchaseRate = nb.PurchaseRate;
                                    eb.SellingRate = nb.SellingRate;
                                    eb.Weight = nb.Weight;
                                    eb.BarCode = nb.BarCode;
                                    eb.ModifideDate = UTCDateTime.BDDateTime();
                                    eb.ModifiedBy = erpManager.UserId;
                                }
                            }
                        }

                    }


                    List<PosProductBatch> batcDeleteList = database.PosProductBatchDbSet.Where(w =>w.PosProductId==posProduct.Id && !batchIdList.Contains(w.Id)).ToList();
                    if (batcDeleteList.Any())
                    {
                        foreach (var dl in batcDeleteList)
                        {
                            database.Entry(dl).State = EntityState.Deleted;
                        }
                    }

                    var lstAdd = itemBatch.Where(i => i.Id == 0).ToList();
                    if (lstAdd.Any())
                    {
                        List<PosProductBatch> listOfAddedproductBatch = new List<PosProductBatch>();
                        foreach (var ls in lstAdd)
                        {
                            PosProductBatch objBatch = new PosProductBatch();
                            objBatch.PosProductId = posProduct.Id;
                            objBatch.BatchName = ls.BatchName;
                            objBatch.Mrp = ls.Mrp;
                            objBatch.CmnCompanyId = erpManager.CmnId;
                            objBatch.PurchaseRate = ls.PurchaseRate;
                            objBatch.SellingRate = ls.SellingRate;
                            objBatch.Weight = ls.Weight;
                            objBatch.BarCode = ls.BarCode;
                            objBatch.CreatedBy = erpManager.UserId;
                            objBatch.CreatedDate = UTCDateTime.BDDateTime();
                            listOfAddedproductBatch.Add(objBatch);
                        }
                        database.PosProductBatchDbSet.AddRange(listOfAddedproductBatch);
                         
                    }
                    rtr += database.SaveChanges();
                    tran.Commit();
                    return rtr;
                }

            }
        }


        public static int Insert(PosProduct posProduct, List<VmItemBatch> itemBatch, ErpManager erpManager)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                using (var tran = database.Database.BeginTransaction())
                {
                    posProduct.CmnCompanyId = erpManager.CmnId;
                    posProduct.CreatedBy = erpManager.UserId;
                    posProduct.CreatedDate = UTCDateTime.BDDateTime();
                    database.PosProductDbSet.Add(posProduct);
                    database.SaveChanges();
                    List<PosProductBatch> productBatches = new List<PosProductBatch>();
                    foreach (var ls in itemBatch)
                    {
                        PosProductBatch objBatch = new PosProductBatch();
                        objBatch.PosProductId = posProduct.Id;
                        objBatch.BatchName = ls.BatchName;
                        objBatch.Mrp = ls.Mrp;
                        objBatch.CmnCompanyId = erpManager.CmnId;
                        objBatch.PurchaseRate = ls.PurchaseRate;
                        objBatch.SellingRate = ls.SellingRate;
                        objBatch.Weight = ls.Weight;
                        objBatch.BarCode = ls.BarCode;
                        objBatch.CreatedBy = erpManager.UserId;
                        objBatch.CreatedDate = UTCDateTime.BDDateTime();
                        productBatches.Add(objBatch);
                    }
                    database.PosProductBatchDbSet.AddRange(productBatches);
                    int rtr = database.SaveChanges();
                    tran.Commit();
                    return rtr;
                }
            }
        }

        public List<ViewModelItemBatch> GetItemBatch(int productId)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                int cmnId = new ErpManager().CmnId;
                var lst = (from l in database.PosProductBatchDbSet
                    where l.CmnCompanyId == cmnId && l.PosProductId==productId
                    select new ViewModelItemBatch
                    {
                      Id  = l.Id,
                       BatchName = l.BatchName,
                       Mrp = l.Mrp,PurchaseRate = l.PurchaseRate,
                      SellingRate = l.SellingRate,
                      DateFrom = l.DateFrom,DateTo = l.DateTo, Weight = l.Weight,BarCode = l.BarCode
                    }).ToList();
                return lst;
            }

        }

        public IEnumerable<dynamic> GetBarcodeByItemWise(int productId)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                int cmnId = new ErpManager().CmnId;
                var lst = (from l in database.PosProductBatchDbSet
                           where l.CmnCompanyId == cmnId && l.PosProductId == productId
                           select new
                           {
                               l.Id,
                               l.BatchName,
                               l.BarCode,
                               l.SellingRate
                               ,l.DateTo
                               ,l.PosProduct.Name                               
                           }).ToList();
                return lst;
            }

        }

        public IEnumerable<dynamic> GetUomDetailsByUomGroupId(int uomGroupId)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                int cmnId = new ErpManager().CmnId;
                var lst = (from l in database.PosUomGroupDetailDbSet.Include(e => e.PosUomGroup.Name)
                           where l.CmnCompanyId == cmnId && l.PosUomGroupId == uomGroupId
                           select new
                           {
                               l.PosUomMaster.UomCode,
                               l.PosUomMaster.UomDescription,
                               l.ConversionFactor,
                               IsBaseUom = l.PosUomMaster.IsBaseUom ? "Yes" : "No"
                           }).ToList();
                return lst;
            }

        }

        public IEnumerable<dynamic> GetItemList(int cmnId)
        {
            using (ConnectionDatabase database=new ConnectionDatabase())
            {
                var lst = (from ls in database.PosProductDbSet
                           where ls.CmnCompanyId==cmnId
                    select new
                    {
                        CategoryId = ls.PosProductCategoryId,
                        CategoryName = ls.ProductCategory.Name,
                        ClassId=ls.PosProductClassId,//Adel
                        ClassName=ls.ProductClass.NameEn,//Adel
                        GroupId = ls.PosProductGroupId,//Adel
                        GroupName = ls.ProductGroup.NameEn,//Adel
                        SourceId = ls.PosProductSourceId,//Adel
                        SourceName = ls.ProductSource.NameEn,//Adel
                        DescriptionId =ls.PosProductDescriptionId,//Adel
                        DescreptionName = ls.ProductDescription.NameEn,//Adel
                        PosProductId = ls.Id,
                        ls.Id,
                        ls.Code,
                        ls.CompanyCode,
                        ls.Name,
                        ls.ShortName,
                        ls.NameInOtherLang,
                        SupplierName= ls.PosSupplier.Name,
                        NumberOfBatch=ls.PosProductBatch.Count,
                        UomGroup=ls.PosUomGroup.Name,
                        ls.PosUomGroupId,
                        ls.PosSupplierId,
                        ls.Vat,
                        ls.Sd,
                        ls.IsHideToStock,
                        ls.IsPriceChangeable,
                        ls.PosProductScientificName,//Adel
                        ls.PosProductHasExpire,//Adel
                        ls.PosProductIsShortage,//Adel
                        ls.PosProductNote//Adel
                    }).ToList();
                return lst;
            }
        } 

    }
}