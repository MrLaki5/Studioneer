using System.Web.Mvc;

namespace WebRole1.Controllers
{
    public class HomeController : Controller
    {

        //Redirect user after login depending on hes session
        public ActionResult Login()
        {
            if (Session["type"] == null) {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() == "Administrator")
            {
                return RedirectToAction("Configuration", "Admin");
            }
            return RedirectToAction("Info", "Account");
        }
    }
}