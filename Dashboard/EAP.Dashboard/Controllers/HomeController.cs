using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EAP.Dashboard.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()

        {


            //如果未登录，则跳转到登录页面
            if (Session["user_account"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.site = site;
            if (Session["lang"] == null || Session["lang"].ToString() == "cn")
            {
                return View();
            }
            else
            {
                return View("Home/Index.en.cshtml");
            }
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}