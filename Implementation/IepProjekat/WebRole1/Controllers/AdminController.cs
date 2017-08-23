using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebRole1.Models;

namespace WebRole1.Controllers
{
    public class AdminController : Controller
    {
        private Model1 db = new Model1();

        [HttpGet]
        public ActionResult Configuration()
        {
            if (Session["type"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Administrator")
            {
                return RedirectToAction("Logout", "Account");
            }

            var users = from m in db.Parameters select m;
            Parameter parameter = users.First();
            return View(parameter);
        }

        [HttpGet]
        public ActionResult Approvals(int Search = 0)
        {
            if (Session["type"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Administrator")
            {
                return RedirectToAction("Logout", "Account");
            }
            var users = from m in db.Users select m;
            if (Search == 0)
            {
                ViewBag.chBox = "0";
                return View(users);
            }
            else
            {
                users = users.Where(s => s.Status == 0);
                ViewBag.chBox = "1";
                return View(users);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Configuration([Bind(Include = "IdP,AnswerNumber,SilverNumber,GoldNumber,PlatinumNumber,UnlockNumber,PremiumNumber")] Parameter parameter)
        {
            if (Session["type"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Administrator")
            {
                return RedirectToAction("Logout", "Account");
            }
            if (ModelState.IsValid)
            {
                db.Entry(parameter).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Configuration", "Admin");
        }

        [HttpPost]
        public ActionResult Approvals(string email, string type)
        {
            if (Session["type"] == null)
            {
                return Content("error", "text/plain");
            }
            if (Session["type"].ToString() != "Administrator")
            {
                return Content("error", "text/plain");
            }
            if (type != "Student" && type != "Professor")
                return Content("error", "text/plain");
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any())
            {
                User user = users.First();
                user.Type = type;
                user.Status = 1;
                db.SaveChanges();
                return Content("Success", "text/plain");
            }
            return Content("error", "text/plain");
        }
    }
}