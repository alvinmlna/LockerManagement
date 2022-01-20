using LockerManagementSystem.App_Class;
using LockerManagementSystem.App_Class.Helpers;
using LockerManagementSystem.App_Class.Security;
using LockerManagementSystem.DAL;
using LockerManagementSystem.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace LockerManagementSystem.Controllers
{
    public class FeedbackController : Controller
    {
        LockerManagementSystemEntities db = new LockerManagementSystemEntities();
        // GET: Feedback
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(FeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                Feedback feedback = new Feedback
                {
                    BadgeId = model.BadgeId,
                    DateSubmit = DateTime.UtcNow.ToBatamTime(),
                    Department = model.Department,
                    Locker = model.Locker,
                    Name = model.Name,
                    Remark = model.Remark,
                    Category = "",
                    Status = "Open",
                    Superior = model.Superior,
                    SuperiorEmail = model.SuperiorEmail
                };

                db.Feedback.Add(feedback);
                var isSaved = db.SaveChanges();

                if (isSaved > 0)
                {
                    ViewBag.complete = "Success";

                    SendMail(feedback);
                }
                ModelState.Clear();
                return View("index");
            }
            return View("Index", model);
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
                return Json(new { name = "Employee not found", dept = "Employee not found" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult UpdateLocker(string badgeId)
        {
            // Getting all Customer data    
            var model = db.EmployeeLocker.Where(x => x.BadgeId == badgeId).ToList();

            var result = model.Select(x => new SelectListItem
            {
                Value = x.LockerNumber,
                Text = x.LockerNumber
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutocompleteEmail(string Prefix)
        {
            IEnumerable<tbl_GMDS> modelList = null;

            var fromDB = GMDSHelpers.GetAllEmployee()
                .Where(x => x.Name.Contains(Prefix, StringComparison.OrdinalIgnoreCase))
                .Take(10);

            if (fromDB != null)
            {
                modelList = fromDB;
            }

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }


        [AuthorizationHandlerAttribute(Roles = "1,2")]
        public ActionResult FeedbackList(string searchString, string currentFilter, int? page)
        {
            var model = db.Feedback.Where(x => x.Status == "Open")
                .OrderByDescending(x => x.DateSubmit).ToList();

            var currentUser = UserHelpers.GetLoggedUser();
            if (currentUser != null && currentUser.Access == (int)AccessEnum.AttireTeam)
            {
                var allTemporaryLocker = db.Locker.Where(x => x.LockerType == (int)LockerEnum.temporary).Select(x => x.LockerNumber).ToList();
                model = model.Where(x => allTemporaryLocker.Contains(x.Locker)).ToList(); //untuk attireteam hanya untuk temporary locker
            }

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                model = model.Where(s => s.BadgeId.Contains(searchString) || s.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            int pageSize = 30;
            int pageNumber = (page ?? 1);
            return View(model.ToPagedList(pageNumber, pageSize));
        }

        [AuthorizationHandlerAttribute(Roles = "1,2")]
        public ActionResult FeedbackListCLOSE(string searchString, string currentFilter, int? page)
        {
            var model = db.Feedback.Where(x => x.Status != "Open")
                .OrderByDescending(x => x.DateSubmit).ToList();

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                model = model.Where(s => s.BadgeId.Contains(searchString) || s.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            int pageSize = 30;
            int pageNumber = (page ?? 1);
            return View(model.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult Close(int feedbackId)
        {
            var tobeUpdate = db.Feedback.FirstOrDefault(x => x.Id == feedbackId);
            tobeUpdate.Status = "Close";
            db.Entry(tobeUpdate).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            var locker = db.Locker.FirstOrDefault(x => x.LockerNumber == tobeUpdate.Locker);
            if (locker != null)
            {
                locker.Stock = locker.Stock - 1;
                db.Entry(locker).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            //mencari transaksi terakhir
            var transactionId = db.Transaction.Where(x => x.LockerNumber == tobeUpdate.Locker && x.BadgeId == tobeUpdate.BadgeId && x.Status == (int)StatusEnum.Release).FirstOrDefault();
            if (transactionId != null)
            {
                ReturnController returnController = new ReturnController();
                returnController.ReturnRequest(tobeUpdate.Remark, transactionId.TransactionId);
            }


            return RedirectToAction("FeedbackList");
        }

        public ActionResult Delete(int feedbackId, string status)
        {
            var tobeDeleted = db.Feedback.FirstOrDefault(x => x.Id == feedbackId);
            db.Feedback.Remove(tobeDeleted);
            db.SaveChanges();

            if (status == "Open")
            {
                return RedirectToAction("FeedbackList");
            }
            else
            {
                return RedirectToAction("FeedbackListCLOSE");
            }
        }



        public void SendMail(Feedback feedback)
        {
            string defaultFromMail = "oe.automail@infineon.com";
            var emailTemplate = System.IO.File.ReadAllText(Server.MapPath("/Content/EmailTemplate.txt"));
            if (emailTemplate != null)
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(defaultFromMail);

                //mail.To.Add(feedback.SuperiorEmail);
                mail.To.Add("MuhammadAlvin.Maulana@infineon.com");
                mail.Bcc.Add("MuhammadAlvin.Maulana@infineon.com");


                mail.Subject = "Attire System - Locker Key " + feedback.Remark + " by " + feedback.Name;

                var lockerDetail = db.Locker.FirstOrDefault(x => x.LockerNumber == feedback.Locker);

                var bodyContent = emailTemplate;
                bodyContent = bodyContent.Replace("[%name%]",  feedback.Name);
                bodyContent = bodyContent.Replace("[%lockernumber%]",  feedback.Locker);
                bodyContent = bodyContent.Replace("[%area%]", EnumHelpers.GetAreaName(lockerDetail.Area));
                bodyContent = bodyContent.Replace("[%site%]",  lockerDetail.Site);
                bodyContent = bodyContent.Replace("[%reason%]",  feedback.Remark);

                mail.Body = bodyContent;
                mail.IsBodyHtml = true;

                //string SMTP = "smtp.intra.infineon.com";
                string SMTP = "mailrelay-internal.infineon.com";
                try
                {
                    SmtpClient SmtpServer = new SmtpClient(SMTP);
                    SmtpServer.Credentials = new NetworkCredential("OJT TEAM", "1234");
                    SmtpServer.Send(mail);
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}