using LockerManagementSystem.App_Class.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LockerManagementSystem.ViewModels
{
    public class LockerViewModel
    {
        public int Id { get; set; }
        public string LockerNumber { get; set; }
        public string LockerKeyNumber { get; set; }
        public int LockerType { get; set; }

        public string LockerTypeView
        {
            get
            {
                if (LockerType == 1)
                {
                    return "Permanent";
                } else if (LockerType == 2)
                {
                    return "Temporary";
                } else
                {
                    return "error";
                }
            }
        }

        public int Area { get; set; }
        public string AreaView
        {
            get
            {
                return EnumHelpers.GetAreaName(Area);
            }
        }

        public string Site { get; set; }
        public Nullable<int> Stock { get; set; }
        public bool IsActive { get; set; }

        public string ayam { get; set; }
    }
}