using LockerManagementSystem.App_Class;
using LockerManagementSystem.App_Class.Helpers;
using LockerManagementSystem.App_Class.Security;
using LockerManagementSystem.DAL;
using LockerManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LockerManagementSystem.Controllers
{
    [AuthorizationHandlerAttribute(Roles = "1,2")]
    public class ReturnController : Controller
    {
        private LockerManagementSystemEntities db = new LockerManagementSystemEntities();

        // GET: Return
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Search(string badgeid)
        {
            var employee =
                db.EmployeeLocker.Where(x => x.BadgeId == badgeid)
                .Select(x => new ReturnViewModel
                {
                    TransactionId = x.TransactionId,
                    BadgeId = x.BadgeId,
                    Name = x.Transaction.Name,
                    Department = x.Transaction.Department,
                    Area = x.Transaction.Area,
                    Site = x.Transaction.Site,
                    LockerNunmber = x.LockerNumber,
                    LockerType = (x.Locker.LockerType == 1) ? "Permanent" : "Temporary",
                    DateTemporaryReturn = x.Transaction.DateTemporaryReturn
                }).ToList();

            var emp = GMDSHelpers.Find(badgeid);

            ViewBag.Name = emp?.Name;

            return View(employee);
        }

        [HttpGet]
        public ActionResult ReturnRequest(string Remark, int TransactionId)
        {
            var tobeUpdate = db.Transaction.FirstOrDefault(x => x.TransactionId == TransactionId);
            if (tobeUpdate != null)
            {
                tobeUpdate.Status = (int)StatusEnum.Return;
                tobeUpdate.DateReturn = DateTime.UtcNow.ToBatamTime();
                tobeUpdate.ReceivedBy = UserHelpers.CurrentUser();
                tobeUpdate.Remark = Remark;
                db.Entry(tobeUpdate).State = System.Data.Entity.EntityState.Modified;
                var isSaved = db.SaveChanges();

                if (isSaved > 0)
                {
                    var locker = db.EmployeeLocker.FirstOrDefault(x => x.TransactionId == TransactionId);
                    if (locker != null)
                    {
                        //naikkan stocknya karna kan dikembalikan
                        var lockerDetail = locker.Locker;
                        lockerDetail.Stock = lockerDetail.Stock + 1;
                        db.Entry(lockerDetail).State = System.Data.Entity.EntityState.Modified;

                        db.EmployeeLocker.Remove(locker);
                        db.SaveChanges();
                    }
                }
            }

            return null;
        }

        [HttpGet]
        public ActionResult Extend(int TransactionId)
        {
            var tobeUpdate = db.Transaction.FirstOrDefault(x => x.TransactionId == TransactionId);
            if (tobeUpdate != null)
            {
                var IsExpired = tobeUpdate.DateTemporaryReturn < DateTime.UtcNow.ToBatamTime();

                DateTime newTarget = DateTime.UtcNow.ToBatamTime().AddDays(3);

                if (IsExpired)
                {
                    tobeUpdate.DateTemporaryReturn = newTarget;
                } else
                {
                    tobeUpdate.DateTemporaryReturn = tobeUpdate.DateTemporaryReturn.Value.AddDays(3);
                }

                db.Entry(tobeUpdate).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return null;
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