using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BCategory:BObject
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public BCategory Parent { get; set; }
        public BCategory ChindRen { get; set; }
    }
}
