using ERP.Controllers.Manager;
using ERP.Models;
using ERP.Models.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Models.VModel;
using ERP.CSharpLib;
using System.Data.Entity;

namespace ERP.Controllers.POS.Gateway
{
    public class SchemeGateway
    {
        public IEnumerable<PosScheme> GetScheme(int cmnId)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                var lst = database.PosSchemeDbSet.Where(w => w.CmnCompanyId == cmnId).OrderByDescending(o => o.DateFrom).ToList();
                return lst;
            }
        }

        public long SaveScheme(VmPosScheme posSchemeData, ErpManager erpManager)
        {

            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                using (var scope = database.Database.BeginTransaction())
                {
                    //Save Scheme
                    PosScheme posScheme = new PosScheme();
                    posScheme.SchemeCode = database.GetSeqPosSchemeSchemeCode();
                    posScheme.Description = posSchemeData.Description;
                    posScheme.IsCombiScheme = posSchemeData.IsCombiScheme;
                    posScheme.SchemeType = posSchemeData.SchemeType;
                    posScheme.Status = posSchemeData.Status;
                    posScheme.DateFrom = posSchemeData.DateFrom;
                    posScheme.DateTo = posSchemeData.DateTo;
                    posScheme.CmnCompanyId = erpManager.CmnId;
                    posScheme.CreatedBy = erpManager.UserId;
                    posScheme.CreatedDate = UTCDateTime.BDDateTime();
                    database.PosSchemeDbSet.Add(posScheme);
                    database.SaveChanges();

                    //Save Scheme branch
                    List<PosSchemeBranch> listSchemeBranch = new List<PosSchemeBranch>();
                    foreach (var sBranch in posSchemeData.PosBranch)
                    {
                        PosSchemeBranch posSchemeBranch = new PosSchemeBranch();
                        posSchemeBranch.PosSchemeId = posScheme.Id;
                        posSchemeBranch.PosBranchId = sBranch;
                        listSchemeBranch.Add(posSchemeBranch);
                    }
                    database.PosSchemeBranchDbSet.AddRange(listSchemeBranch);
                    database.SaveChanges();

                    //Save Scheme customer
                    List<PosSchemeCustomerClass> listSchemeCustomer = new List<PosSchemeCustomerClass>();
                    foreach (var sClass in posSchemeData.CusClass)
                    {
                        PosSchemeCustomerClass posSchemeCustomer = new PosSchemeCustomerClass();
                        posSchemeCustomer.PosSchemeId = posScheme.Id;
                        posSchemeCustomer.PosCustomerClassId = sClass;
                        listSchemeCustomer.Add(posSchemeCustomer);
                    }
                    database.PosSchemeCustomerClassDbSet.AddRange(listSchemeCustomer);
                    database.SaveChanges();

                    //Save Scheme Product
                    List<PosSchemeProduct> listSchemeProduct = new List<PosSchemeProduct>();
                    foreach (var sProduct in posSchemeData.PosProducts)
                    {
                        PosSchemeProduct posSchemeProduct = new PosSchemeProduct();
                        posSchemeProduct.PosSchemeId = posScheme.Id;
                        posSchemeProduct.PosProductId = sProduct.ProductId;
                        posSchemeProduct.PosProductBatchId = sProduct.ProductBatchId;
                        listSchemeProduct.Add(posSchemeProduct);
                    }
                    database.PosSchemeProductDbSet.AddRange(listSchemeProduct);
                    database.SaveChanges();

                    //Save Scheme Slab
                    List<PosSchemeSlab> listSchemeSlab = new List<PosSchemeSlab>();
                    foreach (var sSlab in posSchemeData.PosSchemeSlab)
                    {
                        PosSchemeSlab posSchemeSlab = new PosSchemeSlab();
                        posSchemeSlab.PosSchemeId = posScheme.Id;
                        posSchemeSlab.PurQtyOrAmt = sSlab.PurQtyOrAmt;
                        posSchemeSlab.DiscountPer = sSlab.DiscountPer;
                        posSchemeSlab.FlatAmt = sSlab.FlatAmt;
                        database.PosSchemeSlabDbSet.Add(posSchemeSlab);
                        database.SaveChanges();
                        //Save Scheme Slab Free
                        List<PosSchemeSlabFreeProduct> listSchemeSlabFree = new List<PosSchemeSlabFreeProduct>();
                        if (sSlab.Products != null)
                        {
                            foreach (var sSlabFree in sSlab.Products)
                            {
                                PosSchemeSlabFreeProduct posSchemeSlabFree = new PosSchemeSlabFreeProduct();
                                posSchemeSlabFree.PosSchemeId = posScheme.Id;
                                posSchemeSlabFree.PosSchemeSlabId = posSchemeSlab.Id;
                                posSchemeSlabFree.PosProductId = sSlabFree.PosProductId;
                                posSchemeSlabFree.PosProductBatchId = sSlabFree.PosProductBatchId;
                                posSchemeSlabFree.Qty = sSlabFree.Qty;
                                posSchemeSlabFree.PosUomMasterId = sSlabFree.PosUomMasterId;
                                posSchemeSlabFree.Availability = sSlabFree.Availability;
                                listSchemeSlabFree.Add(posSchemeSlabFree);
                            }
                            database.PosSchemeSlabFreeProductDbSet.AddRange(listSchemeSlabFree);
                            database.SaveChanges();
                        }
                    }
                    scope.Commit();
                    return posScheme.Id;
                }
            }
        }

        public long UpdateScheme(VmPosScheme posSchemeData, ErpManager erpManager)
        {

            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                using (var scope = database.Database.BeginTransaction())
                {
                    //Update Scheme
                    PosScheme posScheme = database.PosSchemeDbSet.Where(w => w.Id == posSchemeData.Id).First();
                    posScheme.Description = posSchemeData.Description;
                    posScheme.IsCombiScheme = posSchemeData.IsCombiScheme;
                    posScheme.SchemeType = posSchemeData.SchemeType;
                    posScheme.Status = posSchemeData.Status;
                    posScheme.DateFrom = posSchemeData.DateFrom;
                    posScheme.DateTo = posSchemeData.DateTo;
                    posScheme.CmnCompanyId = erpManager.CmnId;
                    posScheme.CreatedBy = erpManager.UserId;
                    posScheme.CreatedDate = UTCDateTime.BDDateTime();
                    database.PosSchemeDbSet.Add(posScheme);
                    database.Entry(posScheme).State = EntityState.Modified;
                    database.SaveChanges();

                    //Delete Old Data
                    List<PosSchemeSlabFreeProduct> psfp = new List<PosSchemeSlabFreeProduct>();
                    psfp = database.PosSchemeSlabFreeProductDbSet.Where(w => w.PosSchemeId == posSchemeData.Id).ToList();
                    database.PosSchemeSlabFreeProductDbSet.RemoveRange(psfp);
                    database.SaveChanges();

                    List<PosSchemeSlab> pss = new List<PosSchemeSlab>();
                    pss = database.PosSchemeSlabDbSet.Where(w => w.PosSchemeId == posSchemeData.Id).ToList();
                    database.PosSchemeSlabDbSet.RemoveRange(pss);
                    database.SaveChanges();

                    List<PosSchemeProduct> psp = new List<PosSchemeProduct>();
                    psp = database.PosSchemeProductDbSet.Where(w => w.PosSchemeId == posSchemeData.Id).ToList();
                    database.PosSchemeProductDbSet.RemoveRange(psp);
                    database.SaveChanges();

                    List<PosSchemeBranch> psb = new List<PosSchemeBranch>();
                    psb = database.PosSchemeBranchDbSet.Where(w => w.PosSchemeId == posSchemeData.Id).ToList();
                    database.PosSchemeBranchDbSet.RemoveRange(psb);
                    database.SaveChanges();

                    List<PosSchemeCustomerClass> psc = new List<PosSchemeCustomerClass>();
                    psc = database.PosSchemeCustomerClassDbSet.Where(w => w.PosSchemeId == posSchemeData.Id).ToList();
                    database.PosSchemeCustomerClassDbSet.RemoveRange(psc);
                    database.SaveChanges();

                    //Save Scheme branch
                    List<PosSchemeBranch> listSchemeBranch = new List<PosSchemeBranch>();
                    foreach (var sBranch in posSchemeData.PosBranch)
                    {
                        PosSchemeBranch posSchemeBranch = new PosSchemeBranch();
                        posSchemeBranch.PosSchemeId = posScheme.Id;
                        posSchemeBranch.PosBranchId = sBranch;
                        listSchemeBranch.Add(posSchemeBranch);
                    }
                    database.PosSchemeBranchDbSet.AddRange(listSchemeBranch);
                    database.SaveChanges();

                    //Save Scheme customer
                    List<PosSchemeCustomerClass> listSchemeCustomer = new List<PosSchemeCustomerClass>();
                    foreach (var sClass in posSchemeData.CusClass)
                    {
                        PosSchemeCustomerClass posSchemeCustomer = new PosSchemeCustomerClass();
                        posSchemeCustomer.PosSchemeId = posScheme.Id;
                        posSchemeCustomer.PosCustomerClassId = sClass;
                        listSchemeCustomer.Add(posSchemeCustomer);
                    }
                    database.PosSchemeCustomerClassDbSet.AddRange(listSchemeCustomer);
                    database.SaveChanges();

                    //Save Scheme Product
                    List<PosSchemeProduct> listSchemeProduct = new List<PosSchemeProduct>();
                    foreach (var sProduct in posSchemeData.PosProducts)
                    {
                        PosSchemeProduct posSchemeProduct = new PosSchemeProduct();
                        posSchemeProduct.PosSchemeId = posScheme.Id;
                        posSchemeProduct.PosProductId = sProduct.ProductId;
                        posSchemeProduct.PosProductBatchId = sProduct.ProductBatchId;
                        listSchemeProduct.Add(posSchemeProduct);
                    }
                    database.PosSchemeProductDbSet.AddRange(listSchemeProduct);
                    database.SaveChanges();

                    //Save Scheme Slab
                    List<PosSchemeSlab> listSchemeSlab = new List<PosSchemeSlab>();
                    foreach (var sSlab in posSchemeData.PosSchemeSlab)
                    {
                        PosSchemeSlab posSchemeSlab = new PosSchemeSlab();
                        posSchemeSlab.PosSchemeId = posScheme.Id;
                        posSchemeSlab.PurQtyOrAmt = sSlab.PurQtyOrAmt;
                        posSchemeSlab.DiscountPer = sSlab.DiscountPer;
                        posSchemeSlab.FlatAmt = sSlab.FlatAmt;
                        database.PosSchemeSlabDbSet.Add(posSchemeSlab);
                        database.SaveChanges();
                        //Save Scheme Slab Free                            
                        List<PosSchemeSlabFreeProduct> listSchemeSlabFree = new List<PosSchemeSlabFreeProduct>();
                        if (sSlab.Products != null)
                        {
                            foreach (var sSlabFree in sSlab.Products)
                            {
                                PosSchemeSlabFreeProduct posSchemeSlabFree = new PosSchemeSlabFreeProduct();
                                posSchemeSlabFree.PosSchemeId = posScheme.Id;
                                posSchemeSlabFree.PosSchemeSlabId = posSchemeSlab.Id;
                                posSchemeSlabFree.PosProductId = sSlabFree.PosProductId;
                                posSchemeSlabFree.PosProductBatchId = sSlabFree.PosProductBatchId;
                                posSchemeSlabFree.Qty = sSlabFree.Qty;
                                posSchemeSlabFree.PosUomMasterId = sSlabFree.PosUomMasterId;
                                posSchemeSlabFree.Availability = sSlabFree.Availability;
                                listSchemeSlabFree.Add(posSchemeSlabFree);
                            }
                            database.PosSchemeSlabFreeProductDbSet.AddRange(listSchemeSlabFree);
                            database.SaveChanges();
                        }
                    }
                    scope.Commit();
                    return posScheme.Id;
                }
            }

        }

        public long DeleteScheme(long schemeId, ErpManager erpManager)
        {


            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                PosScheme chk = new PosScheme();
                int comId = erpManager.CmnId;
                chk = database.PosSchemeDbSet.Where(w => w.Id == schemeId && w.CmnCompanyId == comId).First();

                if (chk == null)
                    return 0;
            }
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                using (var scope = database.Database.BeginTransaction())
                {
                    List<PosSchemeSlabFreeProduct> psfp = new List<PosSchemeSlabFreeProduct>();
                    psfp = database.PosSchemeSlabFreeProductDbSet.Where(w => w.PosSchemeId == schemeId).ToList();
                    database.PosSchemeSlabFreeProductDbSet.RemoveRange(psfp);
                    database.SaveChanges();

                    List<PosSchemeSlab> pss = new List<PosSchemeSlab>();
                    pss = database.PosSchemeSlabDbSet.Where(w => w.PosSchemeId == schemeId).ToList();
                    database.PosSchemeSlabDbSet.RemoveRange(pss);
                    database.SaveChanges();

                    List<PosSchemeProduct> psp = new List<PosSchemeProduct>();
                    psp = database.PosSchemeProductDbSet.Where(w => w.PosSchemeId == schemeId).ToList();
                    database.PosSchemeProductDbSet.RemoveRange(psp);
                    database.SaveChanges();

                    List<PosSchemeBranch> psb = new List<PosSchemeBranch>();
                    psb = database.PosSchemeBranchDbSet.Where(w => w.PosSchemeId == schemeId).ToList();
                    database.PosSchemeBranchDbSet.RemoveRange(psb);
                    database.SaveChanges();

                    List<PosSchemeCustomerClass> psc = new List<PosSchemeCustomerClass>();
                    psc = database.PosSchemeCustomerClassDbSet.Where(w => w.PosSchemeId == schemeId).ToList();
                    database.PosSchemeCustomerClassDbSet.RemoveRange(psc);
                    database.SaveChanges();

                    PosScheme ps = database.PosSchemeDbSet.Where(w => w.Id == schemeId).First();
                    database.PosSchemeDbSet.Remove(ps);
                    database.SaveChanges();
                    scope.Commit();
                    return 1;
                }

            }

        }

        public VmPosScheme GetSchemeById(int schemeId, ErpManager erpManager)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {

                int comId = erpManager.CmnId;
                PosScheme chk = database.PosSchemeDbSet.FirstOrDefault(w => w.Id == schemeId && w.CmnCompanyId == comId);

                if (chk == null)
                    return null;
            }
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                var rawScheme = database.PosSchemeDbSet.First(w => w.Id == schemeId);
                VmPosScheme posScheme = new VmPosScheme()
                {
                    Id = rawScheme.Id,
                    SchemeCode = rawScheme.SchemeCode,
                    Description = rawScheme.Description,
                    IsCombiScheme = rawScheme.IsCombiScheme,
                    SchemeType = rawScheme.SchemeType,
                    DateFrom = rawScheme.DateFrom,
                    DateTo = rawScheme.DateTo,
                    Status = rawScheme.Status,
                };

                posScheme.CusClass = database.PosSchemeCustomerClassDbSet.Where(w => w.PosSchemeId == schemeId).Select(s => s.PosCustomerClassId).ToList();
                posScheme.PosBranch = database.PosSchemeBranchDbSet.Where(w => w.PosSchemeId == schemeId).Select(s => s.PosBranchId).ToList();
                posScheme.PosProducts = database.PosSchemeProductDbSet.Where(w => w.PosSchemeId == schemeId).Select(s => new VmPosSchemePrd() {ProductId = s.PosProductId, ProductBatchId = s.PosProductBatchId}).ToList();
                posScheme.PosSchemeSlab = database
                    .PosSchemeSlabDbSet
                    .Where(w => w.PosSchemeId == schemeId)
                    .Select(s => new VmPosSchemePrdSchemeSlab()
                    {
                        PurQtyOrAmt = s.PurQtyOrAmt,
                        DiscountPer = s.DiscountPer,
                        FlatAmt = s.FlatAmt,
                        Products = database
                            .PosSchemeSlabFreeProductDbSet
                            .Where(ww => ww.PosSchemeSlabId == s.Id)
                            .Select(ss => new VmPosSchemePrdSchemeSlabFree()
                            {
                                PosProductId = ss.PosProductId,
                                PosProductName = ss.PosProduct.Name,
                                PosProductBatchName = ss.PosProductBatch.BatchName,
                                Availability = ss.Availability,
                                PosProductBatchId = ss.PosProductBatchId,
                                PosUomMasterId = ss.PosUomMasterId,
                                Qty = ss.Qty
                            })
                            .ToList()
                    })
                    .ToList();
                return posScheme;
            }
        }

    }
}