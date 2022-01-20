using LockerManagementSystem.App_Class;
using LockerManagementSystem.App_Class.Helpers;
using LockerManagementSystem.DAL;
using LockerManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LockerManagementSystem.Controllers
{
    public class RequestController : Controller
    {
        LockerManagementSystemEntities db = new LockerManagementSystemEntities();

        // GET: Requestt
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult UpdateLocker(int type, int area, string site)
         {
            // Getting all Customer data    
            // Ignore  Site
            var allUsedLocker = db.EmployeeLocker.Select(x => x.LockerID).ToList();
            var model = db.Locker.Where(x => !allUsedLocker.Contains(x.Id) && x.Area == area && x.IsActive).ToList();

            if (type == (int)LockerEnum.permanent || type == 3)
            {
                model = model
                .Where(x => x.LockerType == (int)LockerEnum.permanent).ToList();
            }
            else if (type == (int)LockerEnum.temporary)
            {
                model = model
              .Where(x => x.LockerType == (int)LockerEnum.temporary).ToList();
            }
            else
            {
                model = new List<Locker>();
            }

            var result = model.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.LockerNumber
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RecentRequest()
        {
            var fromDB = db.Transaction
                .Where(x => x.Status == (int)StatusEnum.Request)
                .Select(x => new RequestViewModel
                {
                    BadgeId = x.BadgeId,
                    Name = x.Name,
                    TransactionType = x.TransactionType,
                    LockerNumber = x.LockerNumber,
                    DateRequest = x.DateRequest,
                    Area = x.Area
                })
                .ToList()
                .OrderByDescending(x => x.DateRequest)
                .ToList();

            return PartialView(fromDB);
        }

        [HttpGet]
        public JsonResult EmployeeName(string employeeId)
        {
            var employee = GMDSHelpers.Find(employeeId);
            if (employee != null)
            {
                return Json(new { name = employee.Name, dept = employee.Department }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { name = "Employee not found", dept = "Employee not found"  }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitRequest(RequestViewModel model)
        {
            if (ModelState.IsValid)
            {

                var special = isSpecialAccess(model.BadgeId);
                if (special == false)
                {
                    if (model.TransactionType == 1) //Permanent / Temporary
                    {
                        var alreadyRequest = AlreadyRequest(model.BadgeId, 1);
                        if (alreadyRequest == true)
                        {
                            ViewBag.errorMessage = "Anda sudah melakukan request loker permanen!";
                            ModelState.Clear();
                            return View("index");
                        }

                        var alreadyHave = AlreadyHave(model.BadgeId, 1);
                        if (alreadyHave == true)
                        {
                            ViewBag.errorMessage = "Anda sudah memiliki loker permanen!";
                            ModelState.Clear();
                            return View("index");
                        }

                    }
                    else if (model.TransactionType == 2) //temporary
                    {

                        var alreadyRequest = AlreadyRequest(model.BadgeId, 2);
                        if (alreadyRequest == true)
                        {
                            ViewBag.errorMessage = "Anda sudah melakukan request loker temporary!";
                            ModelState.Clear();
                            return View("index");
                        }

                        var alreadyHave = AlreadyHave(model.BadgeId, 2);
                        if (alreadyHave == true)
                        {
                            ViewBag.errorMessage = "Anda sudah memiliki loker temporary!";
                            ModelState.Clear();
                            return View("index");
                        }

                    }
                }



                if (model.TransactionType == 3 && model.IsChangeWithFriend == 1) //tukaran dengan teman
                {
                    //check jika ia belum mengembalikan loker
                    var havePermanentLocker = db.EmployeeLocker
                        .FirstOrDefault(x => x.BadgeId == model.BadgeId &&
                                             x.Locker.LockerType == (int)LockerEnum.permanent);

                    if (havePermanentLocker == null)
                    {
                        ViewBag.errorMessage = "Anda tidak memiliki kunci loker untuk ditukar";
                        ModelState.Clear();
                        return View("index");
                    }
                    
                     var friend_havePermanentLocker = db.EmployeeLocker
                        .FirstOrDefault(x => x.BadgeId == model.FriendBadgeNumber &&
                                             x.Locker.LockerType == (int)LockerEnum.permanent);

                    if (friend_havePermanentLocker == null)
                    {
                        ViewBag.errorMessage = "Teman anda tidak memiliki kunci loker untuk ditukar";
                        ModelState.Clear();
                        return View("index");
                    }
                }
                else if (model.TransactionType == 3 && model.IsChangeWithFriend == 2) //tukaran loker aja
                {
                    model.FriendBadgeNumber = null;
                    model.FriendDepartment = null;
                    model.FriendName = null;
                }

                var empData = GMDSHelpers.Find(model.BadgeId);

                Transaction transaction = new Transaction();
                transaction.BadgeId = model.BadgeId;
                transaction.Name = model.Name;
                transaction.Department = model.Department;

                transaction.Designation = empData?.Designation;
                transaction.EmployeeGender = empData?.Gender;

                transaction.PIC = model.PIC;

                transaction.Area = model.AreaEnum.ToString();
                transaction.Site = model.Site;
                transaction.TransactionType = model.TransactionType;
                transaction.FriendBadgeNumber = model.FriendBadgeNumber;
                transaction.DateRequest = DateTime.UtcNow.ToBatamTime();
                transaction.Status = (int)StatusEnum.Request;

                db.Transaction.Add(transaction);

                var isSaved = db.SaveChanges();
                if (isSaved > 0)
                {
                    if (transaction.TransactionType == 3 && model.IsChangeWithFriend == 1)
                    {
                        ViewBag.successMessage = "Request berhasil, silahkan konfirmasi ke attire room";
                    }
                        else
                    {
                        ViewBag.successMessage = "Request berhasil, silahkan mengambil kunci di attire room";
                    }
                }
                    else
                {
                    ViewBag.errorMessage = "Request failed";
                }

                ModelState.Clear();
                return View("index");
            }
            return View("Index", model);
        }

        private bool AlreadyRequest(string badgeId, int transactionType)
        {
            var exist = db.Transaction
                       .Where(x => x.BadgeId == badgeId
                       && (x.TransactionType == transactionType)
                       && x.Status == (int)StatusEnum.Request).Any();

            return exist;
        }

        private bool AlreadyHave(string badgeId, int transactionType)
        {
            var havePermanentLocker = db.EmployeeLocker
                      .Where(x => x.BadgeId == badgeId &&
                    x.Locker.LockerType == transactionType).Any();

            return havePermanentLocker;
        }

        private bool isSpecialAccess(string badge)
        {
            return db.SpecialEmployee.Where(x => x.BadgeId == badge).Any();
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