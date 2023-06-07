using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
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

        [HttpPost]
        public ActionResult ChangeImage(Producto producto)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase File = Request.Files[0];
                if (File.ContentLength > 0 && File.ContentType.Contains("image"))
                {
                    WebImage image = new WebImage(File.InputStream);
                    producto.imagen = image.GetBytes();

                    Producto p = db.Producto.Find(producto.idProd);
                    p.imagen = producto.imagen;
                    db.SaveChanges();
                }
                else
                {
                    string fileName = Server.MapPath("~/Image/Ev.jpg");
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    byte[] image = br.ReadBytes((int)fs.Length);
                    br.Close();
                    fs.Close();

                    producto.imagen = image;
                }                
            }

            return RedirectToAction("Index");
        }


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
        public ActionResult Create(Producto producto)
        {

            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase File = Request.Files[0];
                if (File.ContentLength > 0 && File.ContentType.Contains("image"))
                {
                    WebImage image = new WebImage(File.InputStream);
                    producto.imagen = image.GetBytes();
                }
                else
                {
                    string fileName = Server.MapPath("~/Image/Ev.jpg");
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    byte[] image = br.ReadBytes((int)fs.Length);
                    br.Close();
                    fs.Close();

                    producto.imagen = image;
                }
            }

            producto.state = "Activo";
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
        public ActionResult Edit(Producto producto)
        {
            if (producto != null)
            {
                using (EverydayDB db = new EverydayDB())
                {
                    Producto p = db.Producto.Find(producto.idProd);
                    p.nameProd = producto.nameProd;
                    p.description = producto.description;
                    p.price = producto.price;
                    p.stock = producto.stock;
                    p.state = producto.state;
                    db.SaveChanges();
                }

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
