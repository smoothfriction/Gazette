// // Copyright 2012, Smoothfriction
// // Author: Erik van Brakel
// // Licensed under the BSD 3-Clause License, see license.txt for details, or go to // http://www.opensource.org/licenses/BSD-3-Clause
using System.Web.Mvc;

namespace Gazette.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your quintessential app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your quintessential contact page.";

            return View();
        }
    }
}
