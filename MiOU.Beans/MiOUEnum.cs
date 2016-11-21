using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities
{
    public enum ApiCallStatus
    {
        OK,
        ERROR
    }
    public enum SearchOrderType
    {
        ALL,
        IN,
        OUT
    }
    public enum OrderExtendStatus
    {
        APPLIED=0,
        APPROVED=1,
        REJECTED=2
    }
    public enum OrderStatus
    {
        CREATED = 0,
        PRE_BOOKED = 1,
        BOOKED = 2,
        RENTING = 3,
        RETURN_APPLIED = 4,
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
