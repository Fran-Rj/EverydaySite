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
            if (Session["user"] != null)
            {
                var producto = db.Producto.Where(p => p.color == data || p.Tipo.nameType == data ||
                p.Marca.nameMarc == data || p.Categoria.nameCateg == data);

                return View(producto.ToList());
            }

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