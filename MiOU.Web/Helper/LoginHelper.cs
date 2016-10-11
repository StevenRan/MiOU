using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using log4net;
using MiOU.BL;
using WeChat.Adapter;
using WeChat.Adapter.Authorization;
using System.Threading.Tasks;
using MiOU.Web.Models;
namespace MiOU.Web.Helper
{
    public enum ExternalLoginResult
    {
        FIRST_LOGIN,
        NO_CODE,
        LOGIN_SUCCESS
    }

    public class LoginHelper
    {
        static ILog logger= log4net.LogManager.GetLogger("LoginHelper");
    }
}