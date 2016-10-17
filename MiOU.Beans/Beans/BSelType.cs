using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BSelType:BObject
    {
        public BUser CreatedBy { get; set; }
        public BUser UpdatedBy { get; set; }
        public string Description { get; set; }
    }
}
