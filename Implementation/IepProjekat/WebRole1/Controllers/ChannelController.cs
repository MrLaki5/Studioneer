﻿using log4net;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebRole1.Models;

namespace WebRole1.Controllers
{
    public class ChannelController : Controller
    {
        private Model1 db = new Model1();
        private ILog log = LogManager.GetLogger("log");

        //Professor:----------------------------------------------------------------------

        //Lists all proffesors channels
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Professor")
            {
                log.Error("wrong user type");
                return RedirectToAction("Logout", "Account");
            }
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
            log.Error("No users found in database");
            return RedirectToAction("Logout", "Account");
        }
        
        //List all profesors channels:ajax, open & close channel
        [HttpPost]
        public ActionResult Index(int id, string action)
        {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return Content("error2", "text/plain");
            }
            if (Session["type"].ToString() != "Professor")
            {
                log.Error("wrong user type");
                return Content("error2", "text/plain");
            }
            var channels = from m in db.Channels select m;
            channels = channels.Where(s => s.IdC == id);
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            //compare channel and user if they come from same
            if (users.Any() && channels.Any())
            {
                User user = users.First();
                Channel channel = channels.First();
                if (channel.IdU == user.IdU)
                {
                    //find witch operation
                    if (string.Compare(action, "Open") == 0)
                    {
                        if (channel.OpenTime != null)
                        {
                            log.Error("channel "+channel.Name+" already opened");
                            return Content("error2", "text/plain");
                        }
                        channel.OpenTime = DateTime.UtcNow;
                        db.SaveChanges();
                        string ret = String.Format("{0:dd/MMM/yyyy HH:mm}", channel.OpenTime);
                        return Content(ret, "text/plain");
                    }
                    if (string.Compare(action, "Close") == 0)
                    {
                        if (channel.CloseTime != null)
                        {
                            log.Error("channel " + channel.Name + " already has close time");
                            return Content("error2", "text/plain");
                        }
                        channel.CloseTime = DateTime.UtcNow;
                        db.SaveChanges();
                        string ret = String.Format("{0:dd/MMM/yyyy HH:mm}", channel.OpenTime);
                        return Content(ret, "text/plain");
                    }
                }
            }
            log.Error("inputs are wrong");
            return Content("error2", "text/plain");
        }

        //seting closing time display
        [HttpGet]
        public ActionResult TimeSet(int? id)
        {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Professor")
            {
                log.Error("wrong user type");
                return RedirectToAction("Logout", "Account");
            }
            if (id == null)
            {
                log.Error("id missing");
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
                {
                    log.Error("channel " + channel.Name + " already opened");
                    return RedirectToAction("Logout", "Account");
                }
                if (channel.CloseTime != null)
                {
                    log.Error("channel " + channel.Name + " already has close time");
                    return RedirectToAction("Logout", "Account");
                }
                if (channel.IdU == user.IdU)
                {
                    return View(channel);
                }
            }
            log.Error("user and channel not combining");
            return RedirectToAction("Logout", "Account");
        }

        //getting closing time and updateing database
        [HttpPost]
        public ActionResult TimeSet(string closeTime, string closeTime1, int id)
        {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Professor")
            {
                log.Error("wrong user type");
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
                {
                    log.Error("channel " + channel.Name + " already opened");
                    return RedirectToAction("Logout", "Account");
                }
                if (channel.CloseTime != null)
                {
                    log.Error("channel " + channel.Name + " already has close time");
                    return RedirectToAction("Logout", "Account");
                }
                if (channel.IdU != user.IdU)
                {
                    log.Error("channel and user not combining");
                    return RedirectToAction("Logout", "Account");
                }
            }
            else
            {
                log.Error("channel or user missing");
                return RedirectToAction("Logout", "Account");
            }
            string date = closeTime + " " + closeTime1;
            DateTime dt = DateTime.Parse(date);
            if (DateTime.UtcNow > dt) {
                log.Error("end time has expired");
                ViewBag.error = "Date has expired";
                return View(channel);
            }
            channel.CloseTime = dt;
            db.SaveChanges();
            return RedirectToAction("Index", "Channel");
        }

        //creating channel display page
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Professor")
            {
                log.Error("wrong user type");
                return RedirectToAction("Logout", "Account");
            }
            return View();
        }

        //creating channel:ajax 
        [HttpPost]
        public ActionResult Create(string name, string password, string cpassword)
        {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return Content("error2", "text/plain");
            }
            if (Session["type"].ToString() != "Professor")
            {
                log.Error("wrong user type");
                return Content("error2", "text/plain");
            }
            if (String.IsNullOrEmpty(name))
            {
                return Content("Channel name required", "text/plain");
            }
            if (String.IsNullOrEmpty(password))
            {
                return Content("Password is required", "text/plain");
            }
            if (String.IsNullOrEmpty(cpassword))
            {
                return Content("Confirmation password is required", "text/plain");
            }
            if (string.Compare(password, cpassword) != 0)
            {
                return Content("Passwords don't match", "text/plain");
            }
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
                log.Error("user not found");
                return Content("error2", "text/plain");
            }
            db.Channels.Add(chann);
            db.SaveChanges();
            return Content("Success", "text/plain");
        }

        //all stories published on one channel
        [HttpGet]
        public ActionResult History(int? id)
        {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Professor")
            {
                log.Error("wrong user type");
                return RedirectToAction("Logout", "Account");
            }
            if (id == null)
            {
                log.Error("id missing");
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
                {
                    log.Error("channel " + channel.Name + " not opened");
                    return RedirectToAction("Logout", "Account");
                }
                if (channel.IdU != user.IdU)
                {
                    log.Error("channel and user not combinable");
                    return RedirectToAction("Logout", "Account");
                }
                var published = from m in db.Publisheds select m;
                ViewBag.chann = channel.IdC;
                published = published.Where(s => s.IdC == id);
                return View(published);
            }
            log.Error("user or channel not found in database");
            return RedirectToAction("Logout", "Account");
        }

        //all answers for one story on one channel
        [HttpGet]
        public ActionResult StAnswers(int? chann, int? publish)
        {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Professor")
            {
                log.Error("wrong user type");
                return RedirectToAction("Logout", "Account");
            }
            if ((chann == null) || (publish == null))
            {
                log.Error("parameters are missing");
                return RedirectToAction("Logout", "Account");
            }
            var channels = from m in db.Channels select m;
            channels = channels.Where(s => s.IdC == chann);
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any() && channels.Any())
            {
                User user = users.First();
                Channel channel = channels.First();
                if (channel.OpenTime == null)
                {
                    log.Error("channel " + channel.Name + " not opened");
                    return RedirectToAction("Logout", "Account");
                }
                if (channel.IdU != user.IdU)
                {
                    log.Error("user and channel not combinable");
                    return RedirectToAction("Logout", "Account");
                }
                var published = from m in db.Publisheds select m;
                published = published.Where(s => s.IdC == chann);
                published = published.Where(s => s.IdPub == publish);
                ViewBag.chann = channel.IdC;
                if (published.Any())
                {
                    Published pub = published.First();
                    var responses = from m in db.Responses select m;
                    responses = responses.Where(s => s.IdP == pub.IdP);
                    responses = responses.Where(s => s.IdC == channel.IdC);
                    return View(responses);
                }
            }
            log.Error("user or channel or published missing in database");
            return RedirectToAction("Logout", "Account");
        }

        //Student:----------------------------------------------------------------------

        //display all active channels that user is not subscribed to
        [HttpGet]
        public ActionResult ActiveChannels()
        {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Student")
            {
                log.Error("wrong user type");
                return RedirectToAction("Logout", "Account");
            }
            var channels = from m in db.Channels select m;
            var subscription = from m in db.Subscriptions select m;
            var users = from m in db.Users select m;
            string email = Session["email"].ToString();
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any())
            {
                User user = users.First();
                subscription = subscription.Where(s => s.IdU == user.IdU);
                channels = channels.Where(s => s.OpenTime != null);
                channels = channels.Where(s => ((s.CloseTime == null) || (s.CloseTime > DateTime.UtcNow)));
                channels = channels.Where(s => !subscription.Any(s2 => s2.IdC == s.IdC));
                return View(channels);
            }
            log.Error("user not found in database");
            return RedirectToAction("Logout", "Account");
        }

        //display form for entering channel password
        [HttpGet]
        public ActionResult EnterPass(int? id) {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Student")
            {
                log.Error("wrong user type");
                return RedirectToAction("Logout", "Account");
            }
            if (id == null)
            {
                log.Error("arguments missing");
                return RedirectToAction("Logout", "Account");
            }
            var channels = from m in db.Channels select m;
            var subscription = from m in db.Subscriptions select m;
            var users = from m in db.Users select m;
            string email = Session["email"].ToString();
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any())
            {
                User user = users.First();
                subscription = subscription.Where(s => s.IdU == user.IdU);
                channels = channels.Where(s => s.OpenTime != null);
                channels = channels.Where(s => ((s.CloseTime == null) || (s.CloseTime > DateTime.UtcNow)));
                channels = channels.Where(s => !subscription.Any(s2 => s2.IdC == s.IdC));
                channels = channels.Where(s => s.IdC == id);
                if (channels.Any()) {
                    return View(channels.First());
                }
            }
            log.Error("user missing in database");
            return RedirectToAction("Logout", "Account");
        }

        //checks password on channel
        [HttpPost]
        public ActionResult EnterPass(int id, string password)
        {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Student")
            {
                log.Error("wrong user type");
                return RedirectToAction("Logout", "Account");
            }
            var channels = from m in db.Channels select m;
            var subscription = from m in db.Subscriptions select m;
            var users = from m in db.Users select m;
            string email = Session["email"].ToString();
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any())
            {
                User user = users.First();
                subscription = subscription.Where(s => s.IdU == user.IdU);
                channels = channels.Where(s => s.OpenTime != null);
                channels = channels.Where(s => ((s.CloseTime == null) || (s.CloseTime > DateTime.UtcNow)));
                channels = channels.Where(s => !subscription.Any(s2 => s2.IdC == s.IdC));
                channels = channels.Where(s => s.IdC == id);
                if (channels.Any())
                {
                    Channel chan = channels.First();
                    string rpass = chan.Password;
                    string npass= FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
                    if (string.Compare(npass, rpass) != 0)
                    {
                        ViewBag.error = "Wrong password";
                        return View(chan);
                    }                    
                    Subscription sub = new Subscription();
                    sub.IdC = chan.IdC;
                    sub.IdU = user.IdU;
                    sub.IsPremium = 0;
                    db.Subscriptions.Add(sub);
                    db.SaveChanges();
                    ViewBag.succ = 1;
                    return View(chan);
                }
            }
            log.Error("missing user or channel in database");
            return RedirectToAction("Logout", "Account");
        }

        //display all users subscribed channels
        [HttpGet]
        public ActionResult Subscribed()
        {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Student")
            {
                log.Error("wrong user type");
                return RedirectToAction("Logout", "Account");
            }
            var subscription = from m in db.Subscriptions select m;
            var users = from m in db.Users select m;
            string email = Session["email"].ToString();
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any())
            {
                User user = users.First();
                subscription = subscription.Where(s => s.IdU == user.IdU);
                subscription = subscription.Where(s => s.Channel.OpenTime != null);
                subscription = subscription.Where(s => ((s.Channel.CloseTime == null) || (s.Channel.CloseTime > DateTime.UtcNow)));
                return View(subscription);
            }
            log.Error("user not found in database");
            return RedirectToAction("Logout", "Account");
        }

        //subscribed channels:ajax premium activate deactivate
        [HttpPost]
        public ActionResult Subscribed(int id, string func)
        {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return Content("error2", "text/plain");
            }
            if (Session["type"].ToString() != "Student")
            {
                log.Error("wrong user type");
                return Content("error2", "text/plain");
            }
            var subscription = from m in db.Subscriptions select m;
            var users = from m in db.Users select m;
            string email = Session["email"].ToString();
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any())
            {
                User user = users.First();
                subscription = subscription.Where(s => s.IdU == user.IdU);
                subscription = subscription.Where(s => s.Channel.OpenTime != null);
                subscription = subscription.Where(s => ((s.Channel.CloseTime == null) || (s.Channel.CloseTime > DateTime.UtcNow)));
                subscription = subscription.Where(s => s.IdC == id);
                if (subscription.Any())
                {
                    Subscription sub = subscription.First();
                    if (string.Compare(func, "Activate premium") == 0)
                    {
                        sub.IsPremium = 1;
                        db.SaveChanges();
                        return Content("success", "text/plain");
                    }
                    if (string.Compare(func, "Deactivate premium") == 0)
                    {
                        sub.IsPremium = 0;
                        db.SaveChanges();
                        return Content("success", "text/plain");
                    }
                }
            }
            log.Error("user or subscription not found in database");
            return Content("error2", "text/plain");
        }
    }
}