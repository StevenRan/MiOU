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
using log4net;
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
        private ILog _logger=null;
        public VHomeController()
        {
            Logger = log4net.LogManager.GetLogger(this.GetType().FullName);
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
       
        public ILog Logger
        {
            get { return _logger?? log4net.LogManager.GetLogger(this.GetType().FullName); }
            set { _logger = value; }
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
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnURL = null)
        {
            string provider = string.Empty;
            string errorMessage = null;
            ViewBag.ReturnURL = returnURL;
            if (ModelState.IsValid)
            {                
                if(model.ExternalUserType==1)
                {
                    provider = "WeChat";
                }
                ApplicationUser newUser = new ApplicationUser();
                newUser.Email = model.Email;
                ApplicationUser user = new ApplicationUser()
                {
                    Email = model.Email,
                    NickName = model.NickName,
                    ExternalUserId = model.ExternalUserId,
                    ExternalUserType = model.ExternalUserType,
                    Password= model.Password,
                    City = model.City,
                    Province = model.Province,
                    District = model.District,
                    Name = model.Name,
                    UserName =model.Email,
                    Gendar = model.Gendar,
                    RegTime = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now),
                    Updated=0,
                    Status=1,
                    Phone="",
                    CurrencyAmount=0,
                    AccountAmount=0,
                    VipLevel=0,
                    UserType = model.UserType
                };
                UserLoginInfo login = new UserLoginInfo(provider, model.ExternalUserId);                
                try
                {
                    var emailExisted = await UserManager.FindByEmailAsync(user.Email);                   
                    if (emailExisted==null)
                    {
                        var result = await UserManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            user = await UserManager.FindByEmailAsync(model.Email);                           
                            if (result.Succeeded)
                            {
                                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                                return RedirectToAction("Index");
                            }
                            AddErrors(result);
                        }
                    }
                    else
                    {
                        errorMessage = string.Format("邮箱{0}已经注册过，不能重复注册",model.Email);
                    }
                }
                catch(MiOUException mex)
                {
                    Logger.Error(mex);
                }
                catch(Exception ex)
                {
                    Logger.Fatal(ex);
                }                
            }
            ViewBag.Error = errorMessage;
            BaseManager manager = new BaseManager();
            List<BArea> provinces = manager.GetAreas(0);
            List<BArea> cities = new List<BArea>();
            List<BArea> districts = new List<BArea>();
            List<BUserType> uTypes = manager.GetUserTypes();
            ViewBag.LoginProvider = provider;
            ViewBag.Provinces = new SelectList(provinces, "Id", "Name");
            ViewBag.Types = new SelectList(uTypes, "Id", "Name");

            if(model.Province>0)
            {
                BArea province = (from p in provinces where p.Id == model.Province select new BArea { Id=p.Id,Name=p.Name }).FirstOrDefault<BArea>();
                if (province != null)
                {
                    cities = manager.GetAreas(model.Province);
                    if (model.City>0)
                    {
                        if (model.City==1 || model.City==9 || model.City==22 || model.City==2)
                        {
                            BArea city = province;                           
                            cities = new List<BArea>();
                            cities.Add(new BArea() { Id = city.Id, Name = city.Name });
                            districts = manager.GetAreas(model.Province);                          
                        }
                        else
                        {
                            BArea city = (from c in cities where c.Id==model.City select c).FirstOrDefault<BArea>();
                            if (city != null)
                            {
                                districts = manager.GetAreas(city.Id);                                
                            }
                        }

                    }
                }
            }
            ViewBag.Cities = new SelectList(cities, "Id", "Name"); ;
            ViewBag.Districts = new SelectList(districts, "Id", "Name");
            return View(model);
        }

        // GET: VHome
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnURL=null)
        {
            ViewBag.ReturnURL = returnURL;
            var info = await AuthenticationManager.GetExternalLoginInfoAsync();          
            WeChatUserInfo userInfo = LoginHelper.GetLoginInfo(Request,Session);
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