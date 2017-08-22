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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}