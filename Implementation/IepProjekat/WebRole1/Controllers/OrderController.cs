using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebRole1.Models;

namespace WebRole1.Controllers
{
    public class OrderController : Controller
    {

        private Model1 db = new Model1();

        [HttpGet]
        public ActionResult Index()
        {

            if (Session["type"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() == "Administrator")
            {
                return RedirectToAction("Logout", "Account");
            }
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            var parameters = from m in db.Parameters select m;
            if (parameters.Any())
            {
                Parameter par = parameters.First();
                ViewBag.silver = par.SilverNumber;
                ViewBag.gold = par.GoldNumber;
                ViewBag.platinum = par.PlatinumNumber;
                if (users.Any())
                {
                    return View();
                }
            }         
            return RedirectToAction("Login", "Account");
        }
    }
}