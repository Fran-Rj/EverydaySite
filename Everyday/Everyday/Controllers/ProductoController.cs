using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Everyday.Models;

namespace Everyday.Controllers
{
    public class ProductoController : Controller
    {
        private EverydayDB db = new EverydayDB();

        // GET: Producto
        public ActionResult Index()
        {
            var producto = db.Producto.Include(p => p.Categoria).Include(p => p.Marca).Include(p => p.Tipo);
            return View(producto.ToList());
        }

        // GET: Producto/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // GET: Producto/Create
        public ActionResult Create()
        {
            ViewBag.idCateg = new SelectList(db.Categoria, "idCateg", "nameCateg");
            ViewBag.idMarc = new SelectList(db.Marca, "idMarc", "nameMarc");
            ViewBag.idType = new SelectList(db.Tipo, "idType", "nameType");
            return View();
        }

        // POST: Producto/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idProd,imagen,nameProd,description,price,stock,state,idType,idMarc,idCateg,createdAt")] Producto producto, HttpPostedFileBase File)
        {
            if (File.ContentLength > 0)
            {
                WebImage image = new WebImage(File.InputStream);
                producto.imagen = image.GetBytes();
            }

            producto.createdAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Producto.Add(producto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idCateg = new SelectList(db.Categoria, "idCateg", "nameCateg", producto.idCateg);
            ViewBag.idMarc = new SelectList(db.Marca, "idMarc", "nameMarc", producto.idMarc);
            ViewBag.idType = new SelectList(db.Tipo, "idType", "nameType", producto.idType);
            return View(producto);
        }

        [HttpGet]
        public ActionResult MostrarImagen(int Id)
        {
            using (EverydayDB db = new EverydayDB())
            {
                var imagen = (from Producto in db.Producto
                              where Producto.idProd == Id
                              select Producto.imagen).FirstOrDefault();
                return File(imagen, "png");
            }
        }

        // GET: Producto/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            ViewBag.idCateg = new SelectList(db.Categoria, "idCateg", "nameCateg", producto.idCateg);
            ViewBag.idMarc = new SelectList(db.Marca, "idMarc", "nameMarc", producto.idMarc);
            ViewBag.idType = new SelectList(db.Tipo, "idType", "nameType", producto.idType);
            return View(producto);
        }

        // POST: Producto/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idProd,imagen,nameProd,description,price,stock,state,idType,idMarc,idCateg,createdAt")] Producto producto, HttpPostedFileBase File)
        {
            if (File.ContentLength > 0)
            {
                WebImage image = new WebImage(File.InputStream);
                producto.imagen = image.GetBytes();
            }

            producto.createdAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Entry(producto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idCateg = new SelectList(db.Categoria, "idCateg", "nameCateg", producto.idCateg);
            ViewBag.idMarc = new SelectList(db.Marca, "idMarc", "nameMarc", producto.idMarc);
            ViewBag.idType = new SelectList(db.Tipo, "idType", "nameType", producto.idType);
            return View(producto);
        }

        // GET: Producto/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Producto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Producto producto = db.Producto.Find(id);
            db.Producto.Remove(producto);
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
