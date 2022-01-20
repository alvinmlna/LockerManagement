using LockerManagementSystem.App_Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LockerManagementSystem.ViewModels
{
    public class ReturnViewModel
    {
        public int TransactionId { get; set; }
        public string BadgeId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }

        public string Designation { get; set; }
        public string EmployeeGender { get; set; }

        public string Area { get; set; }
        public string Site { get; set; }
        public int LockerId { get; set; }
        public string LockerNunmber { get; set; }
        public string KeyNumber { get; set; }

        public int? stock { get; set; }

        public string LockerType { get; set; }
        public string Remark { get; set; }

        public DateTime? DateTemporaryReturn { get; set; }
        public string DateTemporaryReturnView
        {
            get
            {
                if (DateTemporaryReturn != null)
                {
                    return DateTemporaryReturn.Value.OJTFormat();
                }
                else
                {
                    return "";
                }
            }
        }

        public bool IsExpired
        {
            get
            {
                if (DateTemporaryReturn != null)
                {
                    if (DateTemporaryReturn.Value < DateTime.UtcNow.ToBatamTime())
                    {
                        return true;
                    }
                        else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public string Status
        {
            get
            {
                if (IsExpired)
                {
                    return "Expired";
                } else
                {
                    return "";
                }
            }
        }

        public DateTime? DateRelease { get; set; }
        public string DateReleaseView {
            get {
                if (DateRelease != null)
                {
                    return DateRelease.Value.OJTFormat();
                } else
                {
                    return "";
                }
        } }

        public string PIC { get; set; }
    }
}