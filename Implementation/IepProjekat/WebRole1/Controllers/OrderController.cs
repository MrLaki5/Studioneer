using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebRole1.Models;

namespace WebRole1.Controllers
{
    public class OrderController : Controller
    {

        private Model1 db = new Model1();

        [HttpGet]
        public ActionResult Index()
        {

            if (Session["type"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() == "Administrator")
            {
                return RedirectToAction("Logout", "Account");
            }
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            var parameters = from m in db.Parameters select m;
            if (parameters.Any())
            {
                Parameter par = parameters.First();
                ViewBag.silver = par.SilverNumber;
                ViewBag.gold = par.GoldNumber;
                ViewBag.platinum = par.PlatinumNumber;
                if (users.Any())
                {
                    return View();
                }
            }         
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public ActionResult Index(int package) {

            if (Session["type"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() == "Administrator")
            {
                return RedirectToAction("Logout", "Account");
            }
            string email = Session["email"].ToString();
            var users = from m in db.Users select m;
            users = users.Where(s => s.Mail.Equals(email));
            var parameters = from m in db.Parameters select m;
            if (parameters.Any())
            {
                Parameter par = parameters.First();
                ViewBag.silver = par.SilverNumber;
                ViewBag.gold = par.GoldNumber;
                ViewBag.platinum = par.PlatinumNumber;
                if (users.Any())
                {
                    User user = users.First();
                    string link = "";
                    string api = "aee8499dc5f97cd80d74c64ce7f3545d";
                    int price = 0;
                    int number = 0;
                    switch (package)
                    {
                        case 1:
                            number = ViewBag.silver;
                            price = 50 * ViewBag.silver;
                            break;
                        case 2:
                            number = ViewBag.gold;
                            price = 50 * ViewBag.gold;
                            break;
                        case 3:
                            number = ViewBag.platinum;
                            price = 50 * ViewBag.platinum;
                            break;
                    }
                    if (price == 0) {
                        return RedirectToAction("Logout", "Account");
                    }
                    Order order = new Order();
                    order.IdU = user.IdU;
                    order.Number = number;
                    order.Price = price;
                    order.State = "waiting on";
                    db.Orders.Add(order);
                    db.SaveChanges();
                    string userid = FormsAuthentication.HashPasswordForStoringInConfigFile(user.Mail+user.Password+order.IdO, "SHA1");
                    string returnUrl = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + Url.Action("PayRply", "Order");
                    link = "http://api.centili.com/payment/widget?apikey=" + api + "&price=" + price.ToString() + "&returnurl=" + returnUrl+ "&userid="+userid;
                    return RedirectToAction(link);
                }
            }
            return RedirectToAction("Login", "Account");
        }
    }
}