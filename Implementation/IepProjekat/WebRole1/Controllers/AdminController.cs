using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebRole1.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public ActionResult Configuration()
        {
            if (Session["type"] == null) {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Administrator") {
                return RedirectToAction("Logout", "Account");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Approvals()
        {
            if (Session["type"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Administrator")
            {
                return RedirectToAction("Logout", "Account");
            }
            return View();
        }

    }
}