using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BMaintenanceType:BObject
    {
        public string Description { get; set; }
        public BUser CreatedBy { get; set; }
        public BUser UpdatedBy { get; set; }
    }
}
