using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using ERP.Controllers.Manager;
using ERP.Controllers.POS.Manager;
using ERP.CSharpLib;
using ERP.Models;
using ERP.Models.POS;
using ERP.Models.Security.Authorization;
using ERP.Models.VModel;
using Newtonsoft.Json;

namespace ERP.Controllers
{
    public class ExpenseController : BaseController
    {
        private readonly ErpManager _erpManager = new ErpManager();

        

        public ActionResult Expense()
        {
            return View();
        }


        public ActionResult SaveExpense(string expenseObj)
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {

                PosExpenses expense = JsonConvert.DeserializeObject<PosExpenses>(expenseObj);
                int cmnId = _erpManager.CmnId;
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    expense.CmnCompanyId = cmnId;
                    expense.CreatedBy = _erpManager.UserId;
                    expense.CreatedDate = UTCDateTime.BDDateTime();
                    dbContext.PosExpensesDbSet.Add(expense);
                    dbContext.SaveChanges();
                }

                return Json(new { success = true, message = "Expense saved successfully!", Id = expense.ExpenseType });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while saving the expense.", error = ex.Message });
            }
        }

        public ActionResult UpdateExpense(PosExpenses expense)
        {
            if (expense == null || expense.Id <= 0)
            {
                return Json(new { success = false, message = "Invalid expense data." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                using (var dbContext = new ConnectionDatabase())
                {
                   
                    var existingExpense = dbContext.PosExpensesDbSet.SingleOrDefault(e => e.Id == expense.Id);
                    if (existingExpense == null)
                    {
                        return Json(new { success = false, message = "Expense not found." }, JsonRequestBehavior.AllowGet);
                    }

                    existingExpense.ModifiedBy = _erpManager.UserId;
                    existingExpense.ModifideDate = UTCDateTime.BDDateTime();
                    existingExpense.CreatedBy = existingExpense.CreatedBy;
                    existingExpense.CreatedDate = existingExpense.CreatedDate;
                    existingExpense.CmnCompanyId = existingExpense.CmnCompanyId;
                    existingExpense.ExpenseType = expense.ExpenseType;
                    existingExpense.Amount = expense.Amount;
                    existingExpense.Date = expense.Date;
                    existingExpense.Notes = expense.Notes;

                    
                    dbContext.SaveChanges();

                    return Json(new { success = true, message = "Expense updated successfully." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                

                return Json(new { success = false, message = "An error occurred while updating the expense." }, JsonRequestBehavior.AllowGet);
            }
        }


        //public ActionResult UpdateExpense(PosExpenses expense)
        //{
        //    ActionResult rtr = _erpManager.ActionResultDefault;
        //    try
        //    {
        //        using (ConnectionDatabase dbContext = new ConnectionDatabase())
        //        {

        //            expense.Date = UTCDateTime.BDDateTime();

        //            dbContext.Entry(expense).State = EntityState.Modified;
        //            dbContext.Entry(expense).Property(o => o.Id).IsModified = false;


        //            int feedback = dbContext.SaveChanges();
        //            rtr = Json(feedback, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        rtr = _erpManager.ExceptionDefault(e);
        //    }

        //    return rtr;
        //}

        public ActionResult DeleteExpense(int expenseId)
        {
            if (expenseId <= 0)
            {
                return Json(new { success = false, message = "Invalid expense ID." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                using (var dbContext = new ConnectionDatabase())
                {
                    var expenseToDelete = dbContext.PosExpensesDbSet.Find(expenseId);

                    if (expenseToDelete == null)
                    {
                        return Json(new { success = false, message = "Expense not found." }, JsonRequestBehavior.AllowGet);
                    }

                    dbContext.PosExpensesDbSet.Remove(expenseToDelete);
                    int feedback = dbContext.SaveChanges();

                    return Json(new { success = true, message = "Expense deleted successfully.", rowsAffected = feedback }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                // Log the exception (replace with your logging mechanism)
                Console.WriteLine($"Error deleting expense: {e.Message}");

                return Json(new { success = false, message = "An error occurred while deleting the expense." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetExpenseList()
        {
            ActionResult rtr = _erpManager.ActionResultDefault;
            try
            {
                using (ConnectionDatabase dbContext = new ConnectionDatabase())
                {
                    var data = (from expense in dbContext.PosExpensesDbSet
                                join brn in dbContext.PosBrancheDbSet on expense.PosBranchId equals brn.Id
                                select new
                                {
                                    expense.Id,
                                    expense.ExpenseType,
                                    expense.Amount,
                                    expense.Date,
                                    expense.Notes,
                                    expense.PosBranchId,
                                    Branch = brn.Name
                                }).ToList();

                    rtr = UtilityManager.JsonResultMax(Json(data), JsonRequestBehavior.AllowGet);
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
