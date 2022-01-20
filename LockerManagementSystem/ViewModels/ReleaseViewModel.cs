using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LockerManagementSystem.ViewModels
{
    public class ReleaseViewModel
    {
        public int TransactionId { get; set; }

        public string BadgeId { get; set; }

        public string Name { get; set; }

        public string Department { get; set; }

        public string Area { get; set; }
        public int AreaId { get; set; }


        public string Site { get; set; }

        public int TransactionType { get; set; }

        public int IsChangeWithFriend { get; set; }

        public string FriendBadgeNumber { get; set; }
        public string FriendName { get; set; }
        public string FriendDepartment { get; set; }
        public string FriendArea { get; set; }
        public string FriendSite { get; set; }
        public DateTime FriendDateRequest { get; set; }
        public string FriendLockerNumber { get; set; }
        public string OptionalLockerNumber { get; set; }

        public string LockerNumber { get; set; }
        public List<SelectListItem> ListOfLocker { get; set; }

        public DateTime DateRequest { get; set; }

        //for change

        public int MyTransactionId { get; set; }
        public int FriendTransactionId { get; set; }
    }
}