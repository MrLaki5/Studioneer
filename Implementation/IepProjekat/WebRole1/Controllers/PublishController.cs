using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebRole1.Models;

namespace WebRole1.Controllers
{
    public class PublishController : Controller
    {
        private Model1 db = new Model1();

        [HttpGet]
        public ActionResult Index(int id)
        {
            if (Session["type"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() != "Professor")
            {
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
                questions = questions.Where(s => s.IdU == idU);
                questions = questions.Where(s => s.IsClone == 0);
                questions = questions.Where(s => s.IsLocked == 1);
                return View(questions);
            }
            return RedirectToAction("Logout", "Account");
        }
    }
}