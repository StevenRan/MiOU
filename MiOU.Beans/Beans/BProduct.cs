using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public enum ProductStatus
    {
        CREATED=0,
        PASSED=1,
        FAILED=2
    }
    public class BProduct:BObject
    {
        public string Description { get; set; }
        public BCategory Category { get; set; }
        public BCategory PCategory { get; set; }
        public BUser User { get; set; }
        public float Percentage { get; set; }
        public float Pledge { get; set; }
        public float Price { get; set; }        
        public ProductStatus Status { get; set; }
        public BUser AuditUser { get; set; }
        public long AuditTime { get; set; }
        public string AuditMessage { get; set; }
        public BObject DeliveryType { get; set; }
        public BObject RentType { get; set; }
        public BArea Province { get; set; }
        public BArea City { get; set; }
        public BArea District { get; set; }
        public string Address { get; set; }
        public string Apartment { get; set; }
        public string Nearby { get; set; }
        public string XPlot { get; set; }
        public string YPlot { get; set; }
        public List<BProductImage> Images { get; set; }
        public List<BProductPrice> ProductPrices { get; set; }
        public int Repertory { get; set; }
        public BVIPLevel VIPRentLevel { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public BProductLevel ProductLevel { get; set; }
    }
}
