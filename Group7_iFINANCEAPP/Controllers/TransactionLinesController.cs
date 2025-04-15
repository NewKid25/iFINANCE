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

            // Credit
            ViewBag.MasterAccountID = new SelectList(db.MasterAccount.Where(a => a.Group.NonAdminUserID == nonAdminUserID).Where(a => a.Group.AccountCategory.type == "Credit"), "ID", "name");
            // Debit
            ViewBag.MasterAccountID2 = new SelectList(db.MasterAccount.Where(a => a.Group.NonAdminUserID == nonAdminUserID).Where(a => a.Group.AccountCategory.type == "Debit"), "ID", "name");
            ViewBag.TransactionID = new SelectList(db.Transaction.Where(t => t.NonAdminUserID == nonAdminUserID), "ID", "description");
            return View(line);
        }

        // POST: TransactionLines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "creditedAmount,debitedAmount,comment,TransactionID,MasterAccountID,MasterAccountID2")] TransactionLine transactionLine)
        {
            if (ModelState.IsValid)
            {
                db.TransactionLine.Add(transactionLine);
                db.SaveChanges();
                return RedirectToAction("Index", "Transactions");
            }

            ViewBag.MasterAccountID = new SelectList(db.MasterAccount, "ID", "name", transactionLine.MasterAccountID);
            ViewBag.MasterAccountID2 = new SelectList(db.MasterAccount, "ID", "name", transactionLine.MasterAccountID2);
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
            ViewBag.MasterAccountID = new SelectList(db.MasterAccount, "ID", "name", transactionLine.MasterAccountID);
            ViewBag.MasterAccountID2 = new SelectList(db.MasterAccount, "ID", "name", transactionLine.MasterAccountID2);
            ViewBag.TransactionID = new SelectList(db.Transaction, "ID", "description", transactionLine.TransactionID);
            return View(transactionLine);
        }

        // POST: TransactionLines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "creditedAmount,debitedAmount,comment,TransactionID,MasterAccountID,MasterAccountID2")] TransactionLine transactionLine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transactionLine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Transactions");
            }
            ViewBag.MasterAccountID = new SelectList(db.MasterAccount, "ID", "name", transactionLine.MasterAccountID);
            ViewBag.MasterAccountID2 = new SelectList(db.MasterAccount, "ID", "name", transactionLine.MasterAccountID2);
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
