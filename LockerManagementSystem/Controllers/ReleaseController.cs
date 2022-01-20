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
using System.Web.Security;

namespace LockerManagementSystem.Controllers
{
    [AuthorizationHandlerAttribute(Roles = "1,2")]
    public class ReleaseController : Controller
    {
        private LockerManagementSystemEntities db = new LockerManagementSystemEntities();
        // GET: Release
        public ActionResult Index(string searchString)
        {
            List<RequestViewModel> model = new List<RequestViewModel>();
            var temp = db.Transaction
                .Where(x => x.Status == (int)StatusEnum.Request)
                .Select(x => new RequestViewModel
            {
                TransactionId = x.TransactionId,
                BadgeId = x.BadgeId,
                Name = x.Name,
                Department = x.Department,
                Area = x.Area,
                Site = x.Site,
                DateRequest = x.DateRequest,
                TransactionType = x.TransactionType
            });

            var currentUser = UserHelpers.GetLoggedUser();
            if (currentUser != null && currentUser.Access == (int)AccessEnum.AttireTeam)
            {
                temp = temp.Where(x => x.TransactionType == (int)LockerEnum.temporary ); //untuk attireteam hanya untuk temporary locker
            }


            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();

                model = temp.Where(s => s.BadgeId.Contains(searchString) || s.Name.Contains(searchString)).ToList();
            } else
            {
                model = temp.ToList();
            }

            model = model
            .OrderByDescending(x => x.DateRequest)
            .ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult Process(int transactionId)
        {
            var _db = db.Transaction.FirstOrDefault(x => x.TransactionId == transactionId && x.Status == (int)StatusEnum.Request);
            if (_db != null)
            {
                var model = GetDetail(_db);
                if (_db.TransactionType == 3 && string.IsNullOrEmpty(_db.FriendBadgeNumber) == false )
                {
                    var myTransaction = db.Transaction
                      .Where(x => x.BadgeId == _db.BadgeId
                      && x.Status == (int)StatusEnum.Release
                      && x.TransactionType == (int)LockerEnum.permanent).FirstOrDefault();


                    model.MyTransactionId = myTransaction.TransactionId;
                    model.DateRequest = myTransaction.DateRequest;
                    model.LockerNumber = myTransaction.LockerNumber;

                    var friendTransaction = db.Transaction
                        .Where(x => x.BadgeId == _db.FriendBadgeNumber 
                        && x.Status == (int)StatusEnum.Release 
                        && x.TransactionType == (int)LockerEnum.permanent).FirstOrDefault();

                    if (friendTransaction != null)
                    {
                        model.FriendBadgeNumber = friendTransaction.BadgeId;
                        model.FriendName = friendTransaction.Name;
                        model.FriendDepartment = friendTransaction.Department;
                        model.FriendSite = friendTransaction.Site;
                        model.FriendArea = friendTransaction.Area;
                        model.FriendDateRequest = friendTransaction.DateRequest;
                        model.FriendLockerNumber = friendTransaction.LockerNumber;
                        model.FriendTransactionId = friendTransaction.TransactionId;

                        //Change with friend
                        return PartialView("ChangeWithFriendModal", model);
                    }
                }
                    else
                {
                    //model.ListOfLocker = GetLocker(model.TransactionType, model.AreaId, model.Site);
                    return PartialView("DefaultReleaseModal", model);
                }
            }

            return PartialView(null);
        }


