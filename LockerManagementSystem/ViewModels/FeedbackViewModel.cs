using LockerManagementSystem.DAL;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LockerManagementSystem.ViewModels
{
    public class FeedbackViewModel
    {
        public int Id { get; set; }

        [Required]
        public string BadgeId { get; set; }

        public string Name { get; set; }

        [Required]
        public string Locker { get; set; }

        [Required]
        public string Remark { get; set; }

        public string Superior { get; set; }

        [Required]
        public string SuperiorEmail { get; set; }
        public string Status { get; set; }
        public System.DateTime DateSubmit { get; set; }
        public string Category { get; set; }
        public string Department { get; set; }
    }

    public class FeedbackListViewModel
    {
        public IPagedList<Feedback> OPEN { get; set; }
        public  IPagedList<Feedback> CLOSE { get; set; }
    }
}