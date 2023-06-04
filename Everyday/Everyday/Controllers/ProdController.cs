using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Everyday.Models;

namespace Everyday.Controllers
{
    public class ProdController : Controller
    {
        public ActionResult Carrito(int id)
        {
            if (Session["carrito"] == null)
            {
                List<CarritoItem> compras = new List<CarritoItem>();
                compras.Add(new CarritoItem(db.Producto.Find(id), 1));
                Session["carrito"] = compras;
            }
            else
            {
                List<CarritoItem> compras = (List<CarritoItem>)Session["carrito"];
                int indexExistente = getIndex(id);
                if (indexExistente == -1)
                {
                    compras.Add(new CarritoItem(db.Producto.Find(id), 1));
                }
                else
                {
                    compras[indexExistente].cantidad++;
                    Session["carrito"] = compras;
                }
            }
            return View();
        }

        private int getIndex(int id)
        {
            List<CarritoItem> compras = (List<CarritoItem>)Session["carrito"];
            for (int i = 0; i < compras.Count; i++)
            {
                if (compras[i].Producto.idProd == id)
                {
                    return i;
                }
            }
            return -1;
        }

        public ActionResult DeleteCarrito(int id)
        {
            List<CarritoItem> compras = (List<CarritoItem>)Session["carrito"];
            compras.RemoveAt(getIndex(id));

            return View("Carrito");
        }

        public ActionResult FinalizarCompra()
        {
            List<CarritoItem> compras = (List<CarritoItem>)Session["carrito"];
            if(compras != null && compras.Count > 0)
            {
                DetalleVenta dv = new DetalleVenta();
                Venta venta = new Venta();

                venta.idClient = 2;
                //venta.total = compras.Sum(x => x.Producto.price * x.cantidad);
                venta.total = compras.Sum(x => x.Producto.price);
                venta.createdAt = DateTime.Now;

                

                //SqlConnection connection = new SqlConnection("data source=FRANK; initial catalog=EverydayBD; integrated security=True");
                //string query = "SELECT MAX(idVenta) FROM Venta";
                //connection.Open();
                //SqlCommand cmd = new SqlCommand(query, connection);
                //int idVenta = (int)cmd.ExecuteScalar();
                //connection.Close();

                venta.DetalleVenta = (from item in compras
                          select new DetalleVenta
                          {
                              iProd = item.Producto.idProd,
                              price = item.Producto.price,
                              quantity = item.cantidad,
                              subTotal = (item.cantidad * item.Producto.price),
                          }).ToList();

                //db.Venta.Add(venta);
                //db.DetallesVenta.Add(venta.DetallesVenta);

            }

            return View();
        }






        private EverydayDB db = new EverydayDB();

        // GET: Producto
        public ActionResult Index()
        {
            var producto = db.Producto.Include(p => p.Tipo).Include(p => p.Categoria).Include(p => p.Marca);
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
            return View();
        }

        // POST: Producto/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idProd,imagen,nameProd,description,price,stock,idType,idMarc,idCateg,createdAt")] Producto producto, HttpPostedFileBase File)
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

            ViewBag.idType = new SelectList(db.Tipo, "idType", "nameType", producto.idType);
            ViewBag.idCateg = new SelectList(db.Categoria, "idCateg", "nameCateg", producto.idCateg);
            ViewBag.idMarc = new SelectList(db.Marca, "idMarc", "nameMarc", producto.idMarc);
            return View(producto);
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
            return View(producto);
        }

        // MOSTRAR IMAGEN
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

        // POST: Producto/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idProd,imagen,nameProd,description,quantity,price,stock,idMarc,idCateg,createdAt")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(producto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idCateg = new SelectList(db.Categoria, "idCateg", "nameCateg", producto.idCateg);
            ViewBag.idMarc = new SelectList(db.Marca, "idMarc", "nameMarc", producto.idMarc);
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
