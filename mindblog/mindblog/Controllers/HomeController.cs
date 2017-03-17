using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mindblog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Forge");
        }
       
        public ActionResult About()
        {
            ViewBag.Message = "Mindblog, the blog about extending your mind.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact me at lordfingle@practicalcybernetics.com";

            return View();
        }
    }
}