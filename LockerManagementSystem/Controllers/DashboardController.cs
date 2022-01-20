using LockerManagementSystem.App_Class;
using LockerManagementSystem.App_Class.Helpers;
using LockerManagementSystem.DAL;
using LockerManagementSystem.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace LockerManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        private LockerManagementSystemEntities db = new LockerManagementSystemEntities();

        // GET: Dashboard
        public ActionResult Index(string area)
        {
            ViewBag.area = area;
            return View();
        }

        public ActionResult Permanent(string area)
        {
            ViewBag.area = area;
            return View();
        }

        [HttpPost]
        public ActionResult LoadPermanent(string area)
        {
            var areaid = (area == "FOL") ? 1 : (area == "EOL") ? 2 : 3;
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



                var customerData =
                      db.EmployeeLocker
                      .Where(x => x.Locker.LockerType == (int)LockerEnum.permanent
                      && x.Transaction.Status == (int)StatusEnum.Release
                      && x.Transaction.Area == area);

                var allusedlocker = customerData.Select(x => x.LockerID);

                var availableLocker = db.Locker.Where(x => !allusedlocker.Contains(x.Id) && x.Area == areaid);





                // Getting all Customer data    
                var customerDataView =
                      customerData
                      .Select(x => new ReturnViewModel
                      {
                          TransactionId = x.TransactionId,
                          BadgeId = x.BadgeId,
                          Name = x.Transaction.Name,
                          Department = x.Transaction.Department,
                          EmployeeGender = x.Transaction.EmployeeGender,
                          PIC = x.Transaction.PIC,
                          Designation = x.Transaction.Designation,
                          Area = x.Transaction.Area,
                          Site = x.Transaction.Site,
                          LockerNunmber = x.Locker.LockerNumber,
                          KeyNumber = x.Locker.LockerKeyNumber,
                          stock = x.Locker.Stock,
                          LockerType = (x.Locker.LockerType == 1) ? "Permanent" : "Temporary",
                          DateRelease = x.Transaction.DateRelease
                      });


                var availableLockerView = availableLocker
                    .Select(x => new ReturnViewModel
                    {
                        TransactionId = 0,
                        BadgeId = (x.LockerType != 1) ? "Temporary Locker" : "",
                        Name = "",
                        Department = "",
                        EmployeeGender = "",
                        PIC = "",
                        Designation = "",
                        Area = area,
                        Site = x.Site,
                        LockerNunmber = x.LockerNumber,
                        KeyNumber = x.LockerKeyNumber,
                        stock = x.Stock,
                        LockerType = (x.LockerType == 1) ? "Permanent" : "Temporary",
                        DateRelease = null
                    });

                customerDataView = customerDataView.Concat(availableLockerView);

                //Sorting    
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    customerDataView = customerDataView.OrderBy(sortColumn + " " + sortColumnDir);
                }
                else
                {
                    customerDataView = customerDataView.OrderByDescending(x => x.DateRelease);
                }

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerDataView = customerDataView.Where(m =>
                    m.BadgeId.Contains(searchValue)
                    || m.Name.Contains(searchValue) || m.LockerNunmber.Contains(searchValue));
                }

                //total number of rows count     
                recordsTotal = customerDataView.Count();
                //Paging     
                var data = customerDataView.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult LoadTemporary(string area)
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
                IEnumerable<ReturnViewModel> customerData =
                      db.EmployeeLocker
                      .Where(x => x.Locker.LockerType == (int)LockerEnum.temporary 
                      && x.Transaction.Status == (int)StatusEnum.Release
                      && x.Transaction.Area == area)
                      .Select(x => new ReturnViewModel
                      {
                          TransactionId = x.TransactionId,
                          BadgeId = x.BadgeId,
                          Name = x.Transaction.Name,
                          Department = x.Transaction.Department,
                          EmployeeGender = x.Transaction.EmployeeGender,
                          Designation = x.Transaction.Designation,
                          PIC = x.Transaction.PIC,
                          Area = x.Transaction.Area,
                          Site = x.Transaction.Site,
                          LockerNunmber = x.Locker.LockerNumber,
                          KeyNumber = x.Locker.LockerKeyNumber,
                          stock = x.Locker.Stock,
                          LockerType = (x.Locker.LockerType == 1) ? "Permanent" : "Temporary",
                          DateTemporaryReturn = x.Transaction.DateTemporaryReturn,
                          DateRelease = x.Transaction.DateRelease,
                      }).ToList();

                //Sorting    
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDir);
                }
                else
                {
                    customerData = customerData.OrderByDescending(x => x.Area).ThenByDescending(x => x.DateRelease);
                }

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => 
                    m.BadgeId.Contains(searchValue)
                    || m.Name.Contains(searchValue)
                    || m.LockerNunmber.Contains(searchValue)
                    || m.Status == "Expired");
                }


                //total number of rows count     
                recordsTotal = customerData.Count();
                //Paging     
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        public void ExportDetail(string area)
        {
            var areaid = (area == "FOL") ? 1 : (area == "EOL") ? 2 : 3;

            var customerData =
                  db.EmployeeLocker
                  .Where(x => x.Locker.LockerType == (int)LockerEnum.permanent
                  && x.Transaction.Status == (int)StatusEnum.Release
                  && x.Transaction.Area == area)
                  .Select(x => new ReturnViewModel
                  {
                      TransactionId = x.TransactionId,
                      BadgeId = x.BadgeId,
                      Name = x.Transaction.Name,
                      Department = x.Transaction.Department,
                      EmployeeGender = x.Transaction.EmployeeGender,
                      PIC = x.Transaction.PIC,
                      Designation = x.Transaction.Designation,
                      Area = x.Transaction.Area,
                      Site = x.Transaction.Site,
                      LockerNunmber = x.Locker.LockerNumber,
                      LockerId = x.Locker.Id,
                      KeyNumber = x.Locker.LockerKeyNumber,
                      stock = x.Locker.Stock,
                      LockerType = (x.Locker.LockerType == 1) ? "Permanent" : "Temporary",
                      DateRelease = x.Transaction.DateRelease
                  }).ToList();

            var allusedlocker = customerData.Select(x => x.LockerId);

            var availableLocker = db.Locker.Where(x =>  !allusedlocker.Contains(x.Id) && x.Area == areaid)
                .Select(x => new ReturnViewModel
                {
                    TransactionId = 0,
                    BadgeId = "",
                    Name = "",
                    Department = "",
                    EmployeeGender = "",
                    Designation = "",
                    Area =  area,
                    Site = x.Site,
                    LockerNunmber = x.LockerNumber,
                    LockerId = x.Id,
                    KeyNumber = x.LockerKeyNumber,
                    stock = x.Stock,
                    LockerType = (x.LockerType == 1) ? "Permanent" : "Temporary",
                }).ToList();

            customerData.AddRange(availableLocker);

            var controller = new ExportToExcelHelper();
            controller.ExportToExcel(customerData, filename: "Report");
        }
    }
}