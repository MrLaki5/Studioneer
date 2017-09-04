using log4net;
using System.Web.Mvc;

namespace WebRole1.Controllers
{
    public class HomeController : Controller
    {

        private ILog log = LogManager.GetLogger("log");

        //Redirect user after login depending on hes session
        public ActionResult Login()
        {
            if (Session["type"] == null) {
                log.Error("Session missing");
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() == "Administrator")
            {
                return RedirectToAction("Configuration", "Admin");
            }
            if (Session["type"].ToString() == "Professor")
            {
                return RedirectToAction("Index", "Question");
            }
            return RedirectToAction("ActiveChannels", "Channel");
        }
    }
}