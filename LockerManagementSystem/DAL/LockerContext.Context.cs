﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class LockerManagementSystemEntities : DbContext
    {
        public LockerManagementSystemEntities()
            : base("name=LockerManagementSystemEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<EmployeeLocker> EmployeeLocker { get; set; }
        public virtual DbSet<Feedback> Feedback { get; set; }
        public virtual DbSet<Locker> Locker { get; set; }
        public virtual DbSet<SpecialEmployee> SpecialEmployee { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
    }
}