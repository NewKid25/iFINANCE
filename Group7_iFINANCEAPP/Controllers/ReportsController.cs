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
            // Redirect to login if not authenticated
            if (Session["NonAdminUserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        // GET: Reports/ProfitLoss
        public ActionResult ProfitLoss(string before, string after)
        {
            int nonAdminUserID = (int)Session["NonAdminUserID"];

            var masterAccounts = db.MasterAccount.Where(a => a.Group.NonAdminUserID == nonAdminUserID).ToList();
            var incomeAccounts = masterAccounts.Where(a => a.Group.AccountCategory.name == "Income").ToList();
            var expenseAccounts = masterAccounts.Where(a => a.Group.AccountCategory.name == "Expenses").ToList();

            double totalIncome;
            double totalExpense;


            // If date filtering is not applicable, sum all closing amounts
            if (before == null && after == null)
            {
                totalIncome = incomeAccounts.Sum(acct => acct.closingAmount);
                totalExpense = expenseAccounts.Sum(acct => acct.closingAmount);
            }
            // Otherwise, filter to only transactions between the specified dates
            else
            {
                totalIncome = 0;
                totalExpense = 0;

                DateTime bef = DateTime.MaxValue;
                DateTime aft = DateTime.MinValue;

                if (before != null)
                {
                    try
                    {
                        bef = DateTime.Parse(before);
                    }
                    catch (FormatException) { }
                }

                if (after != null)
                {
                    try
                    {
                        aft = DateTime.Parse(after);
                    }
                    catch (FormatException) { }
                }

                foreach (var account in incomeAccounts)
                {
                    account.closingAmount = 0;
                    account.closingAmount -= account.TransactionLine1.Where(t => t.Transaction.date > aft && bef > t.Transaction.date).Sum(t => t.debitedAmount) ?? 0;
                    account.closingAmount += account.TransactionLine.Where(t => t.Transaction.date > aft && bef > t.Transaction.date).Sum(t => t.creditedAmount) ?? 0;

                    totalIncome += account.closingAmount;
                }

                foreach (var account in expenseAccounts)
                {
                    account.closingAmount = 0;
                    account.closingAmount += account.TransactionLine1.Where(t => t.Transaction.date > aft && bef > t.Transaction.date).Sum(t => t.debitedAmount) ?? 0;
                    account.closingAmount -= account.TransactionLine.Where(t => t.Transaction.date > aft && bef > t.Transaction.date).Sum(t => t.creditedAmount) ?? 0;

                    totalExpense += account.closingAmount;
                }
            }

            return View(new ProfitLossViewModel(incomeAccounts,expenseAccounts,totalIncome, totalExpense));
        }

        // GET: Reports/TrialBalance
        public ActionResult TrialBalance(string before, string after)
        {
            int nonAdminUserID = (int)Session["NonAdminUserID"];

            var masterAccounts = db.MasterAccount.Where(a => a.Group.NonAdminUserID == nonAdminUserID).ToList();

            // Filter to transactions between the specified dates (if provided)
            DateTime bef = DateTime.MaxValue;
            DateTime aft = DateTime.MinValue;

            if (before != null)
            {
                try
                {
                    bef = DateTime.Parse(before);
                }
                catch (FormatException) { }
            }

            if (after != null)
            {
                try
                {
                    aft = DateTime.Parse(after);
                }
                catch (FormatException) { }
            }

            var trialBalanceLineItems = new List<TrailBalanceLineItem>();
            foreach(var account in masterAccounts)
            {
                var lineItem = new TrailBalanceLineItem(account.name);
                if(account.Group.AccountCategory.type == "Debit")
                {
                    if (before != null || after != null)
                    {
                        account.closingAmount = 0;
                        account.closingAmount += account.TransactionLine1.Where(t => t.Transaction.date > aft && bef > t.Transaction.date).Sum(t => t.debitedAmount) ?? 0;
                        account.closingAmount -= account.TransactionLine.Where(t => t.Transaction.date > aft && bef > t.Transaction.date).Sum(t => t.creditedAmount) ?? 0;
                    }

                    lineItem.debit = account.closingAmount;
                }
                else
                {
                    if (before == null || after == null)
                    {
                        account.closingAmount = 0;
                        account.closingAmount -= account.TransactionLine1.Where(t => t.Transaction.date > aft && bef > t.Transaction.date).Sum(t => t.debitedAmount) ?? 0;
                        account.closingAmount += account.TransactionLine.Where(t => t.Transaction.date > aft && bef > t.Transaction.date).Sum(t => t.creditedAmount) ?? 0;
                    }

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
