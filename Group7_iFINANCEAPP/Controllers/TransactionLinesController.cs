using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Group7_iFINANCEAPP.Models;

namespace Group7_iFINANCEAPP.Controllers
{
    public class TransactionLinesController : Controller
    {
        private Group7_iFINANCEDBEntities db = new Group7_iFINANCEDBEntities();

        // GET: TransactionLines
        public ActionResult Index()
        {
            if (Session["NonAdminUserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var transactionLine = db.TransactionLine.Include(t => t.MasterAccount).Include(t => t.MasterAccount1).Include(t => t.Transaction);
            return View(transactionLine.ToList());
        }

        // GET: TransactionLines/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionLine transactionLine = db.TransactionLine.Find(id);
            if (transactionLine == null)
            {
                return HttpNotFound();
            }
            return View(transactionLine);
        }

        // GET: TransactionLines/Create
        public ActionResult Create(int? transactionId)
        {
            var line = new TransactionLine();

            if (transactionId.HasValue)
            {
                line.TransactionID = transactionId.Value;
            }

            int nonAdminUserID = (int)Session["NonAdminUserID"];

            var masterAccounts = db.MasterAccount.Where(a => a.Group.NonAdminUserID == nonAdminUserID).ToList();
            masterAccounts.Insert(0, null);

            // Credit
            ViewBag.MasterAccountID = new SelectList(masterAccounts, "ID", "name");
            // Debit
            ViewBag.MasterAccountID2 = new SelectList(masterAccounts, "ID", "name");
            ViewBag.Warning = false;
            return View(line);
        }

        // POST: TransactionLines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "creditedAmount,debitedAmount,comment,TransactionID,MasterAccountID,MasterAccountID2")] TransactionLine transactionLine)
        {
            int nonAdminUserID = (int)Session["NonAdminUserID"];

            var masterAccounts = db.MasterAccount.Where(a => a.Group.NonAdminUserID == nonAdminUserID).ToList();
            masterAccounts.Insert(0, null);

            if (ModelState.IsValid)
            {
                if (Request.Form["warning"] != "visible")
                {
                    MasterAccount masterAccount = db.MasterAccount.Find(transactionLine.MasterAccountID ?? 0);
                    MasterAccount masterAccount1 = db.MasterAccount.Find(transactionLine.MasterAccountID2 ?? 0);
                    if ((masterAccount != null && ChartAccountsController.CheckValueAfterCredit(masterAccount, transactionLine.creditedAmount ?? 0) < 0) ||
                        (masterAccount1 != null && ChartAccountsController.CheckValueAfterDebit(masterAccount1, transactionLine.debitedAmount ?? 0) < 0))
                    {
                        ViewBag.Warning = true;

                        ViewBag.MasterAccountID = new SelectList(masterAccounts, "ID", "name", transactionLine.MasterAccountID);
                        ViewBag.MasterAccountID2 = new SelectList(masterAccounts, "ID", "name", transactionLine.MasterAccountID2);
                        ViewBag.TransactionID = new SelectList(db.Transaction, "ID", "description", transactionLine.TransactionID);
                        return View(transactionLine);
                    }
                }
                

                
                db.TransactionLine.Add(transactionLine);

                ChartAccountsController.UpdateClosingBalance(nonAdminUserID, db);

                db.SaveChanges();
                return RedirectToAction("Index", "Transactions");
            }



            ViewBag.MasterAccountID = new SelectList(masterAccounts, "ID", "name", transactionLine.MasterAccountID);
            ViewBag.MasterAccountID2 = new SelectList(masterAccounts, "ID", "name", transactionLine.MasterAccountID2);
            ViewBag.TransactionID = new SelectList(db.Transaction, "ID", "description", transactionLine.TransactionID);
            return View(transactionLine);
        }

        // GET: TransactionLines/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionLine transactionLine = db.TransactionLine.Find(id);
            if (transactionLine == null)
            {
                return HttpNotFound();
            }

            int nonAdminUserID = (int)Session["NonAdminUserID"];

            var masterAccounts = db.MasterAccount.Where(a => a.Group.NonAdminUserID == nonAdminUserID).ToList();
            masterAccounts.Insert(0, null);


            ViewBag.MasterAccountID = new SelectList(masterAccounts, "ID", "name", transactionLine.MasterAccountID);
            ViewBag.MasterAccountID2 = new SelectList(masterAccounts, "ID", "name", transactionLine.MasterAccountID2);
            ViewBag.TransactionID = new SelectList(db.Transaction, "ID", "description", transactionLine.TransactionID);

            ViewBag.Warning = false;
            return View(transactionLine);
        }

        // POST: TransactionLines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            TransactionLine transactionLine = db.TransactionLine.Find(id);

            int nonAdminUserID = (int)Session["NonAdminUserID"];

            var masterAccounts = db.MasterAccount.Where(a => a.Group.NonAdminUserID == nonAdminUserID).ToList();
            masterAccounts.Insert(0, null);


            if (TryUpdateModel(transactionLine, "",
                new string[] { "creditedAmount", "debitedAmount", "comment", "MasterAccountID", "MasterAccountID2" }))
            {
                if (Request.Form["warning"] != "visible")
                {
                    MasterAccount masterAccount = db.MasterAccount.Find(transactionLine.MasterAccountID ?? 0);
                    MasterAccount masterAccount1 = db.MasterAccount.Find(transactionLine.MasterAccountID2 ?? 0);
                    if ((masterAccount != null && ChartAccountsController.CheckValueAfterCredit(masterAccount, transactionLine.creditedAmount ?? 0) < 0) ||
                        (masterAccount1 != null && ChartAccountsController.CheckValueAfterDebit(masterAccount1, transactionLine.debitedAmount ?? 0) < 0))
                    {
                        ViewBag.Warning = true;

                        ViewBag.MasterAccountID = new SelectList(masterAccounts, "ID", "name", transactionLine.MasterAccountID);
                        ViewBag.MasterAccountID2 = new SelectList(masterAccounts, "ID", "name", transactionLine.MasterAccountID2);
                        ViewBag.TransactionID = new SelectList(db.Transaction, "ID", "description", transactionLine.TransactionID);
                        return View(transactionLine);
                    }
                }
                try
                {
                    db.SaveChanges();

                    ChartAccountsController.UpdateClosingBalance(nonAdminUserID, db);

                    return RedirectToAction("Index", "Transactions");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            ViewBag.MasterAccountID = new SelectList(masterAccounts, "ID", "name", transactionLine.MasterAccountID);
            ViewBag.MasterAccountID2 = new SelectList(masterAccounts, "ID", "name", transactionLine.MasterAccountID2);
            ViewBag.TransactionID = new SelectList(db.Transaction, "ID", "description", transactionLine.TransactionID);
            return View(transactionLine);
        }

        // GET: TransactionLines/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionLine transactionLine = db.TransactionLine.Find(id);
            if (transactionLine == null)
            {
                return HttpNotFound();
            }
            return View(transactionLine);
        }

        // POST: TransactionLines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {


            TransactionLine transactionLine = db.TransactionLine.Find(id);

            db.TransactionLine.Remove(transactionLine);
            db.SaveChanges();

            int nonAdminUserID = (int)Session["NonAdminUserID"];

            var masterAccounts = db.MasterAccount.Where(a => a.Group.NonAdminUserID == nonAdminUserID).ToList();
            masterAccounts.Insert(0, null);

            ChartAccountsController.UpdateClosingBalance(nonAdminUserID, db);

            return RedirectToAction("Index", "Transactions");
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