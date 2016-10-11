using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiOU.DAL;
namespace MiOU.Entities.Beans
{
    public class BUser
    {
        public User User { get; set; }
        public Permissions Permission { get; set; }
        public bool IsWebMaster { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsAdmin { get; set; }
        public Area Province { get; set; }
        public Area City { get; set; }
    }
}
