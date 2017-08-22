using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using WebRole1.Models;
using System.Web.Security;

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
        [HttpGet]
        public ActionResult Info()
        {
            if (Session["username"]==null)
                return RedirectToAction("Login", "Account");
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any())
            {
                return View(users.First());
            }
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Logout() {
            Session.Clear();
            return RedirectToAction("Login", "Account");
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
            string password1= FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email)).Where(s=> s.Password.Equals(password1));
            if (users.Any())
            {
                User user = users.First();
                if (user.Status==0)
                    return Content("Registration not confirmed", "text/plain");
                Session["username"] = user.Name;
                Session["email"] = user.Mail;
                Session["type"] = user.Type;
                Session["token"] = user.Balans.ToString();
                return Content("Success", "text/plain");
            }

            return Content("Wrong email or password", "text/plain");

        }

        [HttpPost]
        public ActionResult Register(string email, string name, string lastname, string password, string cpassword)
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

            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any()) {
                return Content("Email is taken", "text/plain");
            }


            User user = new User();
            user.Mail = email;
            user.Name = name;
            user.Lastname = lastname;
            user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
            user.Balans = 0;
            user.Status = 0;
            user.Type = "Undefined";

            db.Users.Add(user);
            db.SaveChanges();

            return Content("Created", "text/plain");
        }

    }

}