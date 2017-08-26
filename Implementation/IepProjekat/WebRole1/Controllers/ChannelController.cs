using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebRole1.Models;

namespace WebRole1.Controllers
{
    public class ChannelController : Controller
    {
        private Model1 db = new Model1();

        [HttpGet]
        public ActionResult Index()
        {
            if (Session["type"] == null)
                return RedirectToAction("Login", "Account");
            if (Session["type"].ToString() != "Professor")
                return RedirectToAction("Logout", "Account");
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any())
            {
                User user = users.First();
                int id = user.IdU;
                var channels = from m in db.Channels select m;
                channels = channels.Where(s => s.IdU == id);
                return View(channels);
            }
            return RedirectToAction("Logout", "Account");
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["type"] == null)
                return RedirectToAction("Login", "Account");
            if (Session["type"].ToString() != "Professor")
                return RedirectToAction("Logout", "Account");
            return View();
        }

        [HttpPost]
        public ActionResult Create(string name, string password, string cpassword)
        {
            if (Session["type"] == null)
                return RedirectToAction("Login", "Account");
            if (Session["type"].ToString() != "Professor")
                return RedirectToAction("Logout", "Account");
            if (String.IsNullOrEmpty(name))
                return Content("Channel name required", "text/plain");
            if (String.IsNullOrEmpty(password))
                return Content("Password is required", "text/plain");
            if (String.IsNullOrEmpty(cpassword))
                return Content("Confirmation password is required", "text/plain");
            if (string.Compare(password, cpassword) != 0)
                return Content("Passwords don't match", "text/plain");

            var channels = from m in db.Channels select m;
            channels = channels.Where(s => s.Name.Equals(name));
            if (channels.Any())
            {
                return Content("Channel name is taken", "text/plain");
            }
            Channel chann = new Channel();
            chann.Name = name;
            chann.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any())
            {
                User user = users.First();
                chann.IdU = user.IdU;
            }
            else
            {
                return RedirectToAction("Logout", "Account");
            }
            db.Channels.Add(chann);
            db.SaveChanges();
            return Content("Success", "text/plain");
        }
    }
}