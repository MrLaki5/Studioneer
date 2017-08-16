using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using WebRole1.Models;

namespace WebRole1.Controllers
{

    public class AccountController : Controller
    {
        private Model1 db = new Model1();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (String.IsNullOrEmpty(email))
                return Content("Enter email", "text/plain");
            if (String.IsNullOrEmpty(password))
                return Content("Enter password", "text/plain");
            try
            {
                var emailChecked = new System.Net.Mail.MailAddress(email);
            }
            catch
            {
                return Content("Email is not valid", "text/plain");
            }

            return Content("Proba123", "text/plain");
        }

        [HttpPost]
        public ActionResult Register(string email, string name, string password, string cpassword)
        {
            if (String.IsNullOrEmpty(email))
                return Content("Enter email", "text/plain");
            if (String.IsNullOrEmpty(name))
                return Content("Enter username", "text/plain");
            if (String.IsNullOrEmpty(password))
                return Content("Enter password", "text/plain");
            if (String.IsNullOrEmpty(cpassword))
                return Content("Enter confirmation of password", "text/plain");
            if (string.Compare(password, cpassword) != 0)
                return Content("Passwords don't match", "text/plain");
            try
            {
                var emailChecked = new System.Net.Mail.MailAddress(email);
            }
            catch {
                return Content("Email is not valid", "text/plain");
            }

            User user = new User();
            user.Mail = email;
            user.Name = name;
            user.Lastname = "kurac";
            user.Password = password;
            user.Balans = 0;
            user.Type = "User";

            db.User.Add(user);
            db.SaveChanges();

            return Content("Proba123", "text/plain");
        }

    }

}