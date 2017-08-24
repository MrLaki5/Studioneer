using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebRole1.Models;

namespace WebRole1.Controllers
{
    public class QuestionController : Controller
    {

        private Model1 db = new Model1();

        [HttpGet]
        public ActionResult Create()
        {
            var parameters = from m in db.Parameters select m;
            if (parameters.Any()) {
                Parameter par = parameters.First();
                ViewBag.k = par.AnswerNumber;
                return View();
            }
            return RedirectToAction("Logout", "Account");
        }

        [HttpPost]
        public ActionResult Create(HttpPostedFileBase file)
        {
            if (Session["type"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Professor")
            {
                return RedirectToAction("Logout", "Account");
            }

            if (file != null) {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/Images"), pic);
                while (!System.IO.File.Exists(path))
                {
                    pic = "1" + pic;
                    path = System.IO.Path.Combine(Server.MapPath("~/Images"), pic);
                }
                // file is uploaded
                file.SaveAs(path);

            }



            string name = Request.Form["naslov"];
            return View();
        }
    }

}