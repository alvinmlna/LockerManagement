using LockerManagementSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LockerManagementSystem.App_Class.Helpers
{
    public class UserHelpers
    {
        

        public static string CurrentUser()
        {
            return HttpContext.Current.User.Identity.Name;
        }

        public static DAL.Admin GetLoggedUser()
        {
            LockerManagementSystemEntities db = new LockerManagementSystemEntities();
            var currUser = HttpContext.Current.User.Identity.Name;
            return db.Admin.FirstOrDefault(x => x.Username.ToLower() == currUser.ToLower());
        }
    }
}