using log4net;
using System;
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
        private ILog log = LogManager.GetLogger("log");

        //Professor:----------------------------------------------------------------------

        //displays all questions of one professor that are templates, Search string for question.title
        [HttpGet]
        public ActionResult Index(string Search="")
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
            log.Error("user not existing in db");
            return RedirectToAction("Logout", "Account");
        }

        //displays edit for specific question that is not locked nad not a clone
        [HttpGet]
        public ActionResult Editor(int? id)
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
                log.Error("id parameter missing");
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
            log.Error("question and user not mergable in database");
            return RedirectToAction("Logout", "Account");
        }

        //displays details of specifick question
        [HttpGet]
        public ActionResult Details(int? id)
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
                log.Error("missing id parameter");
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
                if (question.IsClone == 1)
                {
                    ViewBag.clone = 1;
                }
                else
                {
                    ViewBag.clone = 0;
                }
                if (question.IdU == user.IdU)
                    return View(question);
            }
            log.Error("question and user not mergeable in db");
            return RedirectToAction("Logout", "Account");
        }

        //index:ajax locks and unlocks questions 
        [HttpPost]
        public ActionResult IndexUp(int id) {
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
            else
            {
                log.Error("parameters missing in db, initialize again db");
                return Content("error2", "text/plain");
            }
            if (users.Any() && questions.Any())
            {
                User user = users.First();
                Question question = questions.First();
                if (question.IsLocked == 0)
                {
                    log.Error("question is locked id: "+question.IdP);
                    return Content("error2", "text/plain");
                }
                if (question.IsClone == 1)
                {
                    log.Error("question is clone id: "+question.IdP);
                    return Content("error2", "text/plain");
                }
                if (question.IdU == user.IdU)
                {
                    if (user.Balans >= par.UnlockNumber)
                    {
                        user.Balans -= par.UnlockNumber;
                        question.IsLocked = 0;
                        db.SaveChanges();   
                        Session["token"] = user.Balans.ToString();
                        return Content(user.Balans.ToString(), "text/plain");
                    }
                    return Content("error", "text/plain");
                }
            }
            log.Error("question and user not mergeable");
            return Content("error2", "text/plain");
        }

        //displays create question page
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
            var parameters = from m in db.Parameters select m;
            if (parameters.Any()) {
                Parameter par = parameters.First();
                ViewBag.k = par.AnswerNumber;
                return View();
            }
            log.Error("parameters missing initialize again db");
            return RedirectToAction("Logout", "Account");
        }

        //editor for question, submiting
        [HttpPost]
        public ActionResult Editor(int? id, HttpPostedFileBase file)
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
                log.Error("id argument missing");
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
                if (question1.IsLocked == 1)
                {
                    log.Error("question is locked");
                    return RedirectToAction("Logout", "Account");
                }
                if (question1.IdU != user.IdU)
                {
                    log.Error("question and user not mergeable");
                    return RedirectToAction("Logout", "Account");
                }
            }
            else
            {
                log.Error("user and question not mergeable");
                return RedirectToAction("Logout", "Account");
            }
            if (Request.Form["ttitle"] == null)
            {
                log.Error("html element not loaded properly");
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
                log.Error("html element not loaded properly");
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
                log.Error("html element not loaded properly");
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
                    log.Error("html element not loaded properly");
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
                {
                    locked = 1;
                }
            }
            //string path = "";
            //string imgname = "";
            //bool succ = false;
            if (file != null)
            {
                string exten = Path.GetExtension(file.FileName).ToLower();
                if ((exten != ".jpg") && (exten != ".jpeg") && (exten != ".png"))
                {       //bad type of file check
                    ViewBag.err = "Image is wrong format";
                    return View();
                }
                if (file.ContentLength > 1000000)
                {                                          //size of file check in bytes
                    ViewBag.err = "Image is too big";
                    return View();
                }
                /*
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
                /*if (question1.Image != null)                              //deleting of old image, cos of copies should not be done
                {
                    var filePath = Server.MapPath("~/Images/"+question1.ImageName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }*/
                question1.Image = new byte[file.ContentLength];
                file.InputStream.Read(question1.Image, 0, question1.Image.Length);
            }
            question1.Title = title;
            question1.Text = text;
            question1.IsLocked = locked;
            if (locked == 1)
                question1.LastLock = DateTime.UtcNow;         
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

        //question create, submiting
        [HttpPost]
        public ActionResult Create(HttpPostedFileBase file)
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
            var parameters = from m in db.Parameters select m;
            if (parameters.Any()){
                Parameter par = parameters.First();
                ViewBag.k = par.AnswerNumber;
            }
            if (Request.Form["ttitle"] == null){
                log.Error("html element not loaded properly");
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
                log.Error("html element not loaded properly");
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
                log.Error("html element not loaded properly");
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
                log.Error("user not found in db");
                return RedirectToAction("Logout", "Home");
            }
            Answer[] ans = new Answer[ViewBag.k];
            for (int i = 0; i < ViewBag.k; i++)
            {
                ans[i] = new Answer();
                if (Request.Form["answer " + i] == null)
                {
                    log.Error("html element not loaded properly");
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
                {
                    locked = 1;
                }
            }
            //string path = "";
            //string imgname = "";
            Question question = new Question();
            //bool succ = false;
            if (file != null) {
                
                string exten = Path.GetExtension(file.FileName).ToLower();
                if ((exten != ".jpg") && (exten != ".jpeg") && (exten != ".png")){       //bad type of file check
                    ViewBag.err = "Image is wrong format";
                    return View();
                }
                if (file.ContentLength > 1000000){                                          //size of file check in bytes
                    ViewBag.err = "Image is too big";
                    return View();
                }
                /*
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
                succ = true;*/
                question.Image = new byte[file.ContentLength];
                file.InputStream.Read(question.Image, 0, question.Image.Length);
                //succ = true;
            }
            
            question.Title = title;
            question.Text = text;
            question.IdU = idU;
            question.IsClone = 0;
            question.IsLocked = locked;
            question.CreationTime = DateTime.UtcNow;
            if (locked == 1)
                question.LastLock = question.CreationTime;
            db.Questions.Add(question);
            db.SaveChanges();
            for (int i=0;i< ViewBag.k; i++){
                ans[i].IdP = question.IdP;
                db.Answers.Add(ans[i]);
            }
            db.SaveChanges();
            return RedirectToAction("Index", "Question");
        }

        //Student:----------------------------------------------------------------------


        //display answer page for specific question in specific channel for specific user
        [HttpGet]
        public ActionResult Answering(int? id, int? chan)
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
            if (id == null)
            {
                log.Error("id argument missing");
                return RedirectToAction("Logout", "Account");
            }
            if (chan == null)
            {
                log.Error("chan argument missing");
                return RedirectToAction("Logout", "Account");
            }
            var subscription = from m in db.Subscriptions select m;
            var users = from m in db.Users select m;
            string email = Session["email"].ToString();
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any())
            {
                User user = users.First();
                subscription = subscription.Where(s => s.IdC == chan);
                subscription = subscription.Where(s => s.IdU == user.IdU);
                subscription = subscription.Where(s => s.Channel.OpenTime != null);
                subscription = subscription.Where(s => ((s.Channel.CloseTime == null) || (s.Channel.CloseTime > DateTime.UtcNow)));

                if (subscription.Any())
                {
                    var publisheds = from m in db.Publisheds select m;
                    publisheds = publisheds.Where(s => subscription.Any(s2 => s2.IdC == s.IdC));
                    publisheds = publisheds.Where(s => s.IdP == id);

                    var responses = from m in db.Responses select m;
                    responses = responses.Where(s => s.IdU == user.IdU);
                    responses = responses.Where(s => s.IdC == chan);
                    responses = responses.Where(s => s.IdP == id);

                    publisheds = publisheds.Where(s => !responses.Any(s2 => s2.IdC == s.IdC && s2.IdP == s.IdP));

                    if (publisheds.Any())
                    {
                        ViewBag.premium = subscription.First().IsPremium;
                        return View(publisheds.First());
                    }
                }
            }
            log.Error("data provided is incorect");
            return RedirectToAction("Logout", "Account");
        }

        //checks:ajax for correction of parametes and saves answer in db
        [HttpPost]
        public ActionResult Answering(int published, int answer)
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
            var parameters = from m in db.Parameters select m;
            int tokens = 0;
            if (parameters.Any())
            {
                Parameter par = parameters.First();
                if (par.PremiumNumber != null)
                {
                    tokens = (int)par.PremiumNumber;
                }
            }
            else
            {
                log.Error("parameters missing in db");
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
             
                var publisheds = from m in db.Publisheds select m;
                publisheds = publisheds.Where(s => s.IdPub == published);
                publisheds = publisheds.Where(s => subscription.Any(s2 => s2.IdC == s.IdC));

                if (publisheds.Any())
                {
                    Published pubb = publisheds.First();
                    Question quest = pubb.Question;

                    var responses = from m in db.Responses select m;
                    responses = responses.Where(s => s.IdU == user.IdU && s.IdC==pubb.IdC && s.IdP==pubb.IdP);

                    if (responses.Any())
                    {
                        log.Error("answer already answerd");
                        return Content("error2", "text/plain");
                    }

                    var answers = from m in db.Answers select m;
                    answers = answers.Where(s => s.IdP == quest.IdP);
                    answers = answers.Where(s => s.IsCorrect == 1);
                    var answers1 = from m in db.Answers select m;
                    answers1 = answers1.Where( s=> s.IdA==answer);
                    if (answers1.Any() && answers.Any()) {
                        subscription = subscription.Where(s => s.IdC == pubb.IdC);
                        if (subscription.Any()) {
                            Subscription subb = subscription.First();
                            Response respo = new Response();
                            respo.IdU = user.IdU;
                            respo.IdP = quest.IdP;
                            respo.IdA = answers1.First().IdA;
                            respo.IdC = subb.IdC;
                            respo.SendTime = DateTime.UtcNow;
                            db.Responses.Add(respo);
                            db.SaveChanges();
                            if (subb.IsPremium == 1)
                            {
                                if (user.Balans < tokens)
                                {
                                    subb.IsPremium = 0;
                                    db.SaveChanges();
                                    return Content("-1<>0", "text/plain");
                                }
                                else {
                                    user.Balans -= tokens;
                                    Session["token"] = user.Balans;
                                    string tempStr = answers.First().IdA.ToString();
                                    tempStr += "<>";
                                    tempStr += user.Balans.ToString();
                                    db.SaveChanges();
                                    return Content(tempStr, "text/plain");
                                }
                            }
                            else
                            {
                                return Content("-1<>0", "text/plain");
                            }
                        }
                    }
                }
            }
            log.Error("arguments not corect according to db");
            return Content("error2", "text/plain");
        }

        //displays all answers of specific student
        [HttpGet]
        public ActionResult Answers()
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
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any())
            {
                User user = users.First();
                var responses = from m in db.Responses select m;
                responses = responses.Where(s => s.IdU == user.IdU);
                return View(responses);
            }
            log.Error("user not in db");
            return RedirectToAction("Logout", "Account");
        }

        //displays details for specific answer for specific student
        [HttpGet]
        public ActionResult AnswerDetails(int? idC, int? idP)
        {
            if (Session["type"] == null)
            {
                log.Error("wrong user type");
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Student")
            {
                log.Error("Session missing");
                return RedirectToAction("Logout", "Account");
            }
            if (idC == null)
            {
                log.Error("idC argument missing");
                return RedirectToAction("Logout", "Account");
            }
            if (idP == null)
            {
                log.Error("idP argument missing");
                return RedirectToAction("Logout", "Account");
            }
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            if (users.Any())
            {
                User user = users.First();
                var responses = from m in db.Responses select m;
                responses = responses.Where(s => s.IdU == user.IdU);
                responses = responses.Where(s => s.IdC == idC && s.IdP == idP);
                if (responses.Any())
                    return View(responses.First());
            }
            log.Error("user not in db");
            return RedirectToAction("Logout", "Account");
        }
    }
}