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
    public class ClienteController : Controller
    {
        private EverydayDB db = new EverydayDB();

        // GET: Cliente
        public ActionResult Index()
        {
            var cliente = db.Cliente.Include(c => c.Ciudad).Include(c => c.Departamento).Include(c => c.Pais).Include(c => c.Usuario);
            return View(cliente.ToList());
        }

        // GET: Cliente/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Cliente.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // GET: Cliente/Create
        public ActionResult Create()
        {
            if (Session["user"] != null)
            {
                string cmd = string.Format("select count(*) from Cliente where idUser = '{0}'", Session["user"]);
                DataSet ds = Utilities.Ejecutar(cmd);

                int filas = (int)ds.Tables[0].Rows[0][0];

                if (filas > 0)
                {
                    return RedirectToAction("Pagos", "Tarjeta");
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

        // POST: Cliente/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idClient,nameClient,lastnameClient,nitClient,addressClient,phone,idPais,idDepa,idCity,idUser,createdAt")] Cliente cliente)
        {
            if (Session["user"] != null)
            {
                cliente.idUser = int.Parse(Session["user"].ToString());
                cliente.createdAt = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Cliente.Add(cliente);
                    db.SaveChanges();
                    return RedirectToAction("Pagos", "Tarjeta");
                }
            }

            ViewBag.idCity = new SelectList(db.Ciudad, "idCity", "nameCity", cliente.idCity);
            ViewBag.idDepa = new SelectList(db.Departamento, "idDepa", "nameDepa", cliente.idDepa);
            ViewBag.idPais = new SelectList(db.Pais, "idPais", "namePais", cliente.idPais);
            return View(cliente);
        }

        // GET: Cliente/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Cliente.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            ViewBag.idCity = new SelectList(db.Ciudad, "idCity", "nameCity", cliente.idCity);
            ViewBag.idDepa = new SelectList(db.Departamento, "idDepa", "nameDepa", cliente.idDepa);
            ViewBag.idPais = new SelectList(db.Pais, "idPais", "namePais", cliente.idPais);
            ViewBag.idUser = new SelectList(db.Usuario, "idUser", "photo", cliente.idUser);
            return View(cliente);
        }

        // POST: Cliente/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idClient,nameClient,lastnameClient,nitClient,addressClient,phone,idPais,idDepa,idCity,idUser,createdAt")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idCity = new SelectList(db.Ciudad, "idCity", "nameCity", cliente.idCity);
            ViewBag.idDepa = new SelectList(db.Departamento, "idDepa", "nameDepa", cliente.idDepa);
            ViewBag.idPais = new SelectList(db.Pais, "idPais", "namePais", cliente.idPais);
            ViewBag.idUser = new SelectList(db.Usuario, "idUser", "photo", cliente.idUser);
            return View(cliente);
        }

        // GET: Cliente/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Cliente.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Cliente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cliente cliente = db.Cliente.Find(id);
            db.Cliente.Remove(cliente);
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
