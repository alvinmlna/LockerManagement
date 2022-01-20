using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LockerManagementSystem.App_Class.Security;
using LockerManagementSystem.DAL;

namespace LockerManagementSystem.Controllers
{
    [AuthorizationHandlerAttribute(Roles = "1")]
    public class SpecialEmployeesController : Controller
    {
        private LockerManagementSystemEntities db = new LockerManagementSystemEntities();

        // GET: SpecialEmployees
        public ActionResult Index()
        {
            return View(db.SpecialEmployee.ToList());
        }

        // GET: SpecialEmployees/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialEmployee specialEmployee = db.SpecialEmployee.Find(id);
            if (specialEmployee == null)
            {
                return HttpNotFound();
            }
            return View(specialEmployee);
        }

        // GET: SpecialEmployees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SpecialEmployees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BadgeId,Name")] SpecialEmployee specialEmployee)
        {
            if (ModelState.IsValid)
            {
                db.SpecialEmployee.Add(specialEmployee);
                db.SaveChanges();
                ViewBag.success = "Success";

                return RedirectToAction("Index");
            }

            return View(specialEmployee);
        }

        // GET: SpecialEmployees/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialEmployee specialEmployee = db.SpecialEmployee.Find(id);
            if (specialEmployee == null)
            {
                return HttpNotFound();
            }
            return View(specialEmployee);
        }

        // POST: SpecialEmployees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BadgeId,Name")] SpecialEmployee specialEmployee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(specialEmployee).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.success = "Success";

                return RedirectToAction("Index");
            }
            return View(specialEmployee);
        }

        // GET: SpecialEmployees/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialEmployee specialEmployee = db.SpecialEmployee.Find(id);
            db.SpecialEmployee.Remove(specialEmployee);
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
