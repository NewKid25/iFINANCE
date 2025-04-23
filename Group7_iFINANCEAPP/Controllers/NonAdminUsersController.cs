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
using Group7_iFINANCEAPP.Models.ViewModels;

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

            var admin = db.Administrator;

            var viewModel = new ManageUsersViewModel();
            
            viewModel.Administrators = admin.ToList();
            viewModel.NonAdminUsers = nonAdminUser.ToList();

            return View(viewModel);
        }

        // GET: NonAdminUsers/Details/5
        public ActionResult DetailsAdmin(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Administrator admin = db.Administrator.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // GET: NonAdminUsers/Details/5
        public ActionResult DetailsNonAdmin(int? id)
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
        public ActionResult CreateAdmin()
        {
            return View();
        }

        // POST: NonAdminUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAdmin([Bind(Include = "name,dateHired,dateFinished")] Administrator admin)
        {
            if (ModelState.IsValid)
            {
                Administrator user = db.Administrator.Add(admin);
                db.SaveChanges();

                UserPassword pwd = new UserPassword();
                pwd.userName = Request.Form["username"];
                byte[] pass = UTF8Encoding.UTF8.GetBytes(Request.Form["password"]);
                byte[] hash = SHA256.Create().ComputeHash(pass);

                pwd.encryptedPassword = GetHashString(hash);

                pwd.AdministratorID = user.ID;

                db.UserPassword.Add(pwd);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(admin);
        }

        // GET: NonAdminUsers/Create
        public ActionResult CreateNonAdmin()
        {
            return View();
        }

        // POST: NonAdminUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNonAdmin([Bind(Include = "name,address,email")] NonAdminUser nonAdminUser)
        {
            if (ModelState.IsValid)
            {
                nonAdminUser.AdministratorID = (int)Session["AdministratorID"];


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
        public ActionResult EditAdmin(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Administrator admin = db.Administrator.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: NonAdminUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.


        [HttpPost, ActionName("EditAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult EditAdminPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var admin = db.Administrator.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }

            if (Request.Form["password"].Length > 0)
            {
                byte[] pass = UTF8Encoding.UTF8.GetBytes(Request.Form["password"]);
                byte[] hash = SHA256.Create().ComputeHash(pass);

                int? adminUserID = id;

                UserPassword pwd = db.UserPassword.Where(p => p.AdministratorID == adminUserID).FirstOrDefault();

                pwd.encryptedPassword = GetHashString(hash);
                db.Entry(pwd).State = EntityState.Modified;
            }

            if (TryUpdateModel(admin, "",
                new string[] { "name", "dateHired", "dateFinished" }))
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
            return View(admin);

        }

        // GET: NonAdminUsers/Edit/5
        public ActionResult EditNonAdmin(int? id)
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

        // POST: NonAdminUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.


        [HttpPost, ActionName("EditNonAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult EditNonAdminPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var nonAdminUser = db.NonAdminUser.Find(id);
            if (nonAdminUser == null)
            {
                return HttpNotFound();
            }

            if (Request.Form["password"].Length > 0)
            {
                byte[] pass = UTF8Encoding.UTF8.GetBytes(Request.Form["password"]);
                byte[] hash = SHA256.Create().ComputeHash(pass);

                int? nonAdminUserID = id;

                UserPassword pwd = db.UserPassword.Where(p => p.NonAdminUserID == nonAdminUserID).FirstOrDefault();

                pwd.encryptedPassword = GetHashString(hash);
                db.Entry(pwd).State = EntityState.Modified;
            }

            if (TryUpdateModel(nonAdminUser, "",
                new string[] { "name", "address", "email" }))
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
            return View(nonAdminUser);

        }

        // GET: NonAdminUsers/Delete/5
        public ActionResult DeleteAdmin(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Administrator admin = db.Administrator.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: NonAdminUsers/Delete/5
        [HttpPost, ActionName("DeleteAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAdminConfirmed(int id)
        {
            if ((int)Session["AdministratorID"] == id)
            {
                return RedirectToAction("Index");

            }
            Administrator admin = db.Administrator.Find(id);

            UserPassword p = db.UserPassword.Where(a => a.AdministratorID == id).First();

            db.UserPassword.Remove(p);

            db.Administrator.Remove(admin);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: NonAdminUsers/Delete/5
        public ActionResult DeleteNonAdmin(int? id)
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
        [HttpPost, ActionName("DeleteNonAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteNonAdminConfirmed(int id)
        {
            NonAdminUser nonAdminUser = db.NonAdminUser.Find(id);

            UserPassword p = db.UserPassword.Where(a => a.NonAdminUserID == id).First();

            db.UserPassword.Remove(p);

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
