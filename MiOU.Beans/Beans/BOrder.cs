using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiOU.Entities;
namespace MiOU.Entities.Beans
{
   
    public class BOrder:BObject
    {
        public string OrderNo { get; set; }
        public BProduct Product { get; set; }
        public BPriceCategory PriceCategory { get; set; }
        public BEvaluatedPrice EPrice { get; set; }
        public OrderStatus Status { get; set; }
        public BUser CreatedBy { get; set; }
        public BUser UpdatedBy { get; set; }
        public string Description{get;set;}
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public bool Extended { get; set; }
        public long ExtendTime { get; set; }
        public long ExtendEndTime { get; set; }
        public OrderExtendStatus ExtenedStatus { get; set; }
        public List<BPayment> Payments { get; set; }
        public BPayType PayType { get; set; }
    }
}
