using Everyday.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Everyday.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Usuario u) 
        {
            using (EverydayDB db = new EverydayDB())
            {
                if (u != null)
                {
                    var LogUser = (from Usuario in db.Usuario
                                   where Usuario.email == u.email.Trim() && Usuario.keyUser == u.keyUser.Trim()
                                   select Usuario).FirstOrDefault();

                    if (LogUser != null)
                    {
                        Session["user"] = LogUser.idUser;
                        return RedirectToAction("Home", "Home");
                    }
                    else
                    {
                        ViewBag.Error = "No estás registrado!";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Error = "Usuario o contraseña incorrecta!";
                    return View();
                }
            }
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(Usuario usuario)
        {
            try
            {
                if (usuario != null)
                {
                    //WebImage image = new WebImage(File.InputStream);
                    //usuario.photo = image.GetBytes();

                    using (EverydayDB db = new EverydayDB())
                    {
                        db.Usuario.Add(usuario);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Login");

                }
                else
                {
                    String message = "Debes ingresar tus datos!";
                    ViewBag.Error1 = message;
                    return View();
                }
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }

        public ActionResult LogOff()
        {
            Session.Remove("user");
            return RedirectToAction("Home", "Home");
        }
    }
}