        [HttpPost]
        public string DefaultRelease(int transactionId, int lockerId)
        {
            var transaction = db.Transaction.FirstOrDefault(x => x.TransactionId == transactionId);
            if (transaction != null)
            {

                //if temporary or permanent change request
                if (transaction.TransactionType == (int)LockerEnum.permanent || transaction.TransactionType == (int)LockerEnum.temporary)
                {
                    transaction.Status = (int)StatusEnum.Release;
                    transaction.Releaseby = UserHelpers.CurrentUser();
                    transaction.DateRelease = DateTime.UtcNow.ToBatamTime();

                    if (transaction.TransactionType == (int)LockerEnum.temporary)
                    {
                        transaction.DateTemporaryReturn = DateTime.UtcNow.ToBatamTime().AddDays(3); //tambah 3 hari untuk pengembalian
                    }

                    var locker = db.Locker.FirstOrDefault(x => x.Id == lockerId);
                    transaction.LockerNumber = locker.LockerNumber;
                    transaction.LockerId = locker.Id;

                    db.Entry(transaction).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    //add into employee locker
                    EmployeeLocker req1 = new EmployeeLocker
                    {
                        TransactionId = transaction.TransactionId,
                        BadgeId = transaction.BadgeId,
                        LockerNumber = transaction.LockerNumber,
                        LockerID = transaction.LockerId ?? 0
                    };
                    db.EmployeeLocker.Add(req1);
                    db.SaveChanges();

                    //decerease locker key stock
                    UpdateLocker(locker.Id, false);

                    return "success";
                }
                    else
                {
                    //change locker

                    var today = DateTime.UtcNow.ToBatamTime();

                    var myTransaction = db.Transaction
                        .Where(x => x.BadgeId == transaction.BadgeId
                        && x.Status == (int)StatusEnum.Release
                        && x.TransactionType == (int)LockerEnum.permanent).FirstOrDefault();

                    //set mytransaction to return / with remark change locker
                    myTransaction.DateReturn = today;
                    myTransaction.ReceivedBy = "Change locker";
                    myTransaction.Status = (int)StatusEnum.Return;
                    db.Entry(myTransaction).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //remove from employeelocker
                    var todelete1 = db.EmployeeLocker.FirstOrDefault(x => x.TransactionId == myTransaction.TransactionId);

                    //increase locker key stock
                    UpdateLocker(todelete1.Locker.Id, true);


                    db.EmployeeLocker.Remove(todelete1);
                    db.SaveChanges();

                    //set change transaction locker number to friend locker number

                    var changeTransaction = transaction;
                    var locker = db.Locker.FirstOrDefault(x => x.Id == lockerId);

                    changeTransaction.DateRelease = today;
                    changeTransaction.Releaseby = UserHelpers.CurrentUser();
                    changeTransaction.Status = (int)StatusEnum.Release;
                    changeTransaction.TransactionType = (int)LockerEnum.permanent;
                    changeTransaction.LockerNumber = locker.LockerNumber;
                    changeTransaction.LockerId = locker.Id;
                    db.Entry(changeTransaction).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //add into employee locker
                    EmployeeLocker req1 = new EmployeeLocker
                    {
                        TransactionId = changeTransaction.TransactionId,
                        BadgeId = changeTransaction.BadgeId,
                        LockerNumber = changeTransaction.LockerNumber,
                        LockerID =  changeTransaction.LockerId ?? 0
                    };
                    db.EmployeeLocker.Add(req1);
                    db.SaveChanges();

                    //decerease locker key stock
                    UpdateLocker(locker.Id, false);

                    return "success";
                }
            } 
            return "failed";
        }

