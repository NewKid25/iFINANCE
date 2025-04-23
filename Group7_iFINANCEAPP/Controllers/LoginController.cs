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
            Session["AdministratorID"] = null;
            Session["NonAdminUserID"] = null;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string username, string password)
        {
            byte[] pass = UTF8Encoding.UTF8.GetBytes(Request.Form["password"]);
            byte[] hash = SHA256.Create().ComputeHash(pass);

            string encryptedPassword = NonAdminUsersController.GetHashString(hash);

            UserPassword p = db.UserPassword.Where(a => a.userName == username).Where(a => a.encryptedPassword == encryptedPassword).FirstOrDefault();

            if (p != null)
            {
                Session["NonAdminUserID"] = p.NonAdminUserID;
                Session["AdministratorID"] = p.AdministratorID;
                if (p.NonAdminUserID != null)
                    return RedirectToAction("Index", "Home");
                else
                    return RedirectToAction("Index", "NonAdminUsers");
            }
            else
            {
                ViewBag.ValidationError = "Invalid username or password.";
                return View();
            }

        }

        public ActionResult Reset()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reset(string password)
        {
            byte[] pass = UTF8Encoding.UTF8.GetBytes(Request.Form["password"]);
            byte[] hash = SHA256.Create().ComputeHash(pass);

            string encryptedPassword = NonAdminUsersController.GetHashString(hash);

            UserPassword p;

            int id;

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

            p.encryptedPassword = encryptedPassword;
            db.Entry(p).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
 
    }
}
