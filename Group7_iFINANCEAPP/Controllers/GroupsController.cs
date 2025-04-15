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
    public class GroupsController : Controller
    {
        private Group7_iFINANCEDBEntities db = new Group7_iFINANCEDBEntities();

        // GET: Groups
        public ActionResult Index()
        {
            if (Session["NonAdminUserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            int nonAdminUserID = (int)Session["NonAdminUserID"];
            
            var group = db.Group.Where(g => g.NonAdminUserID == nonAdminUserID).Include(g => g.AccountCategory);


            ViewBag.AccountCategories = db.AccountCategory;
            var k = group.ToList();

            return View(group.ToList());
            
        }

        // GET: Groups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Group.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // GET: Groups/Create

        /*
        public ActionResult Create()
        {
            ViewBag.AccountCategoryID = new SelectList(db.AccountCategory, "ID", "name");
            // TODO: Replace with currently authenticated user
            ViewBag.NonAdminUserID = new SelectList(db.NonAdminUser, "ID", "name");
            return View();
        }
        */

        public ActionResult Create(int? accountCategoryID, int? parentID)
        {

            if (parentID != null)
            {
                ViewBag.parentName = db.Group.Find(parentID).name;
            }
            else if (accountCategoryID != null)
            {
                ViewBag.parentName = db.AccountCategory.Find(accountCategoryID).name;
            } else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // TODO: Replace with currently authenticated user
            ViewBag.NonAdminUserID = new SelectList(db.NonAdminUser, "ID", "name");

            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name,AccountCategoryID")] Group group, int?parentID)
        {
            if (parentID != null)
            {
                group.parentID = parentID;
                group.AccountCategoryID = db.Group.Find(parentID).AccountCategoryID;
            }
            
            group.NonAdminUserID = (int)Session["NonAdminUserID"];

            if (ModelState.IsValid)
            {
                db.Group.Add(group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountCategoryID = new SelectList(db.AccountCategory, "ID", "name", group.AccountCategoryID);
            ViewBag.NonAdminUserID = new SelectList(db.NonAdminUser, "ID", "name", group.NonAdminUserID);
            return View(group);
        }

        // GET: Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Group.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }

            return View(group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        // This was the auto-generated code, but does not work because Bind will clear
        // all the non-bound attributes, but we don't want to bind to every attribute. Updated method below
        // See: https://learn.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/implementing-basic-crud-functionality-with-the-entity-framework-in-asp-net-mvc-application#update-httppost-edit-method

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,name,AccountCategoryID")] Group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountCategoryID = new SelectList(db.AccountCategory, "ID", "name", group.AccountCategoryID);
            return View(group);
        }
        */

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var group = db.Group.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }

            if (TryUpdateModel(group, "",
                new string[] { "name" }))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            return View(group);
        }

        // GET: Groups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Group.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Group group = db.Group.Find(id);
            db.Group.Remove(group);
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
