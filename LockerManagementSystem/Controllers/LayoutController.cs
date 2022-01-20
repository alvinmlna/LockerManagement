using LockerManagementSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LockerManagementSystem.Controllers
{
    public class LayoutController : Controller
    {
        private LockerManagementSystemEntities db = new LockerManagementSystemEntities();

        // GET: Layout
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Navbar()
        {
            var username = User.Identity.Name.ToLower();

            var haveAcess = db.Admin.FirstOrDefault(x => x.Username.ToLower() == username);
            if (haveAcess != null)
            {
                ViewBag.access = haveAcess.Access;
            } else
            {
                ViewBag.access = 0;
            }

            return PartialView();
        }

        public ActionResult AdminNavbar()
        {
            var username = User.Identity.Name.ToLower();

            var haveAcess = db.Admin.FirstOrDefault(x => x.Username.ToLower() == username);
            if (haveAcess != null)
            {
                ViewBag.access = haveAcess.Access;
            }
            else
            {
                ViewBag.access = 0;
            }

            return PartialView();
        }
    }
}