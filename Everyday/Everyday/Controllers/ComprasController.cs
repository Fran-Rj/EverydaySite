using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Everyday.Models;

namespace Everyday.Controllers
{
    public class ComprasController : Controller
    {
        private EverydayDB db = new EverydayDB();

        // GET: Compras
        public ActionResult Index()
        {
            if (Session["user"] != null)
            {
                int id = int.Parse(Session["user"].ToString());
                var compras = db.DetalleVenta.Include(x => x.Producto).Include(x => x.Venta).Where(x => x.Venta.idUser == id);
                return View(compras.ToList());
            }
            return RedirectToAction("Login", "Account");
        }
    }
}