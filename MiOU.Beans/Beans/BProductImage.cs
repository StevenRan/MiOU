using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BProductImage:BObject
    {
        public BProduct Product { get; set; }
        public BFile Image { get; set; }
        public bool IsMain { get; set; }
    }
}
