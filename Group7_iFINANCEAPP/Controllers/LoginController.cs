using Group7_iFINANCEAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Group7_iFINANCEAPP.Controllers
{
    public class LoginController : Controller
    {
        private Group7_iFINANCEDBEntities db = new Group7_iFINANCEDBEntities();

        // GET: Login
        public ActionResult Index()
        {
            // When logging in, user is not yet authenticated
            Session["AdministratorID"] = null;
            Session["NonAdminUserID"] = null;
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string username, string password)
        {
            // Find the hash that matches the provided password
            byte[] pass = UTF8Encoding.UTF8.GetBytes(Request.Form["password"]);
            byte[] hash = SHA256.Create().ComputeHash(pass);

            string encryptedPassword = NonAdminUsersController.GetHashString(hash);

            UserPassword p = db.UserPassword.Where(a => a.userName == username).Where(a => a.encryptedPassword == encryptedPassword).FirstOrDefault();
            
            // If found, authenticate the user
            if (p != null)
            {
                Session["NonAdminUserID"] = p.NonAdminUserID;
                Session["AdministratorID"] = p.AdministratorID;
                // NonAdmin goes to home page
                if (p.NonAdminUserID != null)
                    return RedirectToAction("Index", "Home");
                // Admin goes to Manage Users page
                else
                    return RedirectToAction("Index", "NonAdminUsers");
            }
            else
            {
                ViewBag.ValidationError = "Invalid username or password.";
                return View();
            }

        }

        // GET: Login/Reset
        public ActionResult Reset()
        {
            return View();
        }

        // POST: Login/Reset
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reset(string password)
        {
            // Generate the hash for the new password to store in database
            byte[] pass = UTF8Encoding.UTF8.GetBytes(Request.Form["password"]);
            byte[] hash = SHA256.Create().ComputeHash(pass);

            string encryptedPassword = NonAdminUsersController.GetHashString(hash);

            UserPassword p;

            int id;

            // Find the UserPassword entry of the currently authenticated user
            if (Session["NonAdminUserID"] != null)
            {
                id = (int)Session["NonAdminUserID"];
                p = db.UserPassword.Where(a => a.NonAdminUserID == id).FirstOrDefault();
            }
            else
            {
                id = (int)Session["AdministratorID"];
                p = db.UserPassword.Where(a => a.AdministratorID == id).FirstOrDefault();
            }
            
            // Update password
            p.encryptedPassword = encryptedPassword;
            db.Entry(p).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
 
    }
}
