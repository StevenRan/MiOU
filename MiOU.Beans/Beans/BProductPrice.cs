using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BProductPrice
    {
        public BProduct Product { get; set; }
        public BCategory Category { get; set; }
        public BPriceCategory PriceCategory { get; set; }
        public BEvaluatedPrice EPrice { get; set; }
        public float Price { get; set; }
        public BUser CreatedBy { get; set; }
    }
}
