using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BAvator:BObject
    {
        public bool Enabled { get; set; }
        public BUser UpdatedBy { get; set; }
        public BUser Owner { get; set; }
        public BFile Image { get; set; }
    }
}
