using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LockerManagementSystem.App_Class.Security;
using LockerManagementSystem.DAL;
using LockerManagementSystem.ViewModels;
using PagedList;

namespace LockerManagementSystem.Controllers
{
    [AuthorizationHandlerAttribute(Roles = "1")]
    public class LockersController : Controller
    {
        private LockerManagementSystemEntities db = new LockerManagementSystemEntities();

        // GET: Lockers
        public ActionResult Index()
        {
            ViewBag.FOL     = 11;
            ViewBag.EOL     = 31;
            ViewBag.TESTING = 44;

            return View();
        }

        [HttpPost]
        public ActionResult LoadLocker()
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Getting all Customer data    
                IEnumerable<LockerViewModel> customerData = db.Locker.Where(x => x.IsActive)
                    .Select(x => new LockerViewModel
                    {
                        Id = x.Id,
                        LockerNumber = x.LockerNumber,
                        LockerKeyNumber = x.LockerKeyNumber,
                        LockerType = x.LockerType,
                        Area = x.Area,
                        Site = x.Site,
                        Stock = x.Stock,
                        ayam = "hehe",
                    }).ToList();

                //var usedLocker = db.EmployeeLocker.Select(x => x.LockerNumber).Distinct();

                //foreach (var item in customerData)
                //{
                //    if (usedLocker.Contains(item.LockerNumber))
                //    {
                //        item.ayam = "Not Available";
                //    } else
                //    {
                //        item.ayam = "Available";
                //    }
                //}

                //Sorting    
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDir);
                }
                else
                {
                    customerData = customerData.OrderByDescending(x => x.Area).ThenByDescending(x => x.LockerNumber);
                }

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m =>
                    m.LockerNumber.Contains(searchValue) || m.LockerKeyNumber?.Contains(searchValue) == true);
                }


                //total number of rows count     
                recordsTotal = customerData.Count();
                //Paging     
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        // GET: Lockers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Locker locker = db.Locker.Find(id);
            if (locker == null)
            {
                return HttpNotFound();
            }
            return View(locker);
        }

        // GET: Lockers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Lockers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Locker locker)
        {
            if (ModelState.IsValid)
            {
                var checkExist = db.Locker.FirstOrDefault(x => x.LockerNumber == locker.LockerNumber);
                if (checkExist != null)
                {
                    if (checkExist.IsActive == false)
                    {
                        ViewBag.enableLocker = checkExist.LockerNumber;
                        ModelState.AddModelError("LockerNumber", "Locker number is already exist in server, please enable.");
                    } else
                    {

                        ModelState.AddModelError("LockerNumber", "Locker number is already exist");
                    }

                    return View(locker);
                }

                locker.IsActive = true;
                db.Locker.Add(locker);

                ViewBag.success = "Success";

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(locker);
        }

        [HttpGet]
        public ActionResult EnableLocker(int? id)
        {
            var locker = db.Locker.FirstOrDefault(x => x.Id == id);
            if (locker != null)
            {
                locker.IsActive = true;
                db.Entry(locker).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // GET: Lockers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Locker locker = db.Locker.Find(id);
            if (locker == null)
            {
                return HttpNotFound();
            }
            return View(locker);
        }

        // POST: Lockers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Locker locker)
        {
            if (ModelState.IsValid)
            {
                locker.IsActive = true;
                db.Entry(locker).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.success = "Success";

                return RedirectToAction("Index");
            }
            return View(locker);
        }

        // GET: Lockers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Locker locker = db.Locker.Find(id);
            if (locker == null)
            {
                return HttpNotFound();
            }
            return View(locker);
        }

        // POST: Lockers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Locker locker = db.Locker.Find(id);

            locker.IsActive = false;
            db.Entry(locker).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Lockers/Edit/5
        public ActionResult TrackTransaction(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var locker = db.Transaction.Where(x => x.Locker.Id == id)
                .OrderBy(x => x.DateRequest)
                .Select(x => new RequestViewModel
                {
                    TransactionId = x.TransactionId,
                    BadgeId = x.BadgeId,
                    Name = x.Name,
                    Department = x.Department,
                    Area = x.Area,
                    Site = x.Site,
                    DateRequest = x.DateRequest,
                    DateReturn = x.DateReturn,
                    ReturnBy = x.ReceivedBy,
                    DateRelease = x.DateRelease,
                    ReleaseBy = x.Releaseby
                })
                .OrderByDescending(x => x.DateRelease)
                .ToList();

            if (locker == null)
            {
                return HttpNotFound();
            }
            ViewBag.LockerNumber = id;
            return View(locker);
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
