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
    public class ManageUsersController : Controller
    {
        private Group7_iFINANCEDBEntities db = new Group7_iFINANCEDBEntities();

        // GET: ManageUsers
        public ActionResult Index(string searchString, string tab = "nonadmin")
        {
            // If not authenticated, redirect to login page

            if (Session["AdministratorID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var nonAdminUser = db.NonAdminUser.Include(n => n.Administrator).AsQueryable();

            var adminUser = db.Administrator.AsQueryable();

            // Apply search filtering

            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                if (tab == "admin")
                {
                    adminUser = adminUser.Where(a => a.name.ToLower().Contains(searchString));
                }
                else
                {
                    nonAdminUser = nonAdminUser.Where(n => n.name.ToLower().Contains(searchString));
                }
            }

            var viewModel = new ManageUsersViewModel();
            
            viewModel.Administrators = adminUser.ToList();
            viewModel.NonAdminUsers = nonAdminUser.ToList();
            viewModel.ActiveTab = tab;

            return View(viewModel);
        }

        // GET: ManageUsers/Details/5
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

        // GET: ManageUsers/Details/5
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

        // GET: ManageUsers/CreateAdmin
        public ActionResult CreateAdmin()
        {
            return View();
        }

        // POST: ManageUsers/CreateAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAdmin([Bind(Include = "name,dateHired,dateFinished")] Administrator admin)
        {
            if (ModelState.IsValid)
            {
                Administrator user = db.Administrator.Add(admin);
                db.SaveChanges();

                // Create associated UserPassword entry

                UserPassword pwd = new UserPassword();
                pwd.userName = Request.Form["username"];

                // Encrypt password before storing in database
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

        // GET: ManageUsers/CreateNonAdmin
        public ActionResult CreateNonAdmin()
        {
            return View();
        }

        // POST: ManageUsers/CreateNonAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNonAdmin([Bind(Include = "name,address,email")] NonAdminUser nonAdminUser)
        {
            if (ModelState.IsValid)
            {
                nonAdminUser.AdministratorID = (int)Session["AdministratorID"];


                NonAdminUser user = db.NonAdminUser.Add(nonAdminUser);
                db.SaveChanges();

                // Create associated UserPassword entry

                UserPassword pwd = new UserPassword();
                pwd.userName = Request.Form["username"];

                // Encrypt password before storing in database

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

        // Helper function to get a hashed string, used by LoginController as well
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

        // GET: ManageUsers/EditAdmin/5
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

        // POST: ManageUsers/EditAdmin/5
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

            // Only update password if a value was entered
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

        // GET: ManageUsers/EditNonAdmin/5
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

            UserPassword pwd = db.UserPassword.Where(p => p.NonAdminUserID == id).FirstOrDefault();

            ViewBag.passwordExpiryDate = pwd.passwordExpiryTime;
            ViewBag.userAccountExpiryDate = pwd.userAccountExpiryDate;

            return View(nonAdminUser);
        }

        // POST: ManageUsers/EditNonAdmin/5
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

            // Only update password if a value was entered
            UserPassword pwd = db.UserPassword.Where(p => p.NonAdminUserID == id).FirstOrDefault();
            if (Request.Form["password"].Length > 0)
            {
                byte[] pass = UTF8Encoding.UTF8.GetBytes(Request.Form["password"]);
                byte[] hash = SHA256.Create().ComputeHash(pass);

                int? nonAdminUserID = id;

                pwd = db.UserPassword.Where(p => p.NonAdminUserID == nonAdminUserID).FirstOrDefault();

                pwd.encryptedPassword = GetHashString(hash);

                db.Entry(pwd).State = EntityState.Modified;
            }

            var s = Request.Form["userAccountExpiryDate"];

            if (s.Length > 0)
            {
                pwd.userAccountExpiryDate = DateTime.Parse(Request.Form["userAccountExpiryDate"]);
            }

            s = Request.Form["passwordExpiryDate"];

            if (s.Length > 0)
            {

                pwd.passwordExpiryTime = DateTime.Parse(Request.Form["passwordExpiryDate"]);
            }
            db.Entry(pwd).State = EntityState.Modified;


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

        // GET: ManageUsers/DeleteAdmin/5
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

        // POST: ManageUsers/DeleteAdmin/5
        [HttpPost, ActionName("DeleteAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAdminConfirmed(int id)
        {
            if ((int)Session["AdministratorID"] == id)
            {
                return RedirectToAction("Index");

            }
            Administrator admin = db.Administrator.Find(id);

            // Delete associated UserPassword entry as well
            UserPassword p = db.UserPassword.Where(a => a.AdministratorID == id).First();

            db.UserPassword.Remove(p);

            db.Administrator.Remove(admin);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: ManageUsers/DeleteNonAdmin/5
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

        // POST: ManageUsers/DeleteNonAdmin/5
        [HttpPost, ActionName("DeleteNonAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteNonAdminConfirmed(int id)
        {
            NonAdminUser nonAdminUser = db.NonAdminUser.Find(id);

            // Delete associated UserPassword entry as well
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
