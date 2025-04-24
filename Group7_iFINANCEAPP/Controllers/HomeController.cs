using Group7_iFINANCEAPP.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Group7_iFINANCEAPP.Controllers
{
    public class HomeController : Controller
    {
        private Group7_iFINANCEDBEntities db = new Group7_iFINANCEDBEntities();
        public ActionResult Index()
        {
            // Redirect if not authenticated -- Admin only sees Manage Users
            if (Session["NonAdminUserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            int id = (int)Session["NonAdminUserID"];
            var user = db.NonAdminUser.Find(id);
            ViewBag.UserName = user.name;

            return View(user);
        }
    }
}