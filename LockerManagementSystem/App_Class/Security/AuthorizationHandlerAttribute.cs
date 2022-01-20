using LockerManagementSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LockerManagementSystem.App_Class.Security
{
    public class AuthorizationHandlerAttribute : AuthorizeAttribute
    {
        private LockerManagementSystemEntities db = new LockerManagementSystemEntities();

        public AuthorizationHandlerAttribute()
        {
        }

        public string dummyUser = "";

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var routeData = httpContext.Request.RequestContext.RouteData;
            bool authorized = false;

            var username = "";
            if (String.IsNullOrEmpty(dummyUser))
            {
                username = HttpContext.Current.User.Identity.Name.ToLower();
            }
            else
            {
                username = dummyUser.ToLower();
            }

            //1 = admin, 2 = attire team

            var haveAcess = db.Admin.FirstOrDefault(x => x.Username.ToLower() == username);
            if (haveAcess != null)
            {
                var roleList = Roles.Split(',').ToList();
                var myaccess = haveAcess.Access.ToString();

                if (roleList.Contains(myaccess))
                {
                    authorized = true;
                }
            }


            return authorized;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new ViewResult { ViewName = "AccessDenied" };
        }
    }
}