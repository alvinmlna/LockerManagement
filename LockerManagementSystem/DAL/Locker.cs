//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LockerManagementSystem.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Locker
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Locker()
        {
            this.Transaction = new HashSet<Transaction>();
            this.EmployeeLocker = new HashSet<EmployeeLocker>();
        }
    
        public string LockerNumber { get; set; }
        public string LockerKeyNumber { get; set; }
        public int LockerType { get; set; }
        public int Area { get; set; }
        public string Site { get; set; }
        public Nullable<int> Stock { get; set; }
        public bool IsActive { get; set; }
        public int Id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transaction> Transaction { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeLocker> EmployeeLocker { get; set; }
    }
}