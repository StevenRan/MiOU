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
    
    public partial class EvaluatedPrice
    {
        public int Id { get; set; }
        public int EvaluatedPriceCategory { get; set; }
        public int PriceCategory { get; set; }
        public float Price { get; set; }
        public long Created { get; set; }
        public int UserId { get; set; }
        public long Updated { get; set; }
        public int UpdatedUserId { get; set; }
    }
}
