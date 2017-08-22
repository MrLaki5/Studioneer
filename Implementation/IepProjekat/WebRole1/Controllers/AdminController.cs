﻿using System;
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
            if (Session["type"] == null) {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Administrator") {
                return RedirectToAction("Logout", "Account");
            }

            var users = from m in db.Parameters select m;
            Parameter parameter = users.First();
            return View(parameter);
        }

        [HttpGet]
        public ActionResult Approvals()
        {
            if (Session["type"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Administrator")
            {
                return RedirectToAction("Logout", "Account");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Configuration([Bind(Include = "IdP,AnswerNumber,SilverNumber,GoldNumber,PlatinumNumber,UnlockNumber,PremiumNumber")] Parameter parameter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(parameter).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Configuration", "Admin");
        }

    }
}