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
    
    public partial class VipLevel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Start { get; set; }
        public float End { get; set; }
        public long Created { get; set; }
        public int CreatedBy { get; set; }
        public int Updated { get; set; }
        public int UpdatedBy { get; set; }
        public float CurrencyAmount { get; set; }
        public int Expired { get; set; }
        public float YajinPercentage { get; set; }
    }
}
