using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BProductMaintenance:BObject
    {
        public BProduct Product { get; set; }
        public string Description { get; set; }
        public BMaintenanceType MaintenanceType { get; set; }
    }
}
