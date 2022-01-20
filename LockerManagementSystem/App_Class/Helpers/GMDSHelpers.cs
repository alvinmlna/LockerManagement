using LockerManagementSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LockerManagementSystem.App_Class.Helpers
{
    public class GMDSHelpers
    {
        public static List<tbl_GMDS> GetAllEmployee()
        {
            using (var db = new ems_dbEntities())
            {
                return db.tbl_GMDS.ToList();
            }
        }

        public static tbl_GMDS Find(string badgeId)
        {
            using (var db = new ems_dbEntities())
            {
                return db.tbl_GMDS.FirstOrDefault(x => x.Badge_ID == badgeId);
            }
        }
    }
}