using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiOU.BL;
using MiOU.Entities.Beans;

namespace MiOU
{
    class Program
    {
        static void Main(string[] args)
        {
            PermissionManagement perMger = new PermissionManagement(null);
            perMger.SyncPermissionsWithDB();
        }
    }
}
