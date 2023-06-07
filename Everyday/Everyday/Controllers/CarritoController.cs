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
    public class CarritoController : Controller
    {
        private EverydayDB db = new EverydayDB();

        [HttpPost]
        public ActionResult Res(int id)
        {
            string cmd = string.Format("select * from Carrito where idProd = '{0}'", id);
            DataSet ds = Utilities.Ejecutar(cmd);
            int cantidad = (int)ds.Tables[0].Rows[0]["quantity"];

            cmd = string.Format("select price from Producto where idProd = '{0}'", id);
            ds = Utilities.Ejecutar(cmd);
            decimal precio = (decimal)ds.Tables[0].Rows[0]["price"];

            if (cantidad > 1)
            {                
                cantidad = cantidad - 1;
                decimal subtotal = cantidad * precio;
                cmd = string.Format("update Carrito set quantity = '{0}', subTotal = '{1}' where idProd = '{2}'", cantidad, subtotal, id);
                Utilities.Ejecutar(cmd);
                return View("Index");
            }
            else
            {
                ViewBag.Error = "";
                return View("Index");
            }
        }

        [HttpPost]
        public ActionResult Sum(int id)
        {
            string cmd = string.Format("select quantity from Carrito where idProd = '{0}'", id);
            DataSet ds = Utilities.Ejecutar(cmd);
            int cantidad = (int)ds.Tables[0].Rows[0][0];

            cmd = string.Format("select stock, price from Producto where idProd = '{0}'", id);
            ds = Utilities.Ejecutar(cmd);
            int stock = (int)ds.Tables[0].Rows[0]["stock"];
            decimal precio = (decimal)ds.Tables[0].Rows[0]["price"];

            if (cantidad < stock)
            {
                cantidad = cantidad + 1;
                decimal subtotal = cantidad * precio;
                cmd = string.Format("update Carrito set quantity = '{0}', subTotal = '{1}' where idProd = '{2}'", cantidad, subtotal, id);
                Utilities.Ejecutar(cmd);
                return View("Index");
            }
            else
            {
                ViewBag.Error1 = "Max: " + stock;
                return View("Index");
            }
            
        }

        // GET: Carrito
        public ActionResult Index()
        {
            return View();
        }

        private int getIndex(int id)
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

        [HttpPost]
        public ActionResult AddToCarrito(int id)
        {
            Carrito c = new Carrito();
            string cmd = "";
            DataSet ds;
            decimal precio = 0;

            int indexExist = getIndex(id);

            if (Session["user"] != null)
            {
                cmd = string.Format("select * from Producto where idProd = '{0}'", id);
                ds = Utilities.Ejecutar(cmd);
                precio = (decimal)ds.Tables[0].Rows[0]["price"];
                int stock = (int)ds.Tables[0].Rows[0]["stock"];

                if (stock > 0)
                {
                    if (indexExist == -1)
                    {
                        c.idProd = id;
                        c.quantity = 1;
                        c.subTotal = c.quantity * precio;
                        c.idUser = int.Parse(Session["user"].ToString());

                        if (ModelState.IsValid)
                        {
                            db.Carrito.Add(c);
                            db.SaveChanges();
                            return RedirectToAction("Index", "Carrito");
                        }
                    }
                    else
                    {
                        // Extraigo
                        cmd = string.Format("select * from Carrito where idProd = '{0}'", id);
                        ds = Utilities.Ejecutar(cmd);

                        int cantidad = (int)ds.Tables[0].Rows[0]["quantity"];
                        cantidad = cantidad + 1;

                        // Actualizo
                        cmd = string.Format("update Carrito set quantity = '{0}' where idProd = '{1}'", cantidad, id);
                        Utilities.Ejecutar(cmd);

                        // Vuelvo a extraer
                        cmd = string.Format("select * from Carrito where idProd = '{0}'", id);
                        ds = Utilities.Ejecutar(cmd);
                        cantidad = (int)ds.Tables[0].Rows[0]["quantity"];
                        decimal subtotal = cantidad * precio;

                        // Vuelvo a actualizar
                        cmd = string.Format("update Carrito set subTotal = '{0}' where idProd = '{1}'", subtotal, id);
                        Utilities.Ejecutar(cmd);

                        return RedirectToAction("Index", "Carrito");
                    }
                }
                else
                {
                    ViewBag.Error = "El producto se ha agotado";
                    return RedirectToAction("Description", "Categoria");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("Index", "Carrito");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Carrito carrito = db.Carrito.Find(id);
            db.Carrito.Remove(carrito);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