        [HttpPost]
        public string ChangeRelease(int transactionId,int myTransactionId, int friendtransactionId)
        {
            var transaction = db.Transaction.FirstOrDefault(x => x.TransactionId == transactionId);
            if (transaction != null)
            {
                var changeTransaction   = db.Transaction.FirstOrDefault(x => x.TransactionId == transactionId);
                var myTransaction       = db.Transaction.FirstOrDefault(x => x.TransactionId == myTransactionId);
                var friendTransaction   = db.Transaction.FirstOrDefault(x => x.TransactionId == friendtransactionId);

                var today = DateTime.UtcNow.ToBatamTime();
                var currentUser = UserHelpers.CurrentUser();

                //kita keep disini, karna ketika sudah diganti nanti valuenya berubah 
                Locker myCurrentLocker = myTransaction.Locker;

                //set mytransaction to return / with remark change locker
                myTransaction.DateReturn = today;
                myTransaction.ReceivedBy = currentUser;
                friendTransaction.Remark = "Change locker";
                myTransaction.Status = (int)StatusEnum.Return;
                db.Entry(myTransaction).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //remove from employeelocker
                var todelete1 = db.EmployeeLocker.FirstOrDefault(x => x.TransactionId == myTransactionId);
                db.EmployeeLocker.Remove(todelete1);
                db.SaveChanges();

                //set friendTransaction to return / with remark change locker
                friendTransaction.DateReturn = today;
                friendTransaction.ReceivedBy = currentUser;
                friendTransaction.Remark = "Change locker";
                friendTransaction.Status = (int)StatusEnum.Return;
                db.Entry(friendTransaction).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //remove from employeelocker
                var todelete2 = db.EmployeeLocker.FirstOrDefault(x => x.TransactionId == friendtransactionId);
                db.EmployeeLocker.Remove(todelete2);
                db.SaveChanges();

                //set change transaction locker number to friend locker number
                changeTransaction.DateRelease = today;
                changeTransaction.Releaseby = currentUser;
                changeTransaction.Status = (int)StatusEnum.Release;

                changeTransaction.LockerNumber = friendTransaction.LockerNumber;
                changeTransaction.LockerId = friendTransaction.LockerId;

                changeTransaction.TransactionType = (int)LockerEnum.permanent;
                db.Entry(changeTransaction).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //add into employee locker
                EmployeeLocker req1 = new EmployeeLocker
                {
                    TransactionId = changeTransaction.TransactionId,
                    BadgeId = changeTransaction.BadgeId,
                    LockerNumber = changeTransaction.LockerNumber,
                    LockerID = changeTransaction.LockerId ?? 0
                };
                db.EmployeeLocker.Add(req1);
                db.SaveChanges();

                //add change transaction untuk temannya
                Transaction newTransaction = new Transaction();
                newTransaction.BadgeId = friendTransaction.BadgeId;
                newTransaction.Name = friendTransaction.Name;
                newTransaction.Department = friendTransaction.Department;
                newTransaction.Area = myTransaction.Area;
                newTransaction.Site = myTransaction.Site;
                newTransaction.TransactionType = myTransaction.TransactionType;

                newTransaction.LockerNumber = myCurrentLocker.LockerNumber;
                newTransaction.LockerId = myCurrentLocker.Id;

                newTransaction.DateRequest = today;
                newTransaction.DateRelease = today;
                newTransaction.Releaseby = currentUser;
                newTransaction.PIC = myTransaction.PIC;

                newTransaction.Status = (int)StatusEnum.Release;
                db.Transaction.Add(newTransaction);
                db.SaveChanges();

                //add into employee locker
                EmployeeLocker employeeLocker = new EmployeeLocker
                {
                    BadgeId = newTransaction.BadgeId,
                    TransactionId = newTransaction.TransactionId,
                    LockerNumber = newTransaction.LockerNumber,
                    LockerID = newTransaction.LockerId ?? 0
                };
                db.EmployeeLocker.Add(employeeLocker);
                db.SaveChanges();

                return "success";
            }
            return "failed";
        }

        [HttpPost]
        public string RejectRequest(int TransactionId)
        {
            var transaction = db.Transaction.FirstOrDefault(x => x.TransactionId == TransactionId);
            if (transaction != null)
            {
                EmployeeLocker employeeLocker = db.EmployeeLocker.FirstOrDefault(x => x.TransactionId == TransactionId);
                if (employeeLocker != null)
                {
                    db.EmployeeLocker.Remove(employeeLocker);
                    db.SaveChanges();
                }

                db.Transaction.Remove(transaction);
                var isSaved = db.SaveChanges();

                if (isSaved > 0)
                {
                    return "success";
                }
            }

            return "failed";
        }

        private ReleaseViewModel GetDetail(Transaction _db)
        {
            ReleaseViewModel model = new ReleaseViewModel();
            model.TransactionId = _db.TransactionId;
            model.BadgeId = _db.BadgeId;
            model.Name = _db.Name;
            model.Department = _db.Department;
            model.Area = _db.Area;
            model.AreaId = EnumHelpers.GetAreaId(_db.Area);

            model.Site = _db.Site;
            model.DateRequest = _db.DateRequest;
            model.LockerNumber = _db.LockerNumber;
            model.TransactionType = _db.TransactionType;
            //------

            var friend = GMDSHelpers.Find(model.FriendBadgeNumber);
            if (friend != null)
            {
                model.FriendBadgeNumber = friend.Badge_ID;
                model.FriendName = friend.Name;
                model.FriendDepartment = friend.Department;

                var isExist = db.EmployeeLocker
                    .FirstOrDefault(x => x.BadgeId == model.FriendBadgeNumber && x.Transaction.Status == (int)StatusEnum.Release);

                if (isExist != null)
                {
                    model.FriendArea = isExist.Transaction.Area;
                    model.FriendSite = isExist.Transaction.Site;
                    model.FriendDateRequest = isExist.Transaction.DateRequest;
                    model.FriendLockerNumber = isExist.Transaction.LockerNumber;
                }
            }

            return model;
        }


        private void UpdateLocker(int? lockerId, bool add)
        {
            var locker = db.Locker.FirstOrDefault(x => x.Id == lockerId);

            if (add)
            {
                //tambah stoknya
                locker.Stock = locker.Stock + 1;
            }
                else
            {
                //kurangi stoknya
                locker.Stock = locker.Stock - 1;
            }

            db.Entry(locker).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }



