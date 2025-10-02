using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using ERP.Controllers.Manager;
using ERP.Models;
using ERP.Models.POS;
using ERP.Models.VModel;
using Newtonsoft.Json;
using ERP.Controllers.POS.Gateway;

namespace ERP.Controllers.POS.Manager
{
    public class SchemeManager
    {

        public dynamic GetFreeAndDiscountItem(DateTime date, List<VmBilling> vmBilling, long customerNo, int branchd)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                decimal combiDiscount = 0;

                vmBilling = vmBilling.GroupBy(g => new { g.PosProductId, g.PosProductBatchId }).Select(s => new VmBilling
                {
                    PosProductId=s.First().PosProductId,
                    PosProductBatchId=s.First().PosProductBatchId,
                    Qty = s.Sum(ss => ss.Qty),
                    Amount = s.Sum(a => a.Amount)

                }).ToList();
                List<VmSingleSchemeDiscount> singleSchemeDiscount = new List<VmSingleSchemeDiscount>();
                List<VmSchemeFreeProduct> schemeFreeProduct = new List<VmSchemeFreeProduct>();
               var customerLst = database.PosCustomerDbSet.Where(f =>f.Id ==customerNo).Select(s =>new { Id=s.Id,s.PosCustomerClassId}).FirstOrDefault();
                if (customerLst == null)
                {
                    return "customerNotFound";
                }

