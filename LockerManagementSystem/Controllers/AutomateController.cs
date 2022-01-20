using LockerManagementSystem.App_Class;
using LockerManagementSystem.DAL;
using LockerManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LockerManagementSystem.Controllers
{
    public class AutomateController : Controller
    {
        private LockerManagementSystemEntities db = new LockerManagementSystemEntities();

        // GET: Automate
        public ActionResult Index()
        {
            return View();
        }

        //All request more than 1 day
        public ActionResult AllRequest()
        {
            var today = DateTime.UtcNow.ToBatamTime();

            var model = db.Transaction.ToList()
                .Where(x => x.Status == (int)StatusEnum.Request && x.DateRequest.AddDays(1).Date < today.Date)
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

            return View("AllRequest", model);
        }

        //all released temporary yg lebih dari target
        //normalnya 3 hari
        public ActionResult AllRelease()
        {
            var today = DateTime.UtcNow.ToBatamTime();

            var model = db.Transaction
                .Where(x => x.Status == (int)StatusEnum.Release && x.DateTemporaryReturn < today.Date)
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

            return View("AllRelease", model);
        }
    }
}