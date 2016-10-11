using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using MiOU.BL;
using MiOU.DAL;
using MiOU.Util;
using MiOU.Web.Models;
using WeChat.Adapter.Authorization;
using WeChat.Adapter;
using MiOU.Web.Helper;

namespace MiOU.Web.Controllers.v
{
    public class VHomeController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: VHome
        public async Task<ActionResult> Index()
        {
            string code = Request.QueryString["code"];
            if(string.IsNullOrEmpty(code))
            {
                RedirectToAction("Error", MessageConstants.WECHAT_AUTH_ERROR);
            }
            AccessToken weChatAccessToken = AuthHelper.GetAccessToken(PersistentValueManager.config, code);
            if(weChatAccessToken==null || string.IsNullOrEmpty(weChatAccessToken.Access_Token) || string.IsNullOrEmpty(weChatAccessToken.OpenId))
            {
                RedirectToAction("Error", MessageConstants.WECHAT_AUTH_ERROR);
            }
            
            UserLoginInfo login = new UserLoginInfo("WeChat", weChatAccessToken.OpenId);
            var user = await _userManager.FindAsync(login);
            if (user == null)
            {
                WeChatUserInfo userInfo = AuthHelper.GetUserInfo(PersistentValueManager.config, weChatAccessToken);
                if(userInfo==null)
                {
                    RedirectToAction("Error", MessageConstants.WECHAT_AUTH_ERROR);
                }
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = ""});
            }

            return View();            
        }

        public ActionResult Error(string message)
        {
            return View();
        }
    }
}