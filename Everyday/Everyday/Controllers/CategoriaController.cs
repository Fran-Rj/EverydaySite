using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Everyday.Models;

namespace Everyday.Controllers
{
    public class CategoriaController : Controller
    {
        private EverydayDB db = new EverydayDB();

        public ActionResult See(int id)
        {
            var producto = db.Producto.Where(p => p.idCateg == id);

            string cmd = string.Format("select nameCateg from Categoria where idCateg = '{0}'", id);
            DataSet ds = Utilities.Ejecutar(cmd);

            ViewBag.Categoria = ds.Tables[0].Rows[0][0].ToString();
            return View(producto.ToList());
        }

        public ActionResult Show()
        {
            var producto = db.Producto.OrderBy(p => p.idCateg);

            return View(producto.ToList());
        }

        public ActionResult Index()
        {
            var datos = db.Categoria.ToList();
            return View(datos);
        }


        public ActionResult Description(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Login", "Account");
            }
            Producto categoria = db.Producto.Find(id);
            if (categoria == null)
            {
                return HttpNotFound();
            }
            return View(categoria);
        }

        // GET: Categoria/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categoria/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCateg,nameCateg,createdAt")] Categoria categoria)
        {
            categoria.createdAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Categoria.Add(categoria);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        public ActionResult Delete(int id)
        {
            Categoria categoria = db.Categoria.Find(id);
            db.Categoria.Remove(categoria);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
