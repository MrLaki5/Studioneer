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
                    string userid = FormsAuthentication.HashPasswordForStoringInConfigFile(user.Mail + user.Password + order.IdO, "SHA1");
                    order.Tag = userid;
                    db.SaveChanges();
                    //string returnUrl = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + Url.Action("PayRply", "Order");
                    link = "http://api.centili.com/payment/widget?apikey=" + api + "&price=50&userid="+userid;
                    return Redirect(link);
                }
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult PayReply(string userid, string status) {
            var orders = from m in db.Orders select m;
            orders = orders.Where(s => s.Tag.Equals(userid));
            Order order = null;
            if (!orders.Any())
            {
                return RedirectToAction("Logout", "Account");
            }
            else
            {
                order = orders.First();
            }
            if (string.Compare(order.State, "waiting on") != 0)
            {
                return RedirectToAction("Logout", "Account");
            }
            if (string.Compare(status, "success") == 0)
            {
                order.User.Balans += order.Number;
                order.State = "realized";
                if (Session["token"] != null)
                {
                    Session["token"] = order.User.Balans.ToString();
                }
                ViewBag.message = "Transaction successfull";
                ViewBag.button = "Continue";
            }
            else
            {
                order.State = "rejected";
                ViewBag.message = "Something went wrong";
                ViewBag.button = "Try again";
            }
            db.SaveChanges();
            if (Session["type"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["type"].ToString() == "Administrator")
            {
                return RedirectToAction("Logout", "Account");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Orders() {
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
            if (users.Any())
            {
                User user = users.First();
                var orders = from m in db.Orders select m;
                orders = orders.Where(s => s.IdU == user.IdU);
                return View(orders);
            }   
            return RedirectToAction("Login", "Account");
        }
    }
}