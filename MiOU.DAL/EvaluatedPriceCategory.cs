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
    
    public partial class EvaluatedPriceCategory
    {
        public int Id { get; set; }
        public float StartPrice { get; set; }
        public float EndPrice { get; set; }
        public long Created { get; set; }
        public int CreatedBy { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Updated { get; set; }
        public int UpdatedBy { get; set; }
        public int VIPRentLevel { get; set; }
    }
}
