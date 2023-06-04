using Everyday.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Everyday.Controllers
{
    public class TiendaController : Controller
    {
        private EverydayDB db = new EverydayDB();

        public static int idProd { get; set; }

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
            idProd = id;

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
                ViewBag.Error = "Inicia sesión";
                return RedirectToAction("Description", "Categoria");
            }
            return RedirectToAction("Index", "Carrito");
        }
    }
}