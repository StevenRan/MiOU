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
    
    public partial class AddressBook
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public int Province { get; set; }
        public int City { get; set; }
        public int District { get; set; }
        public string Apartment { get; set; }
        public string NearBy { get; set; }
        public string Address { get; set; }
        public long Created { get; set; }
        public long Updated { get; set; }
        public bool Default { get; set; }
    }
}
