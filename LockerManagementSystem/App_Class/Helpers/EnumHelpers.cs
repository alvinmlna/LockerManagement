using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LockerManagementSystem.App_Class.Helpers
{
    public static class EnumHelpers
    {
        public static string TransactionType(int transactionType)
        {
            switch (transactionType)
            {
                case 1:
                    return "Permanent locker";
                case 2:
                    return "Temporary";
                case 3:
                    return "Change locker";
                default:
                    return "Invalid";
            }
        }

        public static int GetAreaId(string area)
        {
            switch (area)
            {  
                case "FOL":
                    return 1;
                case "EOL":
                    return 2;
                case "TESTING":
                    return 3;
                default:
                    return 0;
            }
        }
        public static string GetAreaName(int area)
        {
            switch (area)
            {
                case 1:
                    return "FOL";
                case 2:
                    return "EOL";
                case 3:
                    return "TESTING";
                default:
                    return "";
            }
        }
    }
}