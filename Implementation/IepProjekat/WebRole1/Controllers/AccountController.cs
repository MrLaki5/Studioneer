﻿using log4net;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebRole1.Models;

namespace WebRole1.Controllers
{
    public class AccountController : Controller
    {
        private Model1 db = new Model1();
        private ILog log = LogManager.GetLogger("log");

        //login page display
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        //register page display
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        //account info display
        [HttpGet]
        public ActionResult Info()
        {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() == "Administrator")
            {
                log.Error("wrong user type");
                return RedirectToAction("Logout", "Account");
            }
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any())
            {
                return View(users.First());
            }
            log.Error("No users found in database");
            return RedirectToAction("Login", "Account");
        }

        //logout and sesion clear
        [HttpGet]
        public ActionResult Logout() {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        //login:ajax data check
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (String.IsNullOrEmpty(email))
            {
                return Content("Enter email", "text/plain");
            }
            if (String.IsNullOrEmpty(password))
            {
                return Content("Enter password", "text/plain");
            }
            try
            {
                var emailChecked = new System.Net.Mail.MailAddress(email);
            }
            catch
            {
                return Content("Email is not valid", "text/plain");
            }
            //checking if password is correct
            string password1= FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email)).Where(s=> s.Password.Equals(password1));
            if (users.Any())
            {
                User user = users.First();
                //checking if user is confirmed in system
                if (user.Status == 0)
                {
                    return Content("Registration not confirmed", "text/plain");
                }
                Session["username"] = user.Name;
                Session["email"] = user.Mail;
                Session["type"] = user.Type;
                Session["token"] = user.Balans.ToString();
                return Content("Success", "text/plain");
            }
            return Content("Wrong email or password", "text/plain");
        }

        //register:ajax data check
        [HttpPost]
        public ActionResult Register(string email, string name, string lastname, string password, string cpassword)
        {
            if (String.IsNullOrEmpty(email))
            {
                return Content("Enter email", "text/plain");
            }
            if (String.IsNullOrEmpty(name))
            {
                return Content("Enter username", "text/plain");
            }
            if (String.IsNullOrEmpty(password))
            {
                return Content("Enter password", "text/plain");
            }
            if (String.IsNullOrEmpty(cpassword))
            {
                return Content("Enter confirmation of password", "text/plain");
            }
            if (string.Compare(password, cpassword) != 0)
            {
                return Content("Passwords don't match", "text/plain");
            }
            try
            {
                var emailChecked = new System.Net.Mail.MailAddress(email);
            }
            catch
            {
                return Content("Email is not valid", "text/plain");
            }
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any())
            {
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