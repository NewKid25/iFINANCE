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
    public class ChartAccountsController : Controller
    {
        private Group7_iFINANCEDBEntities db = new Group7_iFINANCEDBEntities();

        // GET: ChartAccounts
        public ActionResult Index()
        {
            return View(db.AccountCategory.ToList());
        }

        // GET: ChartAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountCategory accountCategory = db.AccountCategory.Find(id);
            if (accountCategory == null)
            {
                return HttpNotFound();
            }
            return View(accountCategory);
        }

        // GET: ChartAccounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChartAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name,type")] AccountCategory accountCategory)
        {
            if (ModelState.IsValid)
            {
                db.AccountCategory.Add(accountCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(accountCategory);
        }

        // GET: ChartAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountCategory accountCategory = db.AccountCategory.Find(id);
            if (accountCategory == null)
            {
                return HttpNotFound();
            }
            return View(accountCategory);
        }

        // POST: ChartAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "name,type")] AccountCategory accountCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accountCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(accountCategory);
        }

        // GET: ChartAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountCategory accountCategory = db.AccountCategory.Find(id);
            if (accountCategory == null)
            {
                return HttpNotFound();
            }
            return View(accountCategory);
        }

        // POST: ChartAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AccountCategory accountCategory = db.AccountCategory.Find(id);
            db.AccountCategory.Remove(accountCategory);
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
