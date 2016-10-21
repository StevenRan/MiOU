using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities
{
    public enum OrderStatus
    {
        CREATED = 0,
        PRE_BOOKED = 1,
        BOOKED = 2,
        RENTING = 3,
        RETURNED = 4,
        COMPLETED = 5,
        CANCELED = 9
    }
    public enum PaymentStatus
    {
        CREATED = 0,
        SUCCEED = 1,
        FAILED = 2,
        CANCELED = 3
    }
}
