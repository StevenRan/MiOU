using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiOU.DAL;
namespace MiOU.Entities.Beans
{
    public enum UserStatus
    {
        ACTIVE=0,
        DISABLED=1
    }
    public class BUser
    {
        public User User { get; set; }
        public Permissions Permission { get; set; }
        public bool IsWebMaster { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsAdmin { get; set; }
        public BArea Province { get; set; }
        public BArea City { get; set; }
        public BArea District { get; set; }
        public BUserType UserType { get; set; }
        public string Gendar { get; set; }
        public BVIPLevel VIPLevel { get; set; }
    }
}
