using log4net;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebRole1.Models;

namespace WebRole1.Controllers
{
    public class AdminController : Controller
    {
        private Model1 db = new Model1();
        private ILog log = LogManager.GetLogger("log");

        //Configuration page display, seting parameters
        [HttpGet]
        public ActionResult Configuration()
        {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Administrator")
            {
                log.Error("wrong user type");
                return RedirectToAction("Logout", "Account");
            }
            var users = from m in db.Parameters select m;
            Parameter parameter = users.First();
            return View(parameter);
        }

        //Approvals display page
        [HttpGet]
        public ActionResult Approvals(int Search = 0)
        {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Administrator")
            {
                log.Error("wrong user type");
                return RedirectToAction("Logout", "Account");
            }
            var users = from m in db.Users select m;
            //in case search==1 display only not activated (status==0) users
            if (Search == 0)
            {
                ViewBag.chBox = "0";
                return View(users);
            }
            else
            {
                users = users.Where(s => s.Status == 0);
                ViewBag.chBox = "1";
                return View(users);
            }
        }

        //Configuration save changes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Configuration([Bind(Include = "IdP,AnswerNumber,SilverNumber,GoldNumber,PlatinumNumber,UnlockNumber,PremiumNumber")] Parameter parameter)
        {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Administrator")
            {
                log.Error("wrong user type");
                return RedirectToAction("Logout", "Account");
            }
            if (ModelState.IsValid)
            {
                db.Entry(parameter).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.message = "Saved";
            }
            else
            {
                ViewBag.message = "Not saved";
            }   
            return View();
        }

        //Approval:ajax submiting user registration
        [HttpPost]
        public ActionResult Approvals(string email, string type)
        {
            if (Session["type"] == null)
            {
                log.Error("Session missing");
                return Content("error", "text/plain");
            }
            if (Session["type"].ToString() != "Administrator")
            {
                log.Error("wrong user type");
                return Content("error", "text/plain");
            }
            if (type != "Student" && type != "Professor")
            {
                return Content("error", "text/plain");
            }
            var users = from m in db.Users select m;
            //find user by email and check if registration is not confirmed
            users = users.Where(s => s.Mail.Equals(email));
            users = users.Where(s => s.Status == 0);
            if (users.Any())
            {
                User user = users.First();
                user.Type = type;
                user.Status = 1;
                db.SaveChanges();
                return Content("Success", "text/plain");
            }
            log.Error("User not found");
            return Content("error", "text/plain");
        }
    }
}