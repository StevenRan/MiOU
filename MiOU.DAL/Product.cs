//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public long Created { get; set; }
        public long Updated { get; set; }
        public float Percentage { get; set; }
        public float Pledge { get; set; }
        public float Price { get; set; }
        public int Status { get; set; }
        public int AuditUserId { get; set; }
        public long AuditTime { get; set; }
        public string AuditMessage { get; set; }
        public int EvaluatedPriceCategoryId { get; set; }
        public float EvaluatedPrice { get; set; }
        public float EvaluatedPercentage { get; set; }
        public int DeliveryType { get; set; }
        public int RentType { get; set; }
        public int Province { get; set; }
        public int City { get; set; }
        public int District { get; set; }
        public string Apartment { get; set; }
        public string NearBy { get; set; }
        public string Address { get; set; }
        public string XPlot { get; set; }
        public string YPlot { get; set; }
        public int Type { get; set; }
        public int Repertory { get; set; }
        public bool Locked { get; set; }
        public int VIPLevel { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public int RentTimes { get; set; }
        public int ManageType { get; set; }
    }
}
