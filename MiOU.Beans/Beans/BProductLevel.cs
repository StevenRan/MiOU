using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BProductLevel:BObject
    {
        public float StartPrice { get; set; }
        public float EndPrice { get; set; }
        public string Description { get; set; }
        public BUser CreatedBy { get; set; }
        public BUser UpdatedBy { get; set; }
        public List<BVIPLevel> RentableVips { get; set; }
        public string RentableVipLevels { get; set; }
    }
}
