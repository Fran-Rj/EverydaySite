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
    public class MarcaController : Controller
    {
        private EverydayDB db = new EverydayDB();

        [HttpPost]
        public ActionResult ChangeImage(Marca m)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase File = Request.Files[0];
                if (File.ContentLength > 0 && File.ContentType.Contains("image"))
                {
                    WebImage image = new WebImage(File.InputStream);
                    m.imagen = image.GetBytes();

                    Marca marca = db.Marca.Find(m.idMarc);
                    marca.imagen = m.imagen;
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
                    m.imagen = image;
                }                
            }

            return RedirectToAction("Index");
        }

        // GET: Marca
        public ActionResult Index()
        {
            return View(db.Marca.ToList());
        }

        public ActionResult Show(int id)
        {
            var producto = db.Producto.Where(p => p.idMarc == id);
            return View(producto.ToList());
        }

        public ActionResult MostrarImagen(int Id)
        {
            using (EverydayDB db = new EverydayDB())
            {
                var imagen = (from Marca in db.Marca
                              where Marca.idMarc == Id
                              select Marca.imagen).FirstOrDefault();
                return File(imagen, "png");
            }
        }

        // GET: Marca/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Marca marca = db.Marca.Find(id);
            if (marca == null)
            {
                return HttpNotFound();
            }
            return View(marca);
        }

        // GET: Marca/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Marca/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idMarc,nameMarc,createdAt")] Marca marca)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase File = Request.Files[0];
                if (File.ContentLength > 0 && File.ContentType.Contains("image"))
                {
                    WebImage image = new WebImage(File.InputStream);
                    marca.imagen = image.GetBytes();
                }
                else
                {
                    string fileName = Server.MapPath("~/Image/Ev.jpg");
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    byte[] image = br.ReadBytes((int)fs.Length);
                    br.Close();
                    fs.Close();

                    marca.imagen = image;
                }
            }
        
            if (marca != null)
            {
                marca.createdAt = DateTime.Now;
                db.Marca.Add(marca);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // GET: Marca/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Marca marca = db.Marca.Find(id);
            if (marca == null)
            {
                return HttpNotFound();
            }
            return View(marca);
        }

        // POST: Marca/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Marca marca)
        {
            if (marca != null)
            {
                using (EverydayDB db = new EverydayDB())
                {
                    Marca m = db.Marca.Find(marca.idMarc);
                    m.nameMarc = marca.nameMarc;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View(marca);
        }

        // GET: Marca/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Marca marca = db.Marca.Find(id);
            if (marca == null)
            {
                return HttpNotFound();
            }
            return View(marca);
        }

        // POST: Marca/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Marca marca = db.Marca.Find(id);
            db.Marca.Remove(marca);
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