using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BUserAvator:BObject
    {
        public BFile Image { get; set; }
        public BUser UpdatedBy { get; set; }
        public BUser Owner { get; set; }
        public bool Enabled { get; set; }

    }
}
