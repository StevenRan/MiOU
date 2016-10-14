using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BEvaluatedPrice:BObject
    {
        public float Price { get; set; }
        public BPriceCategory Catetegory { get; set; }
        public BUser CreatedBy { get; set; }
        public BUser UpdatedBy { get; set; }
    }
}
