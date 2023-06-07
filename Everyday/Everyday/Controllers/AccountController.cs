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
    public class AccountController : Controller
    {
        private EverydayDB db = new EverydayDB();

        public ActionResult Profil(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        [HttpPost]
        public ActionResult Profil(Usuario usuario) 
        {
            if (Session["user"] != null)
            {
                if (usuario != null)
                {
                    using (EverydayDB db = new EverydayDB())
                    {
                        Usuario user = db.Usuario.Find(usuario.idUser);
                        user.nameUser = usuario.nameUser;
                        user.gender = usuario.gender;
                        user.email = usuario.email;
                        user.keyUser = usuario.keyUser;
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Home", "Home");
        }

        [HttpGet]
        public ActionResult MostrarImagen(int Id)
        {
            using (EverydayDB db = new EverydayDB())
            {
                var imagen = (from u in db.Usuario
                              where u.idUser == Id
                              select u.photo).FirstOrDefault();
                return File(imagen, "png");
            }
        }

        [HttpPost]
        public ActionResult ChangeImage(Usuario usuario)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase File = Request.Files[0];
                if (File.ContentLength > 0 && File.ContentType.Contains("image"))
                {
                    WebImage image = new WebImage(File.InputStream);
                    usuario.photo = image.GetBytes();

                    Usuario user = db.Usuario.Find(usuario.idUser);
                    user.photo = usuario.photo;
                    db.SaveChanges();
                }
                else
                {
                    string fileName = Server.MapPath("~/Image/Avatar.png");
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    byte[] image = br.ReadBytes((int)fs.Length);
                    br.Close();
                    fs.Close();

                    usuario.photo = image;
                }                         
            }  

            return RedirectToAction("Home", "Home");
        }


        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Usuario u)
        {
            if( u != null)
            {
                using (EverydayDB db = new EverydayDB())
                {
                    var LogUser = (from Usuario in db.Usuario
                                   where Usuario.email == u.email.Trim() && Usuario.keyUser == u.keyUser.Trim()
                                   select Usuario).FirstOrDefault();

                    if (LogUser != null)
                    {
                        Session["user"] = LogUser.idUser;

                        string cmd = string.Format("select roleUser from Usuario where idUser = '{0}'", Session["user"]);
                        DataSet ds = Utilities.Ejecutar(cmd);
                        string rol = ds.Tables[0].Rows[0][0].ToString();

                        if (rol == "Admin")
                        {
                            return RedirectToAction("Everyday", "Home");
                        }

                        return RedirectToAction("Home", "Home");
                    }
                    else
                    {
                        ViewBag.Error = "Usuario o contraseña incorrecta!";
                        return View();
                    }
                }               
            }

            ViewBag.Error = "";
            return View();         
        }


        public ActionResult LogOff()
        {
            Session.Remove("user");
            return RedirectToAction("Login");
        }



        // GET: Usuario
        public ActionResult Index()
        {
            return View(db.Usuario.ToList());
        }

        // GET: Usuario/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuario/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idUser,photo,nameUser,gender,email,keyUser,roleUser,createdAt,state")] Usuario usuario)
        {
            usuario.roleUser = "Usuario";
            usuario.state = "Activo";
            usuario.createdAt = DateTime.Now;

            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase File = Request.Files[0];
                if (File.ContentLength > 0 && File.ContentType.Contains("image"))
                {
                    WebImage image = new WebImage(File.InputStream);
                    usuario.photo = image.GetBytes();
                }
                else
                {
                    string fileName = Server.MapPath("~/Image/Avatar.png");
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    byte[] image = br.ReadBytes((int)fs.Length);
                    br.Close();
                    fs.Close();

                    usuario.photo = image;
                }
            }

            if (ModelState.IsValid)
            {
                db.Usuario.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Login", "Account");
            }

            return View(usuario);
        }

        // GET: Usuario/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Usuario usuario)
        {
            if (usuario != null)
            {
                using (EverydayDB db = new EverydayDB())
                {
                    Usuario user = db.Usuario.Find(usuario.idUser);
                    user.nameUser = usuario.nameUser;
                    user.gender = usuario.gender;
                    user.email = usuario.email;
                    user.keyUser = usuario.keyUser;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        // GET: Usuario/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuario.Find(id);
            db.Usuario.Remove(usuario);
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
