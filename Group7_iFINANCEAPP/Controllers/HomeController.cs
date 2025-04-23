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
            if (Session["NonAdminUserID"] == null && Session["AdministratorID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            
                int id = (int)Session["NonAdminUserID"];
                var user = db.NonAdminUser.Find(id);
                ViewBag.UserName = user.name;

                //ViewBag.TransactionCount = db.Transaction.Count(t => t.NonAdminUserID == id);  // update field if needed
                //ViewBag.TotalAssets = db.MasterAccount
                //                        .Where(a => a.Group.NonAdminUserID == id)
                //                        .Select(a => a.closingAmount)
                //                        .DefaultIfEmpty(0)
                //                        .Sum();

                //ViewBag.Liabilities = db.MasterAccount.Where(a => a.Group);     // placeholder
                //ViewBag.Income = ;     // placeholder
                //ViewBag.Expenses = ;  // placeholder
            

            return View(user);
        }
    }
}