using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebRole1.Controllers
{
    public class AccountController : Controller
    {
        // GET: Acount
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReadRegister()
        {
            string Name = Request.Form["name"];
            ViewBag.title = Name;
            return Login();
        }

    }

}