using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LockerManagementSystem.App_Start
{
    /*
    public class Code_Junk
    {
        //check apakah sudah pernah request loker permanent sebelumnya
        private Locker CheckPermanentLocker(string badgeId)
        {
            var isSpecialUser = db.SpecialEmployee.FirstOrDefault(x => x.BadgeId == badgeId);
            if (isSpecialUser == null)
            {
                var isExist = db.EmployeeLocker.Where(x => x.BadgeId == badgeId && x.Locker.LockerType == (int)LockerEnum.permanent);
                if (isExist.Count() > 0)
                {
                    return isExist.FirstOrDefault().Locker;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                //kalau special user gas kan aja lah mau request
                return null;
            }
        }

        //check apakah sudah pernah request loker permanent sebelumnya
        private Locker CheckTemporaryLocker(string badgeId)
        {
            var isSpecialUser = db.SpecialEmployee.FirstOrDefault(x => x.BadgeId == badgeId);
            if (isSpecialUser == null)
            {
                var isExist = db.EmployeeLocker.Where(x => x.BadgeId == badgeId && x.Locker.LockerType == (int)LockerEnum.temporary);
                if (isExist.Count() > 0)
                {
                    return isExist.FirstOrDefault().Locker;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }
    }

    */
}