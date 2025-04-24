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
            // Redirect to login if not authenticated
            if (Session["NonAdminUserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            int nonAdminUserID = (int)Session["NonAdminUserID"];
            
            var group = db.Group.Where(g => g.NonAdminUserID == nonAdminUserID).Include(g => g.AccountCategory);

            // Provide the account categories to the view
            ViewBag.AccountCategories = db.AccountCategory;

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
        public ActionResult Create(int? accountCategoryID, int? parentID)
        {
            // Subgroup of another group
            if (parentID != null)
            {
                ViewBag.parentName = db.Group.Find(parentID).name;
            }
            // Top level group in a particular account category
            else if (accountCategoryID != null)
            {
                ViewBag.parentName = db.AccountCategory.Find(accountCategoryID).name;
            }
            // Bad request
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }

        // POST: Groups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name,AccountCategoryID")] Group group, int?parentID)
        {
            if (parentID != null)
            {
                group.parentID = parentID;
                // Account category should match the parent
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

            // Will recursively remove children
            RemoveGroup(group, db);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Helper function to recursively remove contained subgroups and master accounts
        public static void RemoveGroup(Group group, Group7_iFINANCEDBEntities db)
        {
            group.Groups.ToList().ForEach(g => { RemoveGroup(g, db); });
            group.MasterAccount.ToList().ForEach(a =>
            {
                a.TransactionLine.ToList().ForEach(b => { db.TransactionLine.Remove(b); });
                a.TransactionLine1.ToList().ForEach(b => { db.TransactionLine.Remove(b); });
                db.MasterAccount.Remove(a);
            });
            db.Group.Remove(group);   
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
