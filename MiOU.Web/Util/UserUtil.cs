using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MiOU.BL;

namespace MiOU.Web.Util
{
    public class UserUtil
    {
        public static bool IsAdmin(int userId)
        {
            BaseManager baseMgr = new BaseManager(userId);
            return baseMgr.CurrentLoginUser.IsAdmin;
        }
        public static bool IsAdmin(string email)
        {
            BaseManager baseMgr = new BaseManager(email);
            return baseMgr.CurrentLoginUser.IsAdmin;

        }
    }
}