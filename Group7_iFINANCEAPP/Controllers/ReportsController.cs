using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Group7_iFINANCEAPP.Models;
using Group7_iFINANCEAPP.Models.ViewModels;

namespace Group7_iFINANCEAPP.Controllers
{
    public class ReportsController : Controller
    {
        private Group7_iFINANCEDBEntities db = new Group7_iFINANCEDBEntities();


        // GET: Reports
        public ActionResult Index()
        {
            if (Session["NonAdminUserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        public ActionResult ProfitLoss()
        {
            int nonAdminUserID = (int)Session["NonAdminUserID"];

            var masterAccounts = db.MasterAccount.Where(a => a.Group.NonAdminUserID == nonAdminUserID).ToList();
            var incomeAccounts = masterAccounts.Where(a => a.Group.AccountCategory.name == "Income").ToList();
            var expenseAccounts = masterAccounts.Where(a => a.Group.AccountCategory.name == "Expenses").ToList();

            var totalIncome = incomeAccounts.Sum(acct => acct.closingAmount);
            var totalExpense = expenseAccounts.Sum(acct => acct.closingAmount);

            return View(new ProfitLossViewModel(incomeAccounts,expenseAccounts,totalIncome, totalExpense));
        }

        public ActionResult TrialBalance()
        {
            int nonAdminUserID = (int)Session["NonAdminUserID"];

            var masterAccounts = db.MasterAccount.Where(a => a.Group.NonAdminUserID == nonAdminUserID).ToList();

            var trialBalanceLineItems = new List<TrailBalanceLineItem>();
            foreach(var account in masterAccounts)
            {
                var lineItem = new TrailBalanceLineItem(account.name);
                if(account.Group.AccountCategory.type == "Debit")
                {
                    lineItem.debit = account.closingAmount;
                }
                else
                {
                    lineItem.credit = account.closingAmount;
                }
                trialBalanceLineItems.Add(lineItem);
                
            }
            var totalDebit = trialBalanceLineItems.Sum(item => item.debit);
            var totalCredit = trialBalanceLineItems.Sum(item => item.credit);

            return View(new TrialBalanceViewModel(trialBalanceLineItems,totalDebit,totalCredit));
        }
    }



    public enum ReportType
    {
        NONE,
        TRIALBALANCE,
        PROFITLOSS
    }

    public class TrailBalanceLineItem
    {
        public TrailBalanceLineItem(string accountName)
        {
            this.accountName = accountName;
        }
        public string accountName;
        public double? debit { get; set; }
        public double? credit { get; set; }
    }
}
