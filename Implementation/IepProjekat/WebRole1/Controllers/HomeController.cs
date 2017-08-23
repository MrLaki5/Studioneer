using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebRole1.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Login()
        {
            if (Session["type"] == null) {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() == "Administrator")
            {
                return RedirectToAction("Configuration", "Admin");
            }
            return RedirectToAction("Info", "Account");
        }
    }
}