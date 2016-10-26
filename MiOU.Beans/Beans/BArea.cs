using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BArea:BObject
    {
        public List<BArea> Chindren { get; set; }
        public BArea Parent { get; set; }
        public int Level { get; set; }
        public int UPID { get; set; }
    }
}
