using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Controllers.Manager;
using ERP.Models;
using ERP.Models.Security.Authorization;

namespace ERP.Controllers.POS
{
    [HasAuthorization]
    public partial class PosDropDownController : BaseController
    {
        ErpManager _erpManager = new ErpManager();


        public ActionResult GetProductUom(string productCode)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    int cmnId = _erpManager.CmnId;
                    var lst = (from l in database.PosUomGroupDetailDbSet
                               where l.PosUomGroup.PosProducts.Any(w => w.PosUomGroupId == l.PosUomGroupId && w.Code == productCode) && l.CmnCompanyId == cmnId
                               select new
                               {
                                   Id = l.PosUomMasterId,
                                   Name = l.PosUomMaster.UomCode,
                                   l.ConversionFactor,
                                   l.PosUomGroupId,
                                   l.PosUomMaster.IsBaseUom
                               }).ToList();

                        rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }


        public ActionResult GetProducts()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    int cmnId = _erpManager.CmnId;
                    var lst = (from l in database.PosProductDbSet
                               orderby l.Name ascending
                               where l.CmnCompanyId == cmnId
                               select new
                               {
                                   l.Id,
                                   Name = l.Name,
                                   l.Code
                               }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetBatchByProductCode(string productCode)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    int cmnId = _erpManager.CmnId;
                    var lst = (from l in database.PosProductBatchDbSet
                               orderby l.SellingRate descending
                               where l.PosProduct.Code == productCode && l.CmnCompanyId == cmnId
                               select new
                               {
                                   l.Id,
                                   Name = l.BatchName
                               }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }


        public ActionResult GetStockType()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    int cmnId = _erpManager.CmnId;
                    var lst = (from l in database.PosStockTypeDbSet
                               orderby l.IsBaseStock descending
                               where l.CmnCompanyId == cmnId
                               select new
                               {
                                   l.Id,
                                   l.Name
                               }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }


        public ActionResult GetBranch()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    int cmnId = _erpManager.CmnId;
                    var lst = (from l in database.PosBrancheList
                               where l.CmnCompanyId == cmnId
                               select new
                               {
                                   l.Id,
                                   l.Name
                               }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetCustomerClass()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    int cmnId = _erpManager.CmnId;
                    var lst = (from l in database.PosCustomerClassDbSet
                               where l.CmnCompanyId == cmnId
                               select new
                               {
                                   l.Id,
                                   l.Name
                               }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetUomGroup()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    int cmnId = _erpManager.CmnId;
                    var lst = (from l in database.PosUomGroupDbSet
                               where l.CmnCompanyId == cmnId
                               select new
                               {
                                   l.Id,
                                   l.Name
                               }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }


        public ActionResult GetCategories()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    int cmnId = _erpManager.CmnId;
                    var lst = (from l in database.PosProductCategoryDbSet
                               where l.CmnCompanyId == cmnId
                               select new
                               {
                                   l.Id,
                                   l.Name
                               }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }
        //Adel
        public ActionResult GetProductClass()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    int cmnId = _erpManager.CmnId;
                    var lst = (from l in database.PosProductClassDbSet
                               where l.CmnCompanyId == cmnId
                               select new
                               {
                                   Id = l.Id,
                                   Name = l.NameEn
                               }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }
        //Adel
        public ActionResult GetProductGroup()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    int cmnId = _erpManager.CmnId;
                    var lst = (from l in database.PosProductGroupDbSet
                               where l.CmnCompanyId == cmnId
                               select new
                               {
                                   Id = l.Id,
                                   Name = l.NameEn
                               }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }
        //Adel
        public ActionResult GetProductSource()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    int cmnId = _erpManager.CmnId;
                    var lst = (from l in database.PosProductSourceDbSet
                               where l.CmnCompanyId == cmnId
                               select new
                               {
                                   Id = l.Id,
                                   Name = l.NameEn
                               }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }
        //Adel
        public ActionResult GetProductDescription()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    int cmnId = _erpManager.CmnId;
                    var lst = (from l in database.PosProductDescriptionDbSet
                               where l.CmnCompanyId == cmnId
                               select new
                               {
                                   Id = l.Id,
                                   Name = l.NameEn
                               }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetProductByCategory(int categoryId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    var lst = (from l in database.PosProductDbSet
                               where l.PosProductCategoryId == categoryId
                               select new
                               {
                                   l.Id,
                                   l.Name
                               }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }

        public ActionResult GetSupplier()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase database = new ConnectionDatabase())
                {
                    int cmnId = _erpManager.CmnId;
                    var lst = (from l in database.PosSupplierDbSet
                               where l.CmnCompanyId == cmnId
                               select new
                               {
                                   l.Id,
                                   l.Name
                               }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }

            return rtr;
        }


        public ActionResult GetCustomersHavingDue()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase context = new ConnectionDatabase())
                {
                    int companyId = _erpManager.CmnId;

                    var customers = (from si in context.PosSalesInvoiceDbSet
                                     where si.CmnCompanyId == companyId && si.DueAmount > 0 && !si.IsDuePaid && !UtilityManager.PosInvoiceTypeNotInForSalesSide.Contains(si.PosInvoiceType)
                                     group si.PosCustomer by si.PosCustomerId into sGroup
                                     select new
                                     {
                                         Id = sGroup.Key,
                                         Name = sGroup.Select(c => c.FirstName + " " + c.LastName).FirstOrDefault()
                                     }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(customers), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }
            return rtr;
        }


        public ActionResult GetDueInvoiceNumbersByCustomer(int customerId)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase context = new ConnectionDatabase())
                {
                    var invoiceNumbers = (from si in context.PosSalesInvoiceDbSet.Where(s => s.PosCustomerId == customerId && s.DueAmount > 0 && !s.IsDuePaid && !UtilityManager.PosInvoiceTypeNotInForSalesSide.Contains(s.PosInvoiceType))
                        select new
                        {
                            Id = si.InvoiceNumber,
                            Name = si.InvoiceNumber
                        }).ToList();
                    rtr = UtilityManager.JsonResultMax(Json(invoiceNumbers), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }
            return rtr;
        }


        public ActionResult GetCustomer(int posInvoiceType)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase context = new ConnectionDatabase())
                {
                    bool posCustomer = posInvoiceType == 5;
                  //List<int> branch= _erpManager.UserOffice;
                    var customer = context.PosCustomerDbSet.Select(s=>s);
                    //if (posCustomer)
                    //    customer=customer.Where(c => !branch.Contains(c.PosBranchId));
                    //else
                    //    customer=customer.Where(c => branch.Contains(c.PosBranchId));
                    var lst = (from c in customer
                               where c.IsPosBranchCustomer == posCustomer && c.CmnCompanyId==_erpManager.CmnId 
                               orderby c.IsPosBranchCustomer
                                          select new
                                          {
                                              Id = c.Id,
                                              Name =c.Phone+" - "+ c.FirstName + " " + c.LastName + "-" + c.Address
                                          }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }
            return rtr;
        }

        public ActionResult GetCustomerCallCenter(int posInvoiceType, int PosBranchId=0)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase context = new ConnectionDatabase())
                {  bool posCustomer = posInvoiceType == 5;
                    //List<int> branch = _erpManager.UserOffice;
                    var customer = context.PosCustomerDbSet.Select(s => s);
                    //if (posCustomer)
                    //    customer = customer.Where(c => !branch.Contains(c.PosBranchId));
                    //else
                    //    customer = customer.Where(c => branch.Contains(c.PosBranchId));
                    if (PosBranchId == 0)
                    {
                        var lst = (from c in customer
                                   where c.IsPosBranchCustomer == posCustomer && c.CmnCompanyId == _erpManager.CmnId
                                   //&& (c.PosBranchId == PosBranchId || (c.PosBranch.Name == _erpManager.BranchName))
                                   orderby c.IsPosBranchCustomer
                                   select new
                                   {
                                       Id = c.Id,
                                       Name = c.Phone + " - " + c.FirstName +" "+ c.LastName +"-"+c.Address
                                   }).ToList();
                        rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        var lst = (from c in customer
                                   where c.IsPosBranchCustomer == posCustomer && c.CmnCompanyId == _erpManager.CmnId
                                   && (c.PosBranchId == PosBranchId)
                                   orderby c.IsPosBranchCustomer
                                   select new
                                   {
                                       Id = c.Id,
                                       Name = c.Phone + " - " + c.FirstName + " " + c.LastName + "-" + c.Address
                                   }).ToList();
                        rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }
            return rtr;
        }

        public ActionResult GetDeliveryMan(int posInvoiceType)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase context = new ConnectionDatabase())
                {
                    bool posDeliveryMan = posInvoiceType == 5;
                    List<int> branch = _erpManager.UserOffice;
                    var DeliveryMan = context.PosDeliveryManDbSet.Select(s => s);
                    if (posDeliveryMan)
                        DeliveryMan = DeliveryMan.Where(c => !branch.Contains(c.PosBranchId));
                    else
                        DeliveryMan = DeliveryMan.Where(c => branch.Contains(c.PosBranchId));
                    var lst = (from c in DeliveryMan
                               where  c.PosBranch.Name == _erpManager.BranchName
                              
                               select new
                               {
                                   Id = c.Id,
                                   Name = c.Phone + " - " + c.DeliveryManName_en
                               }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(lst), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                rtr = _erpManager.ExceptionDefault(e);
            }
            return rtr;
        }

    }
}