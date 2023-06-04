using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Tree;
using Everyday.Models;

namespace Everyday.Controllers
{
    public class CarritoController : Controller
    {
        private EverydayDB db = new EverydayDB();

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
                if(indexExistente == -1)
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

        public ActionResult Deletes(int id)
        {
            List<CarritoItem> compras = (List<CarritoItem>)Session["carrito"];
            compras.RemoveAt(getIndex(id));

            return View("Carrito");
        }





        // CARRITO
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Carrito c)
        {
            return View();
        }



        // GET: Carrito/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carrito carrito = db.Carrito.Find(id);
            if (carrito == null)
            {
                return HttpNotFound();
            }
            return View(carrito);
        }

        // GET: Carrito/Create
        public ActionResult Create(int? id)
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
                compras.Add(new CarritoItem(db.Producto.Find(id), 1));
                Session["carrito"] = compras;
            }
            return View();                    
        }

        // POST: Carrito/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCarrito,idUser,idProd")] Carrito carrito, int id)
        {
            carrito.idUser = 2;
            carrito.idProd = id;

            //int indexExist = Index(id);
            //if (indexExist == -1)
            //{
                if (ModelState.IsValid)
                {
                    db.Carrito.Add(carrito);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                //compras.Add(new CarritoItem(db.Producto.Find(id), 1));
            //}
            //else
            //{
                //ViewBag.Mensaje = "El producto ya se añadió al carrito";
                //return RedirectToAction("Index");
            //}
  

            ViewBag.idProd = new SelectList(db.Producto, "idProd", "nameProd", carrito.idProd);
            ViewBag.idUser = new SelectList(db.Usuario, "idUser", "photo", carrito.idUser);
            return View(carrito);
        }

        // OBTENER ID IN BD
        private int IndexDB(int id)
        {
            var exist = db.Carrito.ToList();

            for (int i = 0; i < exist.Count; i++)
            {
                if (exist[i].Producto.idProd == id)
                {
                    return i;
                }
            }
            return -1;
        }





        // GET: Carrito/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carrito carrito = db.Carrito.Find(id);
            if (carrito == null)
            {
                return HttpNotFound();
            }
            ViewBag.idProd = new SelectList(db.Producto, "idProd", "nameProd", carrito.idProd);
            ViewBag.idUser = new SelectList(db.Usuario, "idUser", "photo", carrito.idUser);
            return View(carrito);
        }

        // POST: Carrito/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCarrito,idUser,idProd")] Carrito carrito)
        {
            if (ModelState.IsValid)
            {
                db.Entry(carrito).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idProd = new SelectList(db.Producto, "idProd", "nameProd", carrito.idProd);
            ViewBag.idUser = new SelectList(db.Usuario, "idUser", "photo", carrito.idUser);
            return View(carrito);
        }

        // GET: Carrito/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Carrito carrito = db.Carrito.Find(id);
        //    if (carrito == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(carrito);
        //}

        // POST: Carrito/Delete/5

        public ActionResult Delete(int? id)
        {
            Carrito carrito = db.Carrito.Find(id);
            db.Carrito.Remove(carrito);
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
