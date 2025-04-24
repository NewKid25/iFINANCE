using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Group7_iFINANCEAPP.Models;
using Microsoft.Ajax.Utilities;

namespace Group7_iFINANCEAPP.Controllers
{
    public class ChartAccountsController : Controller
    {
        private Group7_iFINANCEDBEntities db = new Group7_iFINANCEDBEntities();

        // GET: ChartAccounts
        public ActionResult Index()
        {
            // Redirect to login if not authenticated
            if (Session["NonAdminUserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            int nonAdminUserID = (int)Session["NonAdminUserID"];

            var masterAccounts = db.MasterAccount.Where(a => a.Group.NonAdminUserID == nonAdminUserID)
                .Include(a => a.TransactionLine)
                .Include(a => a.TransactionLine1)
                .Include(a => a.Group.AccountCategory)
                .ToList();

            // Recalculate closing balances just in case
            UpdateClosingBalance(nonAdminUserID, db);

            return View(masterAccounts);
        }

        // Helper function to update closing balance of all accounts belonging to a user.
        // Also called when a TransactionLine is created/edited
        public static void UpdateClosingBalance(int nonAdminUserID, Group7_iFINANCEDBEntities db)
        {

            var masterAccounts = db.MasterAccount.Where(a => a.Group.NonAdminUserID == nonAdminUserID)
                .Include(a => a.TransactionLine)
                .Include(a => a.TransactionLine1)
                .Include(a => a.Group.AccountCategory)
                .ToList();

            foreach (var masterAccount in masterAccounts)
            {
                double amt = masterAccount.openingAmount;

                foreach (var line in masterAccount.TransactionLine) // Credit entry
                {
                    if (masterAccount.Group.AccountCategory.type == "Credit")
                    {
                        amt += line.creditedAmount ?? 0;
                    } else
                    {
                        amt -= line.creditedAmount ?? 0;
                    }
                }

                foreach (var line in masterAccount.TransactionLine1) // Debit entry
                {
                    if (masterAccount.Group.AccountCategory.type == "Debit")
                    {
                        amt += line.debitedAmount ?? 0;
                    } else
                    {
                        amt -= line.debitedAmount ?? 0;
                    }
                }

                masterAccount.closingAmount = amt;
                db.Entry(masterAccount).State = EntityState.Modified;
            }

            // Accounts may already be up to date
            try
            {
                db.SaveChanges();
            }
            finally
            { }
        }

        // Helper function to see what the closing value of an account would be if the provided
        // debit were applied (used to determine warnings in TransactionLines)
        public static double CheckValueAfterDebit(MasterAccount masterAccount, double debit)
        {
            double amt = masterAccount.closingAmount;

            if (masterAccount.Group.AccountCategory.type == "Debit")
            {
                amt += debit;
            }
            else
            {
                amt -= debit;
            }

            return amt;
        }

        // Helper function to see what the closing value of an account would be if the provided
        // credit were applied (used to determine warnings in TransactionLines)
        public static double CheckValueAfterCredit(MasterAccount masterAccount, double credit)
        {
            double amt = masterAccount.closingAmount;

            if (masterAccount.Group.AccountCategory.type == "Credit")
            {
                amt += credit;
            }
            else
            {
                amt -= credit;
            }

            return amt;
        }

        // GET: ChartAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MasterAccount masterAccount = db.MasterAccount.Find(id);
            if (masterAccount == null)
            {
                return HttpNotFound();
            }
            return View(masterAccount);
        }

        // GET: ChartAccounts/Create
        public ActionResult Create()
        {
            var groups = db.NonAdminUser.Find(Session["NonAdminUserID"]).Group.ToList();
            ViewBag.GroupID = new SelectList(groups, "ID", "name");
            return View();
        }

        // POST: ChartAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,name,openingAmount,closingAmount,GroupID")] MasterAccount masterAccount)
        {
            if (ModelState.IsValid)
            {
                masterAccount.closingAmount = masterAccount.openingAmount;
                db.MasterAccount.Add(masterAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(masterAccount);
        }

        // GET: ChartAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MasterAccount masterAccount = db.MasterAccount.Find(id);
            if (masterAccount == null)
            {
                return HttpNotFound();
            }
            var groups = db.NonAdminUser.Find(Session["NonAdminUserID"]).Group.ToList();
            ViewBag.GroupID = new SelectList(groups, "ID", "name", masterAccount.GroupID);
            return View(masterAccount);
        }

        // POST: ChartAccounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,name,openingAmount,closingAmount,GroupID")] MasterAccount masterAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(masterAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(masterAccount);
        }

        // GET: ChartAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MasterAccount masterAccount = db.MasterAccount.Find(id);
            if (masterAccount == null)
            {
                return HttpNotFound();
            }
            return View(masterAccount);
        }

        // POST: ChartAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MasterAccount masterAccount = db.MasterAccount.Find(id);

            // Delete all corresponding transaction lines
            masterAccount.TransactionLine.ToList().ForEach(t => { db.TransactionLine.Remove(t); });
            masterAccount.TransactionLine1.ToList().ForEach(t => { db.TransactionLine.Remove(t); });

            db.MasterAccount.Remove(masterAccount);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
