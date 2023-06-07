using Everyday.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Everyday.Controllers
{
    public class HomeController : Controller
    {
        private EverydayDB db = new EverydayDB();

        public ActionResult Show(string data)
        {
            int id = 0;
            string cmd = "";
            DataSet ds;

            if (data == "Adidas" || data == "adidas")
            {
                id = 4;
            }

            if (data == "Nike" || data == "nike")
            {
                id = 2;
            }

            if (data == "Gucci" || data == "gucci")
            {
                id = 1;
            }

            if (Session["user"] != null)
            {
                var producto = db.Producto.Where(p => p.color == data || p.idMarc == id);

                return View(producto.ToList());
            }

            ViewBag.Data = data;
            return View();
        }

        public ActionResult Home()
        {
            if (Session["user"] != null)
            {
                return View(db.Marca.ToList());
            }

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