                if (vmBilling.Any())
                {

                    int customerClassId = customerLst.PosCustomerClassId;

                    //get all billing items avaiable scheme
                    long[] billProduct = vmBilling.Select(s => s.PosProductId).ToArray();
                    long[] billProductBatch = vmBilling.Select(s => s.PosProductBatchId).ToArray();
                    var getSchemeableProduct = (from sp in database.PosSchemeProductDbSet
                        join scc in database.PosSchemeCustomerClassDbSet on sp.PosSchemeId equals scc.PosSchemeId
                        join sb in database.PosSchemeBranchDbSet on sp.PosSchemeId equals sb.PosSchemeId
                        where billProduct.Contains(sp.PosProductId)
                              && billProductBatch.Contains(sp.PosProductBatchId)
                              && date>= sp.PosScheme.DateFrom
                              && date<=sp.PosScheme.DateTo 
                              && scc.PosCustomerClassId == customerClassId
                              && sb.PosBranchId == branchd
                        select new
                        {
                            sp.PosSchemeId,
                            sp.PosScheme.IsCombiScheme,
                            sp.PosProductId,
                            sp.PosProductBatchId,
                            sp.PosScheme.SchemeType
                        }).ToList();
                    if (getSchemeableProduct.Any())
                    {
                        //update product rate
                        foreach (var billing in vmBilling)
                        {
                            if (database.PosProductDbSet.Any(a => a.Id == billing.PosProductId && a.IsPriceChangeable == false))
                            {
                                billing.Amount = database.PosProductBatchDbSet.Where(w => w.Id == billing.PosProductBatchId && w.PosProductId == billing.PosProductId).Select(s => s.SellingRate).DefaultIfEmpty(0).First()*billing.Qty;
                            }
                        }

                        //list of scheme ids
                        List<long> listOfSchemeId = getSchemeableProduct.Select(s => s.PosSchemeId).Distinct().ToList();

                        //scheme slab
                        var getAllSchemeSlab = database.PosSchemeSlabDbSet.Where(w => listOfSchemeId.Contains(w.PosSchemeId)).ToList();

                        //scheme slab free item
                        List<long> schemeSlabId = getAllSchemeSlab.Select(s => s.Id).ToList();
                        var getAllSchemeSlabFreeProduct = database.PosSchemeSlabFreeProductDbSet.Where(w => schemeSlabId.Contains(w.PosSchemeSlabId)).ToList();

                        var getAllSingleScheme = getSchemeableProduct.Where(w => !w.IsCombiScheme).Select(s => s.PosSchemeId);

                        foreach (var bl in vmBilling.Where(w=> getSchemeableProduct.Select(s=>s.PosProductId).Contains(w.PosProductId) && getSchemeableProduct.Select(s => s.PosProductBatchId).Contains(w.PosProductBatchId)))
                        {
                            //for single product free/discount
                            foreach (var ss in getAllSchemeSlab.Where(w => getAllSingleScheme.Contains(w.PosSchemeId)).OrderByDescending(o => o.PurQtyOrAmt))
                            {
                                //discount or free based on qty
                                if (getSchemeableProduct.Any(a => a.PosSchemeId == ss.PosSchemeId && a.SchemeType == 1))
                                {
                                    if (bl.Qty >= ss.PurQtyOrAmt)
                                    {
                                        //discount
                                        int mltBy = (int) Math.Floor(bl.Qty/ss.PurQtyOrAmt);
//decimal discount = ((bl.Amount*ss.DiscountPer)/100)*mltBy; abdo
                                        //abdo
                                        //decimal discount = ((bl.Amount * ss.DiscountPer) / 100); case when using minimum qty for discount
                                        decimal discount = ((((bl.Amount / bl.Qty) * ss.PurQtyOrAmt) * ss.DiscountPer) / 100) * mltBy; //case when using multiples of minimum qty
                                        //abdo

                                        //flat amount
                                        discount += (ss.FlatAmt*mltBy);
                                        if (discount > 0)
                                        {
                                            VmSingleSchemeDiscount objDiscount = new VmSingleSchemeDiscount();
                                            objDiscount.PosProductId = bl.PosProductId;
                                            objDiscount.PosProductBatchId = bl.PosProductBatchId;
                                            objDiscount.DiscountAmount = discount;
                                            singleSchemeDiscount.Add(objDiscount);
                                        }

                                        //free product
                                        foreach (var fp in getAllSchemeSlabFreeProduct.Where(w => w.PosSchemeSlabId == ss.Id))
                                        {
                                            VmSchemeFreeProduct objFreeProduct = new VmSchemeFreeProduct();
                                            var freeProductInfo = database.PosVwProductWithBatchDbSet.Where(f => f.Id == fp.PosProductBatchId && f.PosProductId == fp.PosProductId).Select(s => new { s.Name, s.Code, s.BatchName }).First();
                                            objFreeProduct.Batch = freeProductInfo.BatchName;
                                            objFreeProduct.Code = freeProductInfo.Code;
                                            objFreeProduct.Name = freeProductInfo.Name;
                                            if (fp.Availability == 1)
                                            {
                                                //free by Or 
                                                objFreeProduct.PosProductId = fp.PosProductId;
                                                objFreeProduct.PosProductBatchId = fp.PosProductBatchId;
                                                objFreeProduct.Qty = (fp.Qty*mltBy);
                                                schemeFreeProduct.Add(objFreeProduct);
                                                break;
                                            }
                                            else
                                            {
                                                //free by and 
                                                objFreeProduct.PosProductId = fp.PosProductId;
                                                objFreeProduct.PosProductBatchId = fp.PosProductBatchId;
                                                objFreeProduct.Qty = (fp.Qty*mltBy);
                                                schemeFreeProduct.Add(objFreeProduct);
                                            }

                                        }
                                        break;
                                    }
                                }
                                else
                                {
                                    //discount or free based on amount
                                    if (bl.Amount >= ss.PurQtyOrAmt)
                                    {
                                        //discount
                                        int mltBy = (int) Math.Floor(bl.Amount/ss.PurQtyOrAmt);
                                        decimal discount = ((bl.Amount*ss.DiscountPer)/100)*mltBy;

                                        //flat amount
                                        discount += (ss.FlatAmt*mltBy);

                                        VmSingleSchemeDiscount objDiscount = new VmSingleSchemeDiscount();
                                        objDiscount.PosProductId = bl.PosProductId;
                                        objDiscount.PosProductBatchId = bl.PosProductBatchId;
                                        objDiscount.DiscountAmount = discount;
                                        singleSchemeDiscount.Add(objDiscount);


                                        //free product
                                        foreach (var fp in getAllSchemeSlabFreeProduct.Where(w => w.PosSchemeSlabId == ss.Id))
                                        {
                                            VmSchemeFreeProduct objFreeProduct = new VmSchemeFreeProduct();
                                            var freeProductInfo = database.PosVwProductWithBatchDbSet.Where(f => f.Id == fp.PosProductBatchId && f.PosProductId == fp.PosProductId).Select(s => new { s.Name, s.Code, s.BatchName }).First();
                                            objFreeProduct.Batch = freeProductInfo.BatchName;
                                            objFreeProduct.Code = freeProductInfo.Code;
                                            objFreeProduct.Name = freeProductInfo.Name;
                                            if (fp.Availability == 1)
                                            {
                                                //free by Or 
                                                objFreeProduct.PosProductId = fp.PosProductId;
                                                objFreeProduct.PosProductBatchId = fp.PosProductBatchId;
                                                objFreeProduct.Qty = (fp.Qty*mltBy);
                                                schemeFreeProduct.Add(objFreeProduct);
                                                break;
                                            }
                                            else
                                            {
                                                //free by and 
                                                objFreeProduct.PosProductId = fp.PosProductId;
                                                objFreeProduct.PosProductBatchId = fp.PosProductBatchId;
                                                objFreeProduct.Qty = (fp.Qty*mltBy);
                                                schemeFreeProduct.Add(objFreeProduct);
                                            }

                                        }
                                        break;
                                    }
                                }

                            }
                        }


                        //for combi product free/discount
                        var getAllCombiScheme = getSchemeableProduct.Where(w => w.IsCombiScheme).ToList();
                        foreach (long cs in getAllCombiScheme.Select(s => s.PosSchemeId).Distinct())
                        {
                            var cmbProductLst = getAllCombiScheme.Where(w => w.PosSchemeId == cs).Select(s => new {s.PosProductId, s.PosProductBatchId}).ToList();
                            //check  combi scheme avaiable or not
                            var bilingItem = vmBilling.Where(a => cmbProductLst.Select(s => s.PosProductId).Contains(a.PosProductId) && cmbProductLst.Select(s => s.PosProductBatchId).Contains(a.PosProductBatchId)).ToList();
                            if (bilingItem.Any())
                            {
                                foreach (var ss in getAllSchemeSlab.Where(w => w.PosSchemeId == cs).OrderByDescending(o => o.PurQtyOrAmt))
                                {
                                    //discount or free based on qty
                                    if (getSchemeableProduct.Any(a => a.PosSchemeId == ss.PosSchemeId && a.SchemeType == 1))
                                    {
                                        long sumOfQty = bilingItem.Select(s => s.Qty).Sum();
                                        if (sumOfQty >= ss.PurQtyOrAmt)
                                        {
                                            //discount
                                            int mltBy = (int) Math.Floor(sumOfQty/ss.PurQtyOrAmt);
                                            decimal discount = ((sumOfQty*ss.DiscountPer)/100)*mltBy;

                                            //flat amount
                                            discount += (ss.FlatAmt*mltBy);
                                            combiDiscount += discount;

                                            //free product
                                            foreach (var fp in getAllSchemeSlabFreeProduct.Where(w => w.PosSchemeSlabId == ss.Id))
                                            {
                                                VmSchemeFreeProduct objFreeProduct = new VmSchemeFreeProduct();
                                                var freeProductInfo = database.PosVwProductWithBatchDbSet.Where(f => f.Id == fp.PosProductBatchId && f.PosProductId == fp.PosProductId).Select(s => new { s.Name ,s.Code,s.BatchName}).First();
                                                objFreeProduct.Batch = freeProductInfo.BatchName;
                                                objFreeProduct.Code = freeProductInfo.Code;
                                                objFreeProduct.Name = freeProductInfo.Name;

                                                if (fp.Availability == 1)
                                                {
                                                    //free by Or 
                                                    objFreeProduct.PosProductId = fp.PosProductId;
                                                    objFreeProduct.PosProductBatchId = fp.PosProductBatchId;
                                                    objFreeProduct.Qty = (fp.Qty*mltBy);
                                                    schemeFreeProduct.Add(objFreeProduct);
                                                    break;
                                                }
                                                else
                                                {
                                                    //free by and 
                                                    objFreeProduct.PosProductId = fp.PosProductId;
                                                    objFreeProduct.PosProductBatchId = fp.PosProductBatchId;
                                                    objFreeProduct.Qty = (fp.Qty*mltBy);
                                                    schemeFreeProduct.Add(objFreeProduct);
                                                }

                                            }
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        //free or discount based on amount
                                        decimal sumOfAmount = bilingItem.Select(s => s.Amount).Sum();
                                        if (sumOfAmount >= ss.PurQtyOrAmt)
                                        {
                                            //discount
                                            int mltBy = (int) Math.Floor(sumOfAmount/ss.PurQtyOrAmt);
                                            decimal discount = ((sumOfAmount*ss.DiscountPer)/100)*mltBy;

                                            //flat amount
                                            discount += (ss.FlatAmt*mltBy);
                                            combiDiscount += discount;

                                            //free product
                                            foreach (var fp in getAllSchemeSlabFreeProduct.Where(w => w.PosSchemeSlabId == ss.Id))
                                            {
                                                VmSchemeFreeProduct objFreeProduct = new VmSchemeFreeProduct();
                                                var freeProductInfo = database.PosVwProductWithBatchDbSet.Where(f => f.Id == fp.PosProductBatchId && f.PosProductId == fp.PosProductId).Select(s => new { s.Name, s.Code, s.BatchName }).First();
                                                objFreeProduct.Batch = freeProductInfo.BatchName;
                                                objFreeProduct.Code = freeProductInfo.Code;
                                                objFreeProduct.Name = freeProductInfo.Name;

                                                if (fp.Availability == 1)
                                                {
                                                    //free by Or 
                                                    objFreeProduct.PosProductId = fp.PosProductId;
                                                    objFreeProduct.PosProductBatchId = fp.PosProductBatchId;
                                                    objFreeProduct.Qty = (fp.Qty*mltBy);
                                                    schemeFreeProduct.Add(objFreeProduct);
                                                    break;
                                                }
                                                else
                                                {
                                                    //free by and 
                                                    objFreeProduct.PosProductId = fp.PosProductId;
                                                    objFreeProduct.PosProductBatchId = fp.PosProductBatchId;
                                                    objFreeProduct.Qty = (fp.Qty*mltBy);
                                                    schemeFreeProduct.Add(objFreeProduct);
                                                }

                                            }
                                            break;
                                        }
                                    }

                                }
                            }

                        }
                    }
                }
                var rtr = new
                {
                    CombiDiscount = combiDiscount,
                    SingleSchemeDiscount = singleSchemeDiscount,
                    SchemeFreeProduct = schemeFreeProduct
                };
                return rtr;
            }
        }

        public long SaveScheme(string saveData, ErpManager erpManager)
        {

            var postData = JsonConvert.DeserializeObject<VmPosScheme>(saveData);
            SchemeGateway schemeGateway = new SchemeGateway();
            return schemeGateway.SaveScheme(postData, erpManager);
            
        }

        public long UpdateScheme(string saveData, ErpManager erpManager)
        {

            var postData = JsonConvert.DeserializeObject<VmPosScheme>(saveData);
            SchemeGateway schemeGateway = new SchemeGateway();
            return schemeGateway.UpdateScheme(postData, erpManager);

        }


    }
}