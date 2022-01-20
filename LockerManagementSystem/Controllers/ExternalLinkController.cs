using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LockerManagementSystem.Controllers
{
    public class ExternalLinkController : Controller
    {
        // GET: ExternalLink
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult OE()
        {
            return Redirect("http://bthsa1160.infineon.com:200/newattiresurvey/surveyChecker");
        }
    }
}