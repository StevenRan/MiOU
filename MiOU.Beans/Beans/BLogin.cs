using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BLogin:BObject
    {
        public string IP { get; set; }
        public string Description { get; set; }
        public BUser User { get; set; }
    }
}
