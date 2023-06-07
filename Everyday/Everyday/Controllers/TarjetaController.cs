using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
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
        public ActionResult Pago()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Pago(Tarjeta t)
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
                        Utilities u = new Utilities();
                        cmd = string.Format("select idProd, price, from Producto where idProd = '{0}'", Utilities.idProducto);
                        ds = Utilities.Ejecutar(cmd);
                        int idProduct = (int)ds.Tables[0].Rows[0]["idProd"];
                        decimal price = (int)ds.Tables[0].Rows[0]["price"];

                        // TOTAL VENTA/PAGO
                        v.total = 1 * price;

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
                                                            
                                v.total = price;
                                v.createdAt = DateTime.Now;
                                v.idUser = (int)Session["user"];

                                if (v != null)
                                {
                                    db.Venta.Add(v);
                                    db.SaveChanges();
                                }

                                // DATOS DETALLE - VENTA
                                cmd = string.Format("insert into DetalleVenta(idVent, idProd, price, quantity, subTotal, createdAt) " +
                                    "values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", v.idVent, idProduct, price, 1, price, DateTime.Now);
                                Utilities.Ejecutar(cmd);

                                // Disminuir stock
                                cmd = string.Format("update Producto set stock = stock - '{0}' where idProd = '{1}'", 1, idProduct);
                                Utilities.Ejecutar(cmd);

                                return RedirectToAction("Home", "Home");
                            }

                            ViewBag.Error = "Error al procesar pago";
                            return View();
                        }
                        else
                        {
                            ViewBag.Error = "No tienes suficiente dinero :(";
                            return View();
                        }
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
                                //cmd = string.Format("select idClient from Cliente where idUser = '{0}'", Session["user"]);
                                //ds = Utilities.Ejecutar(cmd);
                                v.idUser = (int)Session["user"];

                                if (v != null)
                                {
                                    db.Venta.Add(v);
                                    db.SaveChanges();
                                }

                                // DATOS DETALLE - VENTA
                                cmd = string.Format("insert into DetalleVenta(idVent, idProd, price, quantity, subTotal, createdAt) " +
                                    " select '{0}', p.idProd, p.price, quantity, subTotal, GETDATE() from Carrito" +
                                    " inner join Producto as p on p.idProd = Carrito.idProd where idUser = '{1}'", v.idVent, Session["user"]);
                                Utilities.Ejecutar(cmd);

                                // Disminuir stock
                                cmd = string.Format("update Producto set stock = stock - c.quantity " +
                                    "from Producto as p " +
                                    "inner join Carrito as c " +
                                    "on p.idProd = c.idProd where idUser = '{0}'", Session["user"]);
                                Utilities.Ejecutar(cmd);

                                // Eliminar productos vendidos - carrito
                                cmd = string.Format("delete from Carrito where idUser = '{0}'", Session["user"]);
                                Utilities.Ejecutar(cmd);

                                return RedirectToAction("Home", "Home");
                            }

                            ViewBag.Error = "Error al procesar pago";
                            return View();
                        }
                        else
                        {
                            ViewBag.Error = "No tienes suficiente dinero :(";
                            return View();
                        }
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
