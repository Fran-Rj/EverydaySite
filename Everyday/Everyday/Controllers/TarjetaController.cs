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
    public class TarjetaController : Controller
    {
        private EverydayDB db = new EverydayDB();


        [HttpGet]
        public ActionResult Pagos()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Pagos(Tarjeta t)
        {
            Pago p = new Pago();
            Venta v = new Venta();
            DetalleVenta d = new DetalleVenta();

            if (Session["user"] != null)
            {
                string cmd = "";
                DataSet ds; ;

                // VALIDAR DATOS TARJETA
                cmd = string.Format("select count(*) from Tarjeta where idUser = '{0}'", Session["user"]);
                ds = Utilities.Ejecutar(cmd);

                int filas = (int)ds.Tables[0].Rows[0][0];

                if (filas > 0)
                {
                    cmd = string.Format("select * from Tarjeta where idUser = '{0}'", Session["user"]);
                    ds = Utilities.Ejecutar(cmd);

                    int numTarjeta = (int)ds.Tables[0].Rows[0]["numTarjet"];
                    int cvv = (int)ds.Tables[0].Rows[0]["cvv"];
                    string expire = ds.Tables[0].Rows[0]["expireDate"].ToString();
                    decimal saldo = (decimal)ds.Tables[0].Rows[0]["saldo"];                

                    if (t.numTarjet == numTarjeta && t.cvv == cvv && t.expireDate.ToString() == expire)
                    {
                        // TOTAL VENTA/PAGO
                        cmd = string.Format("select sum(subTotal) from Carrito where idUser = '{0}'", Session["user"]);
                        ds = Utilities.Ejecutar(cmd);
                        v.total = (decimal)ds.Tables[0].Rows[0][0];

                        if (v.total <= saldo)
                        {
                            // DATOS PAGO
                            p.idUser = int.Parse(Session["user"].ToString());
                            p.quantity = (decimal)ds.Tables[0].Rows[0][0];
                            p.createdAt = DateTime.Now;

                            if (p != null)
                            {
                                db.Pago.Add(p);
                                db.SaveChanges();

                                saldo = saldo - v.total;
                                cmd = string.Format("update Tarjeta set saldo = '{0}' where idUser = '{1}'", saldo, Session["user"]);
                                Utilities.Ejecutar(cmd);

                                // DATOS VENTA
                                v.createdAt = DateTime.Now;
                                cmd = string.Format("select idClient from Cliente where idUser = '{0}'", Session["user"]);
                                ds = Utilities.Ejecutar(cmd);
                                v.idClient = (int)ds.Tables[0].Rows[0][0];

                                if (v != null)
                                {
                                    db.Venta.Add(v);
                                    db.SaveChanges();
                                }

                                // DATOS DETALLE - VENTA
                                cmd = string.Format("select idVent from Venta where idClient = '{0}'", v.idClient);
                                ds = Utilities.Ejecutar(cmd);
                                int venta = (int)ds.Tables[0].Rows[0][0];

                                cmd = string.Format("select * from Carrito where idUser = '{0}'", Session["user"]);
                                ds = Utilities.Ejecutar(cmd);

                                int idProd = (int)ds.Tables[0].Rows[0]["idProd"];
                                int cantidad = d.quantity = (int)ds.Tables[0].Rows[0]["quantity"];
                                decimal subTotal = (decimal)ds.Tables[0].Rows[0]["subTotal"];

                                cmd = string.Format("select price from Producto where idProd = '{0}'", idProd);
                                ds = Utilities.Ejecutar(cmd);

                                d.idVent = venta;
                                d.iProd = idProd;
                                d.price = (decimal)ds.Tables[0].Rows[0][0];
                                d.quantity = cantidad;
                                d.subTotal = subTotal;
                                d.createdAt = DateTime.Now;

                                if (d != null)
                                {
                                    db.DetalleVenta.Add(d);
                                    db.SaveChanges();
                                }

                                // Disminuir stock
                                cmd = string.Format("select stock from Producto where idProd = '{0}'", idProd);
                                ds = Utilities.Ejecutar(cmd);

                                int stock = (int)ds.Tables[0].Rows[0][0];
                                stock = stock - cantidad;

                                cmd = string.Format("update Producto set stock = '{0}' where idProd = '{1}'", stock, idProd);
                                Utilities.Ejecutar(cmd);

                                // Eliminar productos vendidos - carrito
                                cmd = string.Format("delete from Carrito where idUser = '{0}'", Session["user"]);
                                Utilities.Ejecutar(cmd);

                                return RedirectToAction("Home", "Home");
                            }

                            ViewBag.Error = "Error de pago";
                            return View();
                        }
                        else
                        {
                            ViewBag.Error = "No tienes suficiente dinero :(";
                            return View();
                        }
                    }
                    else if (t.numTarjet.ToString() != "" && t.cvv.ToString() != "" && t.expireDate.ToString() != "")
                    {
                        ViewBag.Error = "Ingresa tus datos";
                        return View();
                    }
                    else
                    {
                        ViewBag.Error = "Tus credenciales no coinciden";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Error = "Tus credenciales no existen";
                    return View();
                }
            }
            else
            {
                ViewBag.Error = "Inicia sesión";
                return View();
            }
        }





        // GET: Tarjeta
        public ActionResult Index()
        {
            var tarjeta = db.Tarjeta.Include(t => t.Usuario);
            return View(tarjeta.ToList());
        }

        // GET: Tarjeta/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tarjeta tarjeta = db.Tarjeta.Find(id);
            if (tarjeta == null)
            {
                return HttpNotFound();
            }
            return View(tarjeta);
        }

        // GET: Tarjeta/Create
        public ActionResult Create()
        {
            ViewBag.idUser = new SelectList(db.Usuario, "idUser", "photo");
            return View();
        }

        // POST: Tarjeta/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idTarjet,numTarjet,codeTarjet,cvv,nameTarjet,saldo,createdAt,expireDate,state,idUser")] Tarjeta tarjeta)
        {
            if (ModelState.IsValid)
            {
                db.Tarjeta.Add(tarjeta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idUser = new SelectList(db.Usuario, "idUser", "photo", tarjeta.idUser);
            return View(tarjeta);
        }

        // GET: Tarjeta/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tarjeta tarjeta = db.Tarjeta.Find(id);
            if (tarjeta == null)
            {
                return HttpNotFound();
            }
            ViewBag.idUser = new SelectList(db.Usuario, "idUser", "photo", tarjeta.idUser);
            return View(tarjeta);
        }

        // POST: Tarjeta/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idTarjet,numTarjet,codeTarjet,cvv,nameTarjet,saldo,createdAt,expireDate,state,idUser")] Tarjeta tarjeta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tarjeta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idUser = new SelectList(db.Usuario, "idUser", "photo", tarjeta.idUser);
            return View(tarjeta);
        }

        // GET: Tarjeta/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tarjeta tarjeta = db.Tarjeta.Find(id);
            if (tarjeta == null)
            {
                return HttpNotFound();
            }
            return View(tarjeta);
        }

        // POST: Tarjeta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tarjeta tarjeta = db.Tarjeta.Find(id);
            db.Tarjeta.Remove(tarjeta);
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
