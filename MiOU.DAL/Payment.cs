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
    
    public partial class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public int PayType { get; set; }
        public int PayCategory { get; set; }
        public bool Payed { get; set; }
        public long Created { get; set; }
        public long PayedTime { get; set; }
        public float Amount { get; set; }
        public int Status { get; set; }
        public long UpdatedTime { get; set; }
    }
}