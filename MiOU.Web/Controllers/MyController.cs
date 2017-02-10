using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MiOU.Entities.Models;
using MiOU.BL;
using MiOU.Entities.Beans;
using MiOU.Entities.Exceptions;
using System.Threading.Tasks;

namespace MiOU.Web.Controllers
{
    [Authorize]
    public class MyController : Controller
    {
        log4net.ILog logger = null;
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
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
        public MyController()
        {
            logger = log4net.LogManager.GetLogger(this.GetType().FullName);
        }
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MyProducts()
        {
            int iType = 0;
            int cate = 0;
            if (!string.IsNullOrEmpty(Request["type"]))
            {
                int.TryParse(Request["type"], out iType);
            }
            if (iType == 0)
            {
                iType = 1;
            }
            if (!string.IsNullOrEmpty(Request["cate"]))
            {
                int.TryParse(Request["cate"], out cate);
            }
            ProductManagement pdt = new ProductManagement(User.Identity.GetUserId<int>());
            List<BObject> rentTypes = pdt.GetRentTypes();
            ViewBag.rTypes = rentTypes;
            ViewBag.selType = iType;
            int total = 0;
            int pageSize = 20;
            int page = 1;
            if(!string.IsNullOrEmpty(Request["pageSize"]))
            {
                int.TryParse(Request["pageSize"],out pageSize);
                if(pageSize==0)
                {
                    pageSize = 20;
                }
            }
            if (!string.IsNullOrEmpty(Request["page"]))
            {
                int.TryParse(Request["page"], out page);
                if (page == 0)
                {
                    page = 1;
                }
            }
            List<BProduct> products = pdt.SearchProducts(null, null, User.Identity.GetUserId<int>(), 0, 0, cate, iType, 0, 0, 0, null, pageSize, page, true, out total, Entities.ProductOrderField.RENTTIMES);
            return View(products);
        }

        public ActionResult AddProduct()
        {
            ProductManagement pdtMgr = new ProductManagement(User.Identity.GetUserId<int>());
            List<BObject> rentTypes = pdtMgr.GetRentTypes();
            List<BSelType> shippingTypes = pdtMgr.GetDeliveryTypes();
            List<BCategory> cates = pdtMgr.GetCategories(0, false);
            ViewBag.rTypes = new SelectList(rentTypes, "Id", "Name");
            ViewBag.sTypes = new SelectList(shippingTypes, "Id", "Name");
            ViewBag.Cates = new SelectList(cates, "Id", "Name");
            ViewBag.cCates = new SelectList(pdtMgr.GetCategories(cates[0].Id), "Id", "Name");
            ViewBag.cPriceCates = pdtMgr.GetPriceCategories();
            ViewBag.Percentages = new SelectList(pdtMgr.GetPercentages(), "Id", "Name");
            ViewBag.ManageTypes = new SelectList(pdtMgr.GetManageTypes(), "Id", "Name");
            MProduct model = new MProduct() { Phone = pdtMgr.CurrentLoginUser.User.Phone, Contact = pdtMgr.CurrentLoginUser.User.Phone };
            return View("ProductForm", model);
        }
        public ActionResult EditProduct(int? productId)
        {
            ProductManagement pdtMgr = new ProductManagement(User.Identity.GetUserId<int>());
            List<BObject> rentTypes = pdtMgr.GetRentTypes();
            List<BSelType> shippingTypes = pdtMgr.GetDeliveryTypes();
            List<BCategory> cates = pdtMgr.GetCategories(0, false);
            ViewBag.rTypes = new SelectList(rentTypes, "Id", "Name");
            ViewBag.sTypes = new SelectList(shippingTypes, "Id", "Name");
            ViewBag.Cates = new SelectList(cates, "Id", "Name");
            ViewBag.cCates = new SelectList(pdtMgr.GetCategories(cates[0].Id), "Id", "Name");
            ViewBag.cPriceCates = pdtMgr.GetPriceCategories();
            ViewBag.Percentages = new SelectList(pdtMgr.GetPercentages(), "Id", "Name");
            ViewBag.ManageTypes = new SelectList(pdtMgr.GetManageTypes(), "Id", "Name");
            MProduct model = new MProduct() { Phone = pdtMgr.CurrentLoginUser.User.Phone, Contact = pdtMgr.CurrentLoginUser.User.Phone };
            
            if(productId!=null)
            {
                int total = 0;
                List<BProduct> products = pdtMgr.SearchProducts(new int[] { (int)productId }, null, 0, 0, 0, 0, 0, 0, 0, 0, null, 1, 1, true, out total);
                if(products.Count==0)
                {
                    return ShowError("此藕品不存在");
                }
                BProduct product = products[0];
                model.Address = product.Address;
                model.CategoryId = product.PCategory.Id;
                model.Percentage = product.Percentage;
                model.Phone = product.Phone;
                model.Id = product.Id;
                model.ChildCategoryId = product.Category.Id;
                model.Name = product.Name;
                model.ManageType = product.ManageType;
                model.Price = product.Price;
                model.RentType = product.RentType.Id;
                model.DeliveryType = product.DeliveryType.Id;
                model.Contact = product.Contact;
                model.Description = product.Description;
                model.Repertory = product.Repertory;

                foreach(BProductImage image in product.Images)
                {
                    if (string.IsNullOrEmpty(model.PhotoIds))
                    {
                        model.PhotoIds = image.Image.Id.ToString();
                    }
                    else
                    {
                        model.PhotoIds +=","+ image.Image.Id.ToString();
                    }
                }

                ViewBag.Product = product;
            }
            else
            {
                return ShowError("请不要随意修改URL里的数据");
            }
            return View("ProductForm", model);
        }

        public ActionResult ShowError(string message)
        {
            return RedirectToAction("Error", "My", new System.Web.Routing.RouteValueDictionary(new { controller = "My", action = "Error", message = message }));
        }
        

        [HttpPost]
        public ActionResult SaveProduct(MProduct model)
        {
            ProductManagement pdtMgr = new ProductManagement(User.Identity.GetUserId<int>());
            List<BObject> rentTypes = pdtMgr.GetRentTypes();
            List<BSelType> shippingTypes = pdtMgr.GetDeliveryTypes();
            List<BCategory> cates = pdtMgr.GetCategories(0, false);
            ViewBag.rTypes = new SelectList(rentTypes, "Id", "Name");
            ViewBag.sTypes = new SelectList(shippingTypes, "Id", "Name");
            ViewBag.Cates = new SelectList(cates, "Id", "Name");
            ViewBag.cCates = new SelectList(pdtMgr.GetCategories(cates[0].Id), "Id", "Name");
            ViewBag.cPriceCates = pdtMgr.GetPriceCategories();
            ViewBag.Percentages = new SelectList(pdtMgr.GetPercentages(), "Id", "Name");
            ViewBag.ManageTypes = new SelectList(pdtMgr.GetManageTypes(), "Id", "Name");
            try
            {
                if(ModelState.IsValid)
                {
                    model.Percentage = model.Percentage / 100;
                    model.PriceCotegories = Request["PriceCotegories"];
                    if (model.Id==0)
                    {                        
                        pdtMgr.CreateProduct(model);
                        //return RedirectToAction("MyProducts");
                        ViewBag.Message = string.Format("您的藕品 {0} 已经添加成功",model.Name);
                    }
                    else
                    {
                        pdtMgr.UpdateProduct(model);
                        //return RedirectToAction("MyProducts");
                        ViewBag.Message = string.Format("您的藕品 {0} 已经编辑成功", model.Name);
                    }
                    //ViewBag.Message = "";
                    //return Redirect("/My/EditProduct?productId="+model.Id); 
                                   
                }
                else
                {
                    string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                    ViewBag.Error = messages;
                }
                
            }   
            catch(MiOUException mex)
            {
                ViewBag.Error = mex.Message;
            }  
            catch(Exception ex)
            {
                logger.Fatal(ex);
                ViewBag.Error = "知名错误，请联络系统管理员";
            }  
            if(model.Id>0)
            {
                int total = 0;
                List<BProduct> products = pdtMgr.SearchProducts(new int[] { model.Id }, null, 0, 0, 0, 0, 0, 0, 0, 0, null, 1, 1, true, out total);
                if (products.Count == 1)
                {
                    ViewBag.Product = products[0];
                }
            }     
            return View("ProductForm", model);
        }

        public ActionResult NormalRentOut()
        {
            return View();
        }
        public ActionResult VIPRentOut()
        {
            return View();
        }

        public ActionResult NormalRentIn()
        {
            return View();
        }
        public ActionResult VIPRentIn()
        {
            return View();
        }

        public ActionResult Vip()
        {
            return View();
        }

        public ActionResult Money()
        {
            return View();
        }

        public ActionResult Currency()
        {
            return View();
        }

        public ActionResult Certificate()
        {
            return View();
        }

        public ActionResult JoinUs()
        {
            return View();
        }
        public ActionResult ContactUs()
        {
            return View();
        }

        public ActionResult AddressBook()
        {
            UserManagement userMgr = new UserManagement(User.Identity.GetUserId<int>());
            List<BAddress> addresses = userMgr.GetAddresses(User.Identity.GetUserId<int>());
            return View(addresses);
        }
        public ActionResult AddAddress()
        {
            UserManagement userMgr = new UserManagement(User.Identity.GetUserId<int>());
            List<BArea> ares = userMgr.GetAreas(0);
            ViewBag.Provinces = new SelectList(ares, "Id", "Name");
            ViewBag.Cities = new SelectList(new List<BArea>(), "Id", "Name");
            ViewBag.Districts = new SelectList(new List<BArea>(), "Id", "Name");
            MAddress model = new MAddress() { Province = userMgr.CurrentLoginUser.User.Province, City = userMgr.CurrentLoginUser.User.City, District = userMgr.CurrentLoginUser.User.District };
            if(model.Province>0)
            {
                BArea province = userMgr.GetAreaByIdWithChildren(model.Province);
                if(province.IsDirect)
                {
                    List<BArea> tmpCities = new List<BArea>();
                    tmpCities.Add(new BArea() { Id = province.Id, Name = province.Name });
                    ViewBag.Cities = new SelectList(tmpCities, "Id", "Name");
                    ViewBag.Districts = new SelectList(province.Chindren, "Id", "Name");
                }
                else
                {
                    ViewBag.Cities = new SelectList(province.Chindren, "Id", "Name");
                }
            }
            return View("AddressForm", model);
        }

