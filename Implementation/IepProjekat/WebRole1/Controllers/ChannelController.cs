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

        [HttpPost]
        public ActionResult Index(int id, string action)
        {
            if (Session["type"] == null)
                return Content("error2", "text/plain");
            if (Session["type"].ToString() != "Professor")
                return Content("error2", "text/plain");
            var channels = from m in db.Channels select m;
            channels = channels.Where(s => s.IdC == id);
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any() && channels.Any())
            {
                User user = users.First();
                Channel channel = channels.First();
                
                if (channel.IdU == user.IdU)
                {
                    if (string.Compare(action, "Open") == 0)
                    {
                        if (channel.OpenTime != null)
                            return Content("error2", "text/plain");
                        channel.OpenTime = DateTime.UtcNow;
                        db.SaveChanges();
                        string ret = String.Format("{0:dd/MMM/yyyy HH:mm}", channel.OpenTime);
                        return Content(ret, "text/plain");
                    }
                    if (string.Compare(action, "Close") == 0)
                    {
                        if (channel.CloseTime != null)
                            return Content("error2", "text/plain");
                        channel.CloseTime = DateTime.UtcNow;
                        db.SaveChanges();
                        string ret = String.Format("{0:dd/MMM/yyyy HH:mm}", channel.OpenTime);
                        return Content(ret, "text/plain");
                    }
                }
            }
            return Content("error2", "text/plain");
        }

        [HttpGet]
        public ActionResult timeSet(int? id)
        {
            if (Session["type"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Professor")
            {
                return RedirectToAction("Logout", "Account");
            }
            if (id == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            var channels = from m in db.Channels select m;
            channels = channels.Where(s => s.IdC == id);
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));

            if (users.Any() && channels.Any())
            {
                User user = users.First();
                Channel channel = channels.First();
                if (channel.OpenTime == null)
                    return RedirectToAction("Logout", "Account");
                if (channel.CloseTime!=null)
                    return RedirectToAction("Logout", "Account");
                if (channel.IdU == user.IdU)
                    return View(channel);
            }
            return RedirectToAction("Logout", "Account");
        }

        [HttpPost]
        public ActionResult timeSet(string closeTime, string closeTime1, int id) {
            if (Session["type"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Professor")
            {
                return RedirectToAction("Logout", "Account");
            }
            var channels = from m in db.Channels select m;
            channels = channels.Where(s => s.IdC == id);
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            Channel channel = null;
            if (users.Any() && channels.Any())
            {
                User user = users.First();
                channel = channels.First();
                if (channel.OpenTime == null)
                    return RedirectToAction("Logout", "Account");
                if (channel.CloseTime != null)
                    return RedirectToAction("Logout", "Account");
                if (channel.IdU != user.IdU)
                    return RedirectToAction("Logout", "Account");
            }
            else
            {
                return RedirectToAction("Logout", "Account");
            }
            string date = closeTime + " " + closeTime1;
            DateTime dt = DateTime.Parse(date);
            if (DateTime.UtcNow > dt) {
                ViewBag.error = "Date has expired";
                return View(channel);
            }
            channel.CloseTime = dt;
            db.SaveChanges();
            return RedirectToAction("Index", "Channel");
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