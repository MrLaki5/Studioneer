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
        public ActionResult Index(string Search="")
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
            if (parameters.Any())
            {
                Parameter par = parameters.First();
                ViewBag.unlock = par.UnlockNumber;
            }

            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any())
            {
                User user = users.First();
                int id = user.IdU;
                var questions = from m in db.Questions select m;
                questions = questions.Where(s => s.IsClone == 0);
                questions = questions.Where(s => s.IdU == id);
                if (string.Compare(Search, "") != 0)
                {
                    questions = questions.Where(s => s.Title.Contains(Search));
                }
                ViewBag.srch = Search;
                return View(questions);
            }
            return RedirectToAction("Logout", "Account");
        }

        [HttpGet]
        public ActionResult Editor(int? id)
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
            var questions = from m in db.Questions select m;
            questions = questions.Where(s => s.IdP == id);
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            var parameters = from m in db.Parameters select m;
            if (parameters.Any())
            {
                Parameter par = parameters.First();
                ViewBag.k = par.AnswerNumber;
            }
            
            if (users.Any() && questions.Any())
            {
                User user = users.First();
                Question question = questions.First();
                if (question.IsLocked == 1)
                    return RedirectToAction("Logout", "Account");
                if (question.IdU==user.IdU)
                    return View(question);
            }
            return RedirectToAction("Logout", "Account");
        }

        [HttpGet]
        public ActionResult Details(int? id)
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
            var questions = from m in db.Questions select m;
            questions = questions.Where(s => s.IdP == id);
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            var parameters = from m in db.Parameters select m;
            if (parameters.Any())
            {
                Parameter par = parameters.First();
                ViewBag.unlock = par.UnlockNumber;
            }

            if (users.Any() && questions.Any())
            {
                User user = users.First();
                Question question = questions.First();
                if (question.IsClone==1)
                    return RedirectToAction("Logout", "Account");
                if (question.IdU == user.IdU)
                    return View(question);
            }
            return RedirectToAction("Logout", "Account");
        }

        [HttpPost]
        public ActionResult IndexUp(int id) {
            if (Session["type"] == null)
            {
                return Content("error2", "text/plain");
            }
            if (Session["type"].ToString() != "Professor")
            {
                return Content("error2", "text/plain");
            }
            var questions = from m in db.Questions select m;
            questions = questions.Where(s => s.IdP == id);
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            var parameters = from m in db.Parameters select m;
            Parameter par = null;
            if (parameters.Any())
            {
                par = parameters.First();
            }
            else {
                return Content("error2", "text/plain");
            }

            if (users.Any() && questions.Any())
            {
                User user = users.First();
                Question question = questions.First();
                if (question.IsLocked == 0)
                    return Content("error2", "text/plain");
                if (question.IsClone == 1)
                    return Content("error2", "text/plain");
                if (question.IdU == user.IdU) {
                    if (user.Balans >= par.UnlockNumber) {
                        user.Balans -= par.UnlockNumber;
                        question.IsLocked = 0;
                        db.SaveChanges();   
                        Session["token"] = user.Balans.ToString();
                        return Content(user.Balans.ToString(), "text/plain");
                    }
                    return Content("error", "text/plain");
                }
            }
            return Content("error2", "text/plain");
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
        public ActionResult Editor(int? id, HttpPostedFileBase file)
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
            var questions = from m in db.Questions select m;
            questions = questions.Where(s => s.IdP == id);
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            User user = null;
            Question question1 = null;
            if (users.Any() && questions.Any())
            {
                user = users.First();
                question1 = questions.First();
                if (question1.IsLocked==1)
                    return RedirectToAction("Logout", "Account");
                if (question1.IdU != user.IdU)
                    return RedirectToAction("Logout", "Account");
            }
            else
            {
                return RedirectToAction("Logout", "Account");
            }




            if (Request.Form["ttitle"] == null)
            {
                return RedirectToAction("Editor", "Question");
            }
            string title = Request.Form["ttitle"];
            if (title.Length > 20)
            {
                ViewBag.err = "Title length max 20 characters";
                return View();
            }
            if (title == "")
            {
                ViewBag.err = "Title required";
                return View();
            }
            if (Request.Form["text"] == null)
            {
                return RedirectToAction("Editor", "Question");
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
                return RedirectToAction("Editor", "Question");
            }
            string correctAnswer = Request.Form["radio202"];


            int AnsNum = question1.Answers.Count;
            Answer[] ans = new Answer[AnsNum];
            for (int i = 1; i <= AnsNum; i++)
            {
                ans[i-1] = new Answer();
                if (Request.Form["answer " + i] == null)
                {
                    return RedirectToAction("Editor", "Question");
                }
                if (Request.Form["answer " + i] == "")
                {
                    ViewBag.err = "All answers are required";
                    return View();
                }
                string temp = Request.Form["answer " + i];
                ans[i-1].Number = i;
                ans[i-1].Text = temp;
                ans[i-1].Tag = (char)(i + 64) + "";
                if (correctAnswer == "" + i)
                {
                    ans[i-1].IsCorrect = 1;
                }
                else
                {
                    ans[i-1].IsCorrect = 0;
                }
            }

            int locked = 0;
            if (Request.Form["locked"] != null)
            {
                string lck = Request.Form["locked"];
                if (lck == "one")
                    locked = 1;
            }
            string path = "";
            string imgname = "";
            bool succ = false;
            if (file != null)
            {
                string exten = Path.GetExtension(file.FileName).ToLower();
                if ((exten != ".jpg") && (exten != ".jpeg") && (exten != ".png"))
                {       //bad type of file check
                    ViewBag.err = "Image is wrong format";
                    return View();
                }
                if (file.ContentLength > 30000)
                {                                          //size of file check in bytes
                    ViewBag.err = "Image is too big";
                    return View();
                }
                string pic = System.IO.Path.GetFileName(file.FileName);
                int count = 0;
                string pom = FormsAuthentication.HashPasswordForStoringInConfigFile(pic, "SHA1");
                path = System.IO.Path.Combine(Server.MapPath("~/Images"), pom + pic);
                imgname = pom + pic;
                while (System.IO.File.Exists(path))
                {
                    pom = count + "" + pic;
                    pom = FormsAuthentication.HashPasswordForStoringInConfigFile(pom, "SHA1");
                    path = System.IO.Path.Combine(Server.MapPath("~/Images"), pom + pic);
                    imgname = pom + pic;
                    count++;
                }
                pic = pom;
                file.SaveAs(path);                                        // file is uploaded
                succ = true;
                if (question1.Image != null)
                {
                    var filePath = Server.MapPath("~/Images/"+question1.ImageName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            }


            question1.Title = title;
            question1.Text = text;
            question1.IsLocked = locked;
            if (locked == 1)
                question1.LastLock = DateTime.UtcNow;
            if (succ)
            {
                question1.Image = path;
                question1.ImageName = imgname;
            }



            db.SaveChanges();
            foreach (var item in question1.Answers)
            {
                for (int cnt = 0; cnt < AnsNum; cnt++)
                {
                    if (ans[cnt].Number == item.Number) {
                        item.IsCorrect = ans[cnt].IsCorrect;
                        item.Text = ans[cnt].Text;
                        cnt = AnsNum;
                    }
                }
                
            }
            db.SaveChanges();

            return RedirectToAction("Index", "Question");
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
                return View();
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
                return View();
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
                    return View();
                }
                if (Request.Form["answer " + i] == "")
                {
                    ViewBag.err = "All answers are required";
                    return View();
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
            if (Request.Form["locked"] != null)
            {
                string lck = Request.Form["locked"];
                if (lck == "one")
                    locked = 1;
            }
            string path = "";
            string imgname = "";
            bool succ = false;
            if (file != null) {
                string exten = Path.GetExtension(file.FileName).ToLower();
                if ((exten != ".jpg") && (exten != ".jpeg") && (exten != ".png")){       //bad type of file check
                    ViewBag.err = "Image is wrong format";
                    return View();
                }
                if (file.ContentLength > 30000){                                          //size of file check in bytes
                    ViewBag.err = "Image is too big";
                    return View();
                }
                string pic = System.IO.Path.GetFileName(file.FileName);
                int count = 0;
                string pom = FormsAuthentication.HashPasswordForStoringInConfigFile(pic, "SHA1");
                path = System.IO.Path.Combine(Server.MapPath("~/Images"), pom+pic);
                imgname = pom + pic;
                while (System.IO.File.Exists(path))
                {
                    pom = count+"" + pic;
                    pom= FormsAuthentication.HashPasswordForStoringInConfigFile(pom, "SHA1");
                    path = System.IO.Path.Combine(Server.MapPath("~/Images"), pom+pic);
                    imgname = pom + pic;
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
                question.ImageName = imgname;
            }

            

            db.Questions.Add(question);
            db.SaveChanges();
            for (int i=0;i< ViewBag.k; i++){
                ans[i].IdP = question.IdP;
                db.Answers.Add(ans[i]);
            }
            db.SaveChanges();

            return RedirectToAction("Index", "Question");
        }
    }

}