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
    public class AdminRequestController : Controller
    {
        LockerManagementSystemEntities db = new LockerManagementSystemEntities();

        // GET: AdminRequest
        public ActionResult Index()
        {
            return View();
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
                            ViewBag.errorMessage = "Karyawan ini sudah melakukan request loker permanen!";
                            ModelState.Clear();
                            return View("index");
                        }

                        var alreadyHave = AlreadyHave(model.BadgeId, 1);
                        if (alreadyHave == true)
                        {
                            ViewBag.errorMessage = "Karyawan ini sudah memiliki loker permanen!";
                            ModelState.Clear();
                            return View("index");
                        }

                    }
                    else if (model.TransactionType == 2) //temporary
                    {

                        var alreadyRequest = AlreadyRequest(model.BadgeId, 2);
                        if (alreadyRequest == true)
                        {
                            ViewBag.errorMessage = "Karyawan ini sudah melakukan request loker temporary!";
                            ModelState.Clear();
                            return View("index");
                        }

                        var alreadyHave = AlreadyHave(model.BadgeId, 2);
                        if (alreadyHave == true)
                        {
                            ViewBag.errorMessage = "Karyawan ini sudah memiliki loker temporary!";
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
                        ViewBag.errorMessage = "Karyawan ini tidak memiliki kunci loker untuk ditukar";
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


                transaction.Status = (int)StatusEnum.Release;
                transaction.Releaseby = UserHelpers.CurrentUser();
                transaction.DateRelease = DateTime.UtcNow.ToBatamTime();


                var locker = db.Locker.FirstOrDefault(x => x.Id == model.LockerId);
                if (locker == null)
                {
                }

                transaction.LockerNumber = locker.LockerNumber;
                transaction.LockerId = locker.Id;

                db.Transaction.Add(transaction);
                db.SaveChanges();

                //add into employee locker
                EmployeeLocker req1 = new EmployeeLocker
                {
                    TransactionId = transaction.TransactionId,
                    BadgeId = transaction.BadgeId,
                    LockerNumber = locker.LockerNumber,
                    LockerID = locker.Id
                };
                db.EmployeeLocker.Add(req1);
                db.SaveChanges();

                //decerease locker key stock
                locker.Stock = locker.Stock - 1;
                db.Entry(locker).State = System.Data.Entity.EntityState.Modified;
                var isSaved = db.SaveChanges();

                if (isSaved > 0)
                {
                    if (transaction.TransactionType == 3 && model.IsChangeWithFriend == 1)
                    {
                        ViewBag.successMessage = "Release berhasil, mantap mamank!";
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
    }
}