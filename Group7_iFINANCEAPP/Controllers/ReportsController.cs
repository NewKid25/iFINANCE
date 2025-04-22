using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Group7_iFINANCEAPP.Controllers
{
    public class ReportsController : Controller
    {

        public int[] junk = new int[] { 1, 2, 3 };
        // GET: Reports
        public ActionResult Index()
        {
            if (Session["NonAdminUserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View(junk);
        }

        public ActionResult ProfitLossReport()
        {
            return View(junk);
        }

        public ActionResult TrialBalance()
        {
            return View();
        }
    }

   

    public enum ReportType
    {
        NONE,
        TRIALBALANCE,
        PROFITLOSS
    }
}