        [HttpGet]
        public ActionResult ChangeNewLocker(int transactionId)
        {
            var _db = db.Transaction.FirstOrDefault(x => x.TransactionId == transactionId);
            if (_db != null)
            {
                var model = GetDetail(_db);
                //model.ListOfLocker = GetLocker(model.TransactionType, model.AreaId, model.Site);
                return PartialView("DefaultReleaseModal", model);
            }

            return PartialView(null);
        }

        [HttpPost]
        public string ChangeNewLocker(int transactionId, int lockerId)
        {
            var transaction = db.Transaction.FirstOrDefault(x => x.TransactionId == transactionId);
            if (transaction != null)
            {
                //change locker

                var today = DateTime.UtcNow.ToBatamTime();

                var myTransaction = db.Transaction
                    .Where(x => x.BadgeId == transaction.BadgeId
                    && x.Status == (int)StatusEnum.Release
                    && x.TransactionType == (int)LockerEnum.permanent).FirstOrDefault();

                //set mytransaction to return / with remark change locker
                myTransaction.DateReturn = today;
                myTransaction.ReceivedBy = "Change locker";
                myTransaction.Status = (int)StatusEnum.Return;
                db.Entry(myTransaction).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //remove from employeelocker
                var todelete1 = db.EmployeeLocker.FirstOrDefault(x => x.TransactionId == myTransaction.TransactionId);

                //increase locker key stock
                UpdateLocker(todelete1.LockerID, true);


                db.EmployeeLocker.Remove(todelete1);
                db.SaveChanges();

                //set change transaction locker number to friend locker number

                var changeTransaction = transaction;

                changeTransaction.DateRelease = today;
                changeTransaction.Releaseby = UserHelpers.CurrentUser();
                changeTransaction.Status = (int)StatusEnum.Release;
                changeTransaction.TransactionType = (int)LockerEnum.permanent;

                var locker = db.Locker.FirstOrDefault(x => x.Id == lockerId);
                changeTransaction.LockerNumber = locker.LockerNumber;
                changeTransaction.LockerId = locker.Id;


                db.Entry(changeTransaction).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //add into employee locker
                EmployeeLocker req1 = new EmployeeLocker
                {
                    TransactionId = changeTransaction.TransactionId,
                    BadgeId = changeTransaction.BadgeId,
                    LockerNumber = changeTransaction.LockerNumber,
                    LockerID = changeTransaction.LockerId ?? 0
                };
                db.EmployeeLocker.Add(req1);
                db.SaveChanges();


                //decerease locker key stock
                UpdateLocker(changeTransaction.LockerId, false);

                return "success";
            }
            return "failed";
        }

        [HttpGet]
        public ActionResult ChangeWithFriend(int transactionId)
        {
            var _db = db.Transaction.FirstOrDefault(x => x.TransactionId == transactionId);
            if (_db != null)
            {
                var model = GetDetail(_db);
                //var myTransaction = db.Transaction
                //    .Where(x => x.BadgeId == _db.BadgeId
                //    && x.Status == (int)StatusEnum.Release
                //    && x.TransactionType == (int)LockerEnum.permanent).FirstOrDefault();


                model.MyTransactionId = _db.TransactionId;
                model.DateRequest = _db.DateRequest;
                model.LockerNumber = _db.LockerNumber;

                //Change with friend
                return PartialView("Admin_ChangeWithFriendModal", model);
            }

            return PartialView(null);
        }

        [HttpGet]
        public JsonResult GetLastLocker(string employeeId)
        {
            var friendTransaction = db.Transaction
                .Where(x => x.BadgeId == employeeId
                && x.Status == (int)StatusEnum.Release
                && x.TransactionType == (int)LockerEnum.permanent).FirstOrDefault();

            if (friendTransaction != null)
            {
                return Json( new {
                    id = friendTransaction.TransactionId,
                    name = friendTransaction.Name,
                    dept = friendTransaction.Department,
                    area = friendTransaction.Area,
                    Site = friendTransaction.Site,
                    daterequest = friendTransaction.DateRequest.OJTFormat(),
                    locker = friendTransaction.LockerNumber
                }, JsonRequestBehavior.AllowGet);

            } else
            {
                return Json(new { name = "Employee not found", dept = "Employee not found" }, JsonRequestBehavior.AllowGet);
            }

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