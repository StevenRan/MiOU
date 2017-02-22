using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BAddress:BObject
    {
        public BArea Province { get; set; }
        public BArea City { get; set; }
        public BArea District { get; set; }
        public BUser User { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Apartment { get; set; }
        public string Address { get; set; }
        public string NearBy { get; set; }
        public bool IsDefault { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
    }
}
