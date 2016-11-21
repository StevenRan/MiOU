using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BCategory:BObject
    {
        public int Order { get; set; }
        public BCategory Parent { get; set; }
        public List<BCategory> Chindren { get; set; }
    }
}
