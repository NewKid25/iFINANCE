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
            if (Session["NonAdminUserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            int nonAdminUserID = (int)Session["NonAdminUserID"];
            return View(db.MasterAccount.Where(a => a.Group.NonAdminUserID == nonAdminUserID).ToList());
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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
