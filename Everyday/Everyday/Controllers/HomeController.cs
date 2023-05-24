using Everyday.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Everyday.Controllers
{
    public class HomeController : Controller
    {
        private EverydayDB db = new EverydayDB();

        public ActionResult Home()
        {
            ViewBag.vistaActual = "Home";
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            ViewBag.vistaActual = "About";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.vistaActual = "Contact";
            return View();
        }
    }
}