        public ActionResult EditAddress(int? addressId)
        {
            UserManagement userMgr = new UserManagement(User.Identity.GetUserId<int>());
            List<BArea> ares = userMgr.GetAreas(0);
            ViewBag.Provinces = new SelectList(ares, "Id", "Name");
            ViewBag.Cities = new SelectList(new List<BArea>(), "Id", "Name");
            ViewBag.Districts = new SelectList(new List<BArea>(), "Id", "Name");
            int id = 0;
            if(addressId!=null)
            {
                id = (int)addressId;
            }
            else
            {
                return ShowError(Url.Encode("请不要随意修改URL"));
            }
            MAddress model = null;
            try
            {
                model = userMgr.GetAddressModel(id);
                if (model.Province > 0)
                {
                    BArea province = userMgr.GetAreaByIdWithChildren(model.Province);
                    if (province.IsDirect)
                    {
                        List<BArea> tmpCities = new List<BArea>();
                        tmpCities.Add(new BArea() { Id = province.Id, Name = province.Name });
                        ViewBag.Cities = new SelectList(tmpCities, "Id", "Name");
                        ViewBag.Districts = new SelectList(province.Chindren, "Id", "Name");
                    }
                    else
                    {
                        ViewBag.Cities = new SelectList(province.Chindren, "Id", "Name");
                    }
                }
                return View("AddressForm", model);
            }
            catch(MiOUException mex)
            {
                logger.Warn(mex);
                return ShowError(Url.Encode(mex.Message));
            }
            catch(Exception ex)
            {
                logger.Fatal(ex);
                return ShowError(Url.Encode("请稍后再试，目前系统忙"));
            }           
        }
        public ActionResult SaveAddress(MAddress address)
        {
            UserManagement userMgr = new UserManagement(User.Identity.GetUserId<int>());
            if (ModelState.IsValid)
            {                
                try
                {
                    bool result = false;
                    if (address.Id > 0)
                    {
                        ViewBag.Message = "藕品地点更新成功";
                    }
                    else
                    {
                        ViewBag.Message = "藕品地点添加成功";
                    }
                    result =userMgr.SaveAddress(address);
                    if(!result)
                    {
                        ViewBag.Message = null;
                        ViewBag.Error = "藕品地点保存失败";
                    }                    
                }
                catch(MiOUException mex)
                {
                    ViewBag.Error = mex.Message;
                }
                catch(Exception ex)
                {
                    ViewBag.Error = "系统错误，请稍后再试";
                    logger.Fatal(ex);
                }
            }
            else
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));
                ViewBag.Error = messages;
            }
            List<BArea> ares = userMgr.GetAreas(0);
            ViewBag.Provinces = new SelectList(ares, "Id", "Name");
            ViewBag.Cities = new SelectList(new List<BArea>(), "Id", "Name");
            ViewBag.Districts = new SelectList(new List<BArea>(), "Id", "Name");
            if (address.Province > 0)
            {
                BArea province = userMgr.GetAreaByIdWithChildren(address.Province);
                if (province.IsDirect)
                {
                    List<BArea> tmpCities = new List<BArea>();
                    tmpCities.Add(new BArea() { Id = province.Id, Name = province.Name });
                    ViewBag.Cities = new SelectList(tmpCities, "Id", "Name");
                    ViewBag.Districts = new SelectList(province.Chindren, "Id", "Name");
                }
                else
                {
                    ViewBag.Cities = new SelectList(province.Chindren, "Id", "Name");
                }
            }
            return View("AddressForm",address);
        }
        public ActionResult ChangePassword()
        {
            MResetPassword model = new MResetPassword() { Email= User.Identity.Name, Password="", ConfirmPassword="",OldPassword="" };
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> ChangePassword(MResetPassword model)
        {
            if(ModelState.IsValid)
            {
                var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId<int>(), model.OldPassword, model.Password);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    ViewBag.Message = "密码修改成功";
                    return View("ChangePassword", new MResetPassword() { Email = User.Identity.Name, Password = "", ConfirmPassword = "", OldPassword = "" });
                }
                else
                {
                    ViewBag.Message = "当前密码不正确";
                    model = new MResetPassword() { Email = User.Identity.Name, Password = "", ConfirmPassword = "", OldPassword = "" };
                }
            }
            else
            {
                string messages = string.Join("; ", ModelState.Values
                                     .SelectMany(x => x.Errors)
                                     .Select(x => x.ErrorMessage));
                ViewBag.Error = messages;
            }
            return View("ChangePassword",model);
        }
        public ActionResult ChangeAvator()
        {
            return View();
        }

    }
}