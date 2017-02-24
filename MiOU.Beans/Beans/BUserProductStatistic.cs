using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BUserProductStatistic
    {
        public int UserId { get; set; }
        public BCategory Category { get; set; }
        public int Amount { get; set; }
    }
}
