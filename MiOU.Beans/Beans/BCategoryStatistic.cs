using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BCategoryStatistic
    {
        public BCategory Category { get; set; }
        public int SupplierCount { get; set; }
        public int ProductCount { get; set; }
        public int FinishedOrderCount { get; set; }
    }
}
