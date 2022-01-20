using LockerManagementSystem.App_Class;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LockerManagementSystem.ViewModels
{
    public class RequestViewModel
    {
        public int TransactionId { get; set; }

        public string errorMessage { get; set; }

        [Required]
        public string BadgeId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Department { get; set; }

        public string PIC { get; set; }


        public string Area { get; set; }
        public AreaEnum AreaEnum { get; set; }
        public string Site { get; set; }

        [Required]
        public int TransactionType { get; set; }

        public int IsChangeWithFriend { get; set; }

        public string FriendBadgeNumber { get; set; }
        public string FriendName { get; set; }
        public string FriendDepartment { get; set; }

        public string LockerNumber { get; set; }
        public int LockerId { get; set; }

        public DateTime DateRequest { get; set; }
        public DateTime? DateRelease { get; set; }
        public string ReleaseBy { get; set; }

        public DateTime? DateReturn { get; set; }
        public string ReturnBy { get; set; }
    }
}