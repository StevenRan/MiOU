﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MiOU.DAL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MiOUEntities : DbContext
    {
        public MiOUEntities()
            : base("name=MiOUEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Admin_Actions> Admin_Actions { get; set; }
        public DbSet<Admin_Categories> Admin_Categories { get; set; }
        public DbSet<Admin_Users> Admin_Users { get; set; }
        public DbSet<Admin_Users_Actions> Admin_Users_Actions { get; set; }
        public DbSet<Area> Area { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<EvaluatedPriceCategory> EvaluatedPriceCategory { get; set; }
        public DbSet<File> File { get; set; }
        public DbSet<PayCategory> PayCategory { get; set; }
        public DbSet<PayType> PayType { get; set; }
        public DbSet<PriceCategory> PriceCategory { get; set; }
        public DbSet<ProductImage> ProductImage { get; set; }
        public DbSet<ProductMaintenance> ProductMaintenance { get; set; }
        public DbSet<RentType> RentType { get; set; }
        public DbSet<UserType> UserType { get; set; }
        public DbSet<UserVip> UserVip { get; set; }
        public DbSet<EvaluatedPrice> EvaluatedPrice { get; set; }
        public DbSet<ProductPrice> ProductPrice { get; set; }
        public DbSet<UseCurrencyHistory> UseCurrencyHistory { get; set; }
        public DbSet<DeliveryType> DeliveryType { get; set; }
        public DbSet<MaintenanceType> MaintenanceType { get; set; }
        public DbSet<CurrencyTransferCategory> CurrencyTransferCategory { get; set; }
        public DbSet<VipLevel> VipLevel { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<User> User { get; set; }
    }
}
