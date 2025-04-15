using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Group7_iFINANCEAPP.Models;

namespace Group7_iFINANCEAPP.Controllers
{
    public class NonAdminUsersController : Controller
    {
        private Group7_iFINANCEDBEntities db = new Group7_iFINANCEDBEntities();

        // GET: NonAdminUsers
        public ActionResult Index()
        {
            if (Session["AdministratorID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var nonAdminUser = db.NonAdminUser.Include(n => n.Administrator);
            return View(nonAdminUser.ToList());
        }

        // GET: NonAdminUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NonAdminUser nonAdminUser = db.NonAdminUser.Find(id);
            if (nonAdminUser == null)
            {
                return HttpNotFound();
            }
            return View(nonAdminUser);
        }

        // GET: NonAdminUsers/Create
        public ActionResult Create()
        {
            ViewBag.AdministratorID = new SelectList(db.Administrator, "ID", "name");
            return View();
        }

        // POST: NonAdminUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name,address,email,AdministratorID")] NonAdminUser nonAdminUser)
        {
            if (ModelState.IsValid)
            {
                NonAdminUser user = db.NonAdminUser.Add(nonAdminUser);
                db.SaveChanges();

                UserPassword pwd = new UserPassword();
                pwd.userName = Request.Form["username"];
                byte[] pass = UTF8Encoding.UTF8.GetBytes(Request.Form["password"]);
                byte[] hash = SHA256.Create().ComputeHash(pass);

                pwd.encryptedPassword = GetHashString(hash);

                pwd.NonAdminUserID = user.ID;

                db.UserPassword.Add(pwd);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.AdministratorID = new SelectList(db.Administrator, "ID", "name", nonAdminUser.AdministratorID);
            return View(nonAdminUser);
        }

        public static string GetHashString(byte[] hash)
        {
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < hash.Length; i++)
            {
                sBuilder.Append(hash[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // GET: NonAdminUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NonAdminUser nonAdminUser = db.NonAdminUser.Find(id);
            if (nonAdminUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdministratorID = new SelectList(db.Administrator, "ID", "name", nonAdminUser.AdministratorID);
            return View(nonAdminUser);
        }

        // POST: NonAdminUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "name,address,email,AdministratorID")] NonAdminUser nonAdminUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nonAdminUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdministratorID = new SelectList(db.Administrator, "ID", "name", nonAdminUser.AdministratorID);
            return View(nonAdminUser);
        }

        // GET: NonAdminUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NonAdminUser nonAdminUser = db.NonAdminUser.Find(id);
            if (nonAdminUser == null)
            {
                return HttpNotFound();
            }
            return View(nonAdminUser);
        }

        // POST: NonAdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NonAdminUser nonAdminUser = db.NonAdminUser.Find(id);
            db.NonAdminUser.Remove(nonAdminUser);
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
