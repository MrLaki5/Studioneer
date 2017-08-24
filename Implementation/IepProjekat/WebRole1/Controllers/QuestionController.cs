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
        public ActionResult Index()
        {
            if (Session["type"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Professor")
            {
                return RedirectToAction("Logout", "Account");
            }

            return View(db.Questions.ToList());
        }

        public ActionResult Edit()
        {
            var users = from m in db.Questions select m;
            users = users.Where(s => s.IdP==5);
            if (users.Any())
            {
                Question question = users.First();
                return View(question);
            }
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["type"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Professor")
            {
                return RedirectToAction("Logout", "Account");
            }
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

            var parameters = from m in db.Parameters select m;
            if (parameters.Any()){
                Parameter par = parameters.First();
                ViewBag.k = par.AnswerNumber;
            }

            if (Request.Form["ttitle"] == null){
                return RedirectToAction("Create", "Question");
            }
            string title = Request.Form["ttitle"];
            if (title.Length > 20){
                ViewBag.err = "Title length max 20 characters";
                return View();
            }
            if (title == ""){
                ViewBag.err = "Title required";
                return View();
            }
            if (Request.Form["text"] == null){
                return RedirectToAction("Create", "Question");
            }
            string text = Request.Form["text"];
            if (text.Length > 200)
            {
                ViewBag.err = "Text length max 200 characters";
                return View();
            }
            if (text == "")
            {
                ViewBag.err = "Text required";
                return View();
            }


            if (Request.Form["radio202"] == null)
            {
                return RedirectToAction("Create", "Question");
            }
            string correctAnswer = Request.Form["radio202"];

            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            int idU;
            if (users.Any())
            {
                User user = users.First();
                idU = user.IdU;
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }


            Answer[] ans = new Answer[ViewBag.k];

            for (int i = 0; i < ViewBag.k; i++)
            {
                ans[i] = new Answer();
                if (Request.Form["answer " + i] == null)
                {
                    return View("Create", "Question");
                }
                if (Request.Form["answer " + i] == "")
                {
                    ViewBag.err = "All answers are required";
                    return View("Create", "Question");
                }
                string temp = Request.Form["answer " + i];
                ans[i].Number = i + 1;
                ans[i].Text = temp;
                ans[i].Tag = (char)(i + 65) + "";
                if (correctAnswer == "" + i)
                {
                    ans[i].IsCorrect = 1;
                }
                else
                {
                    ans[i].IsCorrect = 0;
                }
            }

            int locked = 0;
            if (Request.Form["locked"] == null)
            {
                return RedirectToAction("Create", "Question");
            }
            string lck= Request.Form["locked"];
            if (lck == "true")
                locked = 1;

            string path = "";
            bool succ = false;
            if (file != null) {
                string exten = Path.GetExtension(file.FileName).ToLower();
                if ((exten != ".jpg") && (exten != ".jpeg") && (exten != ".png")){       //bad type of file check
                    ViewBag.err = "Image is wrong format";
                    return View();
                }
                if (file.ContentLength > 30000){                                          //size of file check in bytes
                    ViewBag.err = "Image is too big";
                    return View("Create", "Question");
                }
                string pic = System.IO.Path.GetFileName(file.FileName);
                int count = 0;
                string pom = FormsAuthentication.HashPasswordForStoringInConfigFile(pic, "SHA1");
                path = System.IO.Path.Combine(Server.MapPath("~/Images"), pom+pic);
                while (System.IO.File.Exists(path))
                {
                    pom = count+"" + pic;
                    pom= FormsAuthentication.HashPasswordForStoringInConfigFile(pom, "SHA1");
                    path = System.IO.Path.Combine(Server.MapPath("~/Images"), pom+pic);
                    count++;
                }
                pic = pom;
                file.SaveAs(path);                                        // file is uploaded
                succ = true;
            }
            

            Question question = new Question();
            question.Title = title;
            question.Text = text;
            question.IdU = idU;
            question.IsClone = 0;
            question.IsLocked = locked;
            question.CreationTime = DateTime.UtcNow;
            if (locked == 1)
                question.LastLock = question.CreationTime;
            if (succ){
                question.Image = path;
            }

            

            db.Questions.Add(question);
            db.SaveChanges();
            for (int i=0;i< ViewBag.k; i++){
                ans[i].IdP = question.IdP;
                db.Answers.Add(ans[i]);
            }
            db.SaveChanges();
        
            return View();
        }
    }

}