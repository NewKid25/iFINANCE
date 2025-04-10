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
    public class TransactionsController : Controller
    {
        private Group7_iFINANCEDBEntities db = new Group7_iFINANCEDBEntities();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.Transaction
                                 .Include(t => t.TransactionLine)
                                 .Include(t => t.NonAdminUser)
                                 .ToList()
                                 .Select(t => new TransactionWithLinesViewModel
                                 {
                                     Transaction = t,
                                     TransactionLines = t.TransactionLine.ToList()
                                 }).ToList();

            return View(transactions);
        }


        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transaction.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.NonAdminUserID = new SelectList(db.NonAdminUser, "ID", "name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,date,description,NonAdminUserID")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Transaction.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.NonAdminUserID = new SelectList(db.NonAdminUser, "ID", "name", transaction.NonAdminUserID);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transaction.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.NonAdminUserID = new SelectList(db.NonAdminUser, "ID", "name", transaction.NonAdminUserID);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,date,description,NonAdminUserID")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NonAdminUserID = new SelectList(db.NonAdminUser, "ID", "name", transaction.NonAdminUserID);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transaction.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transaction.Find(id);
            db.Transaction.Remove(transaction);
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

        public ActionResult TransactionsWithLines()
        {
            using (var db = new Group7_iFINANCEDBEntities())
            {
                var data = db.Transaction
                             .Include(t => t.TransactionLine)
                             .ToList()
                             .Select(t => new TransactionWithLinesViewModel
                             {
                                 Transaction = t,
                                 TransactionLines = t.TransactionLine.ToList()
                             }).ToList();

                return View(data);
            }
        }

    }
}
