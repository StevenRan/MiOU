using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using MiOU.Entities.Beans;
using MiOU.Entities.Exceptions;
using MiOU.Entities;
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
        public VHomeController()
        {
        }
        public VHomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
       
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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "VHome");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model)
        {           

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                int externalUserType = 0;
                switch (info.Login.LoginProvider)
                {
                    case "WeChat":
                        externalUserType = 1;
                        break;
                    default:
                        break;
                }
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    NickName = model.NickName,
                    ExternalUserId = info.Login.ProviderKey,
                    ExternalUserType = externalUserType,
                    City = model.City,
                    Province = model.Province,
                    District = model.District,
                    Name = model.Name,
                    Gendar = model.Gendar,
                    RegTime = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now),
                    UserType = model.UserType
                };

                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToAction("Index");
                    }
                }
                AddErrors(result);
            }
           
            return View(model);
        }

        // GET: VHome
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Login()
        {         
            WeChatUserInfo userInfo = LoginHelper.GetLoginInfo(Request);
            if (userInfo == null)
            {
                RedirectToAction("Error", MessageConstants.WECHAT_AUTH_ERROR);
            }
           
            UserLoginInfo login = new UserLoginInfo("WeChat", userInfo.UnionId);
            var user =await UserManager.FindAsync(login);
            if (user == null)
            {
                BaseManager manager = new BaseManager();
                List<BArea> provinces = manager.GetAreas(0);
                List<BArea> cities = new List<BArea>();
                List<BArea> districts = new List<BArea>();
                List<BUserType> uTypes = manager.GetUserTypes();
                ViewBag.LoginProvider = login.LoginProvider;
                ViewBag.Provinces =new SelectList(provinces,"Id","Name");               
                ViewBag.Types = new SelectList(uTypes, "Id", "Name");
                ExternalLoginConfirmationViewModel model = new ExternalLoginConfirmationViewModel
                {
                    Email = "",
                    ExternalUserType = 1,
                    ExternalUserId = userInfo.UnionId,
                    Gendar = userInfo.Gendar,
                    Name = "",
                    NickName = userInfo.Name,
                    UserType = 0,
                };

                if (!string.IsNullOrEmpty(userInfo.Province))
                {
                    BArea province = manager.GetAreaByName(userInfo.Province);
                    if(province!=null)
                    {
                        model.Province = province.Id;
                        cities = province.Chindren;
                        if(!string.IsNullOrEmpty(userInfo.City))
                        {
                            if (userInfo.City.Contains("上海"))
                            {
                                BArea city = province;
                                model.City = city.Id;
                                cities = new List<BArea>();
                                cities.Add(new BArea() { Id=province.Id,Name=province.Name });
                                districts = province.Chindren;
                                if (!string.IsNullOrEmpty(userInfo.District))
                                {
                                    BArea district = (from d in districts where d.Name.Contains(userInfo.District) select d).FirstOrDefault<BArea>();
                                    if (district != null)
                                    {
                                        model.District = district.Id;
                                    }
                                }
                            }
                            else {
                                BArea city = (from c in cities where c.Name.Contains(userInfo.Name) select c).FirstOrDefault<BArea>();
                                if (city != null)
                                {
                                    model.City = city.Id;
                                    districts = manager.GetAreas(city.Id);
                                    if (!string.IsNullOrEmpty(userInfo.District))
                                    {
                                        BArea district = (from d in districts where d.Name.Contains(userInfo.District) select d).FirstOrDefault<BArea>();
                                        if (district != null)
                                        {
                                            model.District = district.Id;
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
                ViewBag.Cities = new SelectList(cities, "Id", "Name"); ;
                ViewBag.Districts = new SelectList(districts, "Id", "Name");
                return View("ExternalLoginConfirmation", model);
            }

            return RedirectToAction("Index");            
        }

        public ActionResult Error(string message)
        {
            return View();
        }
    }
}