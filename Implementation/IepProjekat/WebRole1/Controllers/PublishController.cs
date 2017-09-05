﻿using log4net;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebRole1.Hubs;
using WebRole1.Models;

namespace WebRole1.Controllers
{
    public class PublishController : Controller
    {

        private Model1 db = new Model1();
        private ILog log = LogManager.GetLogger("log");

        //Professor:----------------------------------------------------------------------

        //Displays all professors active channels for publishing the story
        [HttpGet]
        public ActionResult Index(int? id)
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
            if (id == null) {
                log.Error("id argument missing");
                return RedirectToAction("Logout", "Account");
            }

            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any())
            {
                User user = users.First();
                int idU = user.IdU;
                var questions = from m in db.Questions select m;
                var channels = from m in db.Channels select m;
                channels = channels.Where(s => s.IdU == idU);
                channels = channels.Where(s => s.OpenTime != null);
                channels = channels.Where(s => ((s.CloseTime == null) || (s.CloseTime > DateTime.UtcNow)));
                questions = questions.Where(s => s.IdU == idU);
                questions = questions.Where(s => s.IsClone == 0);
                questions = questions.Where(s => s.IsLocked == 1);
                questions = questions.Where(s => s.IdP == id);
                if (questions.Any())
                {
                    ViewBag.idQ = id;
                    return View(channels);
                }
            }
            log.Error("user missing in db");
            return RedirectToAction("Logout", "Account");
        }

        //Updates database and sends SignalR with new published story on channel
        [HttpPost]
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
            if (Request.Form["idQ"] == null)
            {
                log.Error("arguments missing");
                return RedirectToAction("Logout", "Account");
            }
            string temp= Request.Form["idQ"];
            int idP=0;
            if (!Int32.TryParse(temp, out idP))
            {
                log.Error("idQ not integer");
                return RedirectToAction("Logout", "Account");
            }

            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any())
            {
                User user = users.First();
                int idU = user.IdU;
                var questions = from m in db.Questions select m;
                var channels = from m in db.Channels select m;
                channels = channels.Where(s => s.IdU == idU);
                channels = channels.Where(s => s.OpenTime != null);
                channels = channels.Where(s => ((s.CloseTime == null) || (s.CloseTime > DateTime.UtcNow)));
                questions = questions.Where(s => s.IdU == idU);
                questions = questions.Where(s => s.IsClone == 0);
                questions = questions.Where(s => s.IsLocked == 1);
                questions = questions.Where(s => s.IdP == idP);
                if (questions.Any())
                {
                    Question question = new Question();
                    Question questionOld = questions.First();
                    question.Title = questionOld.Title;
                    question.Text = questionOld.Text;
                    question.IdU = questionOld.IdU;
                    question.IsClone = 1;
                    question.IsLocked = 1;
                    question.CreationTime = questionOld.CreationTime;
                    question.LastLock = questionOld.LastLock;
                    if (questionOld.Image != null) {
                        question.Image = questionOld.Image.ToArray();
                    }
                    List<Published> PubList = new List<Published>();
                    List<Answer> AnsList = new List<Answer>();


                    ViewBag.idQ = idP;
                    if (channels.Any())
                    {
                        int selBool = 0;
                        int numberOfChan = channels.Count();
                        string tempN = "";
                        for (int i = 0; i < numberOfChan; i++) {
                            tempN = "box " + i.ToString();
                            if (Request.Form[tempN] != null) {
                                tempN = Request.Form[tempN];
                                var channels1 = channels;
                                channels1 = channels1.Where(s => s.IdC.ToString().Equals(tempN));
                                if (channels1.Any()) {
                                    Channel chann = channels1.First();
                                    if (selBool == 0) {
                                        db.Questions.Add(question);
                                        db.SaveChanges();
                                        selBool = 1;
                                    }
                                    Published pub = new Published
                                    {
                                        PubTime = DateTime.UtcNow,
                                        IdC = chann.IdC,
                                        IdP = question.IdP
                                    };
                                    db.Publisheds.Add(pub);
                                    PubList.Add(pub);
                                    //db.SaveChanges();
                                }
                            }
                        }

                        if (selBool == 1)
                        {
                            foreach (var item in questionOld.Answers)
                            {
                                Answer ans = new Answer();
                                ans.IsCorrect = item.IsCorrect;
                                ans.Number = item.Number;
                                ans.Tag = item.Tag;
                                ans.Text = item.Text;
                                ans.IdP = question.IdP;
                                db.Answers.Add(ans);
                                AnsList.Add(ans);
                                //db.SaveChanges();
                            }
                            db.SaveChanges();
                            foreach (var item in PubList) {
                                string group = item.Channel.Name + item.Channel.Password;
                                group= FormsAuthentication.HashPasswordForStoringInConfigFile(group, "SHA1");

                                string idChann = item.Channel.IdC.ToString();
                                string idQuest = item.IdP.ToString();
                                string channame = item.Channel.Name;
                                string title = question.Title;
                                string text = question.Text;
                                string time = String.Format("{0:dd/MMM/yyyy HH:mm}", item.PubTime);
                                string answers = "";
                                int k = AnsList.Count;
                                for (int i=0; i<k;i++) {
                                    answers += AnsList.ElementAt(i).Tag + ") " + AnsList.ElementAt(i).Text;
                                    if ((i + 1) != k) {
                                        answers += "<NewAns>";
                                    }
                                }
                                var hubContext = GlobalHost.ConnectionManager.GetHubContext<BoardHub>();
                                hubContext.Clients.Group(group).boardfresh(idQuest, idChann, channame, title, text, answers, time);
                            }
                            ViewBag.msg = "Success";
                        }
                        else {
                            ViewBag.msg = "No channels selected";
                        }
                        return View(channels);
                    }
                    else
                    {
                        ViewBag.msg = "No channels selected";
                        return View(channels);

                    }
                }
            }
            log.Error("user, channel and question not mergable in db");
            return RedirectToAction("Logout", "Account");
        }

        //Student:------------------------------------------------------------------------

        //loads all unanswerd questions for all active channels
        [HttpGet]
        public ActionResult Blackboard() {
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

                var channels = from m in db.Channels select m;
                channels = channels.Where(s => subscription.Any(s2 => s2.IdC == s.IdC));

                var publisheds= from m in db.Publisheds select m;
                publisheds= publisheds.Where(s => channels.Any(s2 => s2.IdC == s.IdC));

                var responses = from m in db.Responses select m;
                responses = responses.Where(s => s.IdU == user.IdU);

                publisheds = publisheds.Where(s => !responses.Any(s2 => s2.IdC == s.IdC && s2.IdP == s.IdP));


                Channel []ch = channels.ToArray();
                List<String> tempL = new List<string>();
                string tempStr = "";
                foreach (var item in ch) {
                    tempStr = item.Name + item.Password;
                    tempStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tempStr, "SHA1");
                    tempL.Add(tempStr);
                }
                ViewBag.chann = tempL;
                return View(publisheds);
            }
            log.Error("input variables wrong");
            return RedirectToAction("Logout", "Account");
        }
    }
}