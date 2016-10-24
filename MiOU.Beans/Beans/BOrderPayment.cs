using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BPayment:BObject
    {
        public BOrder Order { get; set; }
        public BPayType PayType { get; set; }
        public BPayCategory PayCategory { get; set; }
        public bool Payed { get; set; }
        public long PayedTime { get; set; }
        public PaymentStatus Status { get; set; }
        public float Amount { get; set; }
    }
}
