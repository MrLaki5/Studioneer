using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
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
            /*if (Session["type"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Professor")
            {
                return RedirectToAction("Logout", "Account");
            }*/

            if (file != null) {
                string exten = Path.GetExtension(file.FileName).ToLower();
                if ((exten != ".jpg") && (exten != ".jpeg") && (exten != ".png"))       //bad type of file check
                    return RedirectToAction("Create", "Question");
                if (file.ContentLength > 30000)                                          //size of file check in bytes
                    return RedirectToAction("Create", "Question");
                string pic = System.IO.Path.GetFileName(file.FileName);
                int count = 0;
                string pom = FormsAuthentication.HashPasswordForStoringInConfigFile(pic, "SHA1");
                string path = System.IO.Path.Combine(Server.MapPath("~/Images"), pom+pic);
                while (System.IO.File.Exists(path))
                {
                    pom = count+"" + pic;
                    pom= FormsAuthentication.HashPasswordForStoringInConfigFile(pom, "SHA1");
                    path = System.IO.Path.Combine(Server.MapPath("~/Images"), pom+pic);
                    count++;
                }
                pic = pom;
                // file is uploaded
                file.SaveAs(path);
            }



            string name = Request.Form["naslov"];
            return View();
        }
    }

}