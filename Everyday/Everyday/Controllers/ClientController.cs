using Everyday.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Everyday.Controllers
{
    public class ClientController : Controller
    {
        private EverydayDB db = new EverydayDB();

        // GET: Client
        public ActionResult Registrar()
        {
            if (Session["user"] != null)
            {
                string cmd = string.Format("select count(*) from Cliente where idUser = '{0}'", Session["user"]);
                DataSet ds = Utilities.Ejecutar(cmd);

                int filas = (int)ds.Tables[0].Rows[0][0];

                if (filas > 0)
                {
                    return RedirectToAction("Pago", "Tarjeta");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.idCity = new SelectList(db.Ciudad, "idCity", "nameCity");
            ViewBag.idDepa = new SelectList(db.Departamento, "idDepa", "nameDepa");
            ViewBag.idPais = new SelectList(db.Pais, "idPais", "namePais");
            ViewBag.idUser = new SelectList(db.Usuario, "idUser", "photo");
            return View();
        }

        public ActionResult Registrar([Bind(Include = "idClient,nameClient,lastnameClient,nitClient,addressClient,phone,idPais,idDepa,idCity,idUser,createdAt")] Cliente cliente)
        {
            if (Session["user"] != null)
            {
                cliente.idUser = int.Parse(Session["user"].ToString());
                cliente.createdAt = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Cliente.Add(cliente);
                    db.SaveChanges();
                    return RedirectToAction("Pago", "Tarjeta");
                }
            }

            ViewBag.idCity = new SelectList(db.Ciudad, "idCity", "nameCity", cliente.idCity);
            ViewBag.idDepa = new SelectList(db.Departamento, "idDepa", "nameDepa", cliente.idDepa);
            ViewBag.idPais = new SelectList(db.Pais, "idPais", "namePais", cliente.idPais);
            return View(cliente);
        }

    }
}