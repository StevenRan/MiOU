using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Beans
{
    public class BFile:BObject
    {
        public string Path { get; set; }
        public string Ext { get; set; }
        public int UserId { get; set; }
    }
}
