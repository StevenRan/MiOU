using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MiOU.BL;
using MiOU.Entities.Beans;
using MiOU.Entities.Exceptions;
using GridMvc;
using GridMvc.DBGrid;
using MiOU.Web.Models;
using MiOU.Util;
using log4net;
namespace MiOU.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        ILog logger = null;

        public AdminController()
        {
            logger= log4net.LogManager.GetLogger(this.GetType().FullName);
        }
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UpdateUserTypes(int id)
        {

            return View();
        }

        #region Product related
        public ActionResult NewProductCategory()
        {
            return View("UpdateProductCategory");
        }

        public ActionResult UpdateProductCategory(int parent)
        {
            return View("UpdateProductCategory");
        }

        public ActionResult SaveProductCategory(BCategory model)
        {
            return View("UpdateProductCategory");
        }


        public ActionResult ProductPriceCates()
        {
            ProductManagement pdtMgr = new ProductManagement(User.Identity.GetUserId<int>());
            List<BPriceCategory> pcates = pdtMgr.GetPriceCategories();
            PageItemsResult<BPriceCategory> result = new PageItemsResult<BPriceCategory>() { CurrentPage = 1, EnablePaging = true, Items = pcates, PageSize = pcates.Count };
            DBGrid<BPriceCategory> grid = new DBGrid<BPriceCategory>(result);
            return View(grid);
        }
        public ActionResult ProductCates(int? parent)
        {
            ProductManagement pdtMgr = new ProductManagement(User.Identity.GetUserId<int>());
            List<BCategory> cates = pdtMgr.GetCategories(0, true);
            PageItemsResult<BCategory> result = new PageItemsResult<BCategory>() { CurrentPage = 1, EnablePaging = true, Items = cates, PageSize = cates.Count };
            DBGrid<BCategory> grid = new DBGrid<BCategory>(result);
            return View(grid);
        }
        public ActionResult NewProduct()
        {
            ProductManagement pdtMgr = new ProductManagement(User.Identity.GetUserId<int>());
            List<BObject> rentTypes = pdtMgr.GetRentTypes();
            List<BSelType> shippingTypes = pdtMgr.GetDeliveryTypes();
            ViewBag.rTypes = new SelectList(rentTypes, "Id", "Name");
            ViewBag.sTypes = new SelectList(shippingTypes, "Id", "Name");
            return View("UpdateProduct");
        }
        #endregion

        #region Product Level Related
        public ActionResult SearchProductLevels()
        {
            ProductManagement pdtMgr = new ProductManagement(User.Identity.GetUserId<int>());
            List<BProductLevel> levels = pdtMgr.GetProductLevels(0,0,0,null);
            PageItemsResult<BProductLevel> result = new PageItemsResult<BProductLevel>() { CurrentPage=1,EnablePaging=true, Items=levels, PageSize=levels.Count,TotalRecords=levels.Count };
            DBGrid<BProductLevel> grid = new DBGrid<BProductLevel>(result);
            return View(grid);
        }
        public ActionResult NewProductLevel()
        {
            ViewBag.Error = null;
            ProductManagement pdtMgr = new ProductManagement(User.Identity.GetUserId<int>());
            List<BVIPLevel> vips = pdtMgr.GetVipLevels();
            ViewBag.AllVips = vips;
            return View("UpdateProductLevel");
        }
        public ActionResult SaveProductLevel(BProductLevel level)
        {
            ViewBag.Error = null;
            ProductManagement pdtMgr = new ProductManagement(User.Identity.GetUserId<int>());
            if(level!=null)
            {
                level.RentableVipLevels = Request["RentableVipLevels"];
                bool ret = false;
                try
                {
                    ret = pdtMgr.CreateNewProductLevel(level);
                    if (ret)
                    {
                        return RedirectToAction("SearchProductLevels");
                    }
                }
                catch (MiOUException mex)
                {
                    ViewBag.Error = mex.Message;
                    logger.Error(mex);
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "系统错误，请联系系统管理员";
                    logger.Fatal(ex);
                }
            }
            else
            {
                ViewBag.Error = "系统错误，请联系系统管理员";
            }            
            List<BVIPLevel> vips = pdtMgr.GetVipLevels();
            ViewBag.AllVips = vips;
            return View("UpdateProductLevel");
        }
        public ActionResult UpdateProductLevel(int id)
        {
            ViewBag.Error = null;
            ProductManagement pdtMgr = new ProductManagement(User.Identity.GetUserId<int>());
            List<BVIPLevel> vips = pdtMgr.GetVipLevels();
            ViewBag.AllVips = vips;
            List<BProductLevel> levels= pdtMgr.GetProductLevels(id, 0, 0, null);
            BProductLevel level = null;
            if(levels.Count==1)
            {
                level = levels[0];
            }
            else
            {
                ShowError("输入的等级编号不正确");
            }
            return View("UpdateProductLevel",level);
        }
        #endregion

        #region account related
        public ActionResult SetAdminStatus()
        {
            PermissionManagement perMgr = new PermissionManagement(User.Identity.GetUserId<int>());
            int id = 0;
            int.TryParse(Request["id"],out id);
            int status = 0;
            int.TryParse(Request["state"],out status);
            try
            {
                perMgr.SetAdminStatus(id, status);
            }
            catch(MiOUException mex)
            {
                logger.Error(mex);
                return ShowError(mex.Message);
            }
            catch(Exception ex)
            {
                logger.Fatal(ex);
            }
            return RedirectToAction("Administrators");
        }

        #region admin related functions

        [HttpGet]
        public ActionResult AdminPermission(int? id)
        {           
            if (id == null)
            {
                return ShowError("请不要随意修改URL链接里的参数");               
            }
            int userId = 0;
            int.TryParse(id.ToString(),out userId);
            if(userId<=0)
            {
                return ShowError("请不要随意修改URL链接里的参数");
            }

            PermissionManagement perMgr = new PermissionManagement(User.Identity.GetUserId<int>());
            UserManagement userMgr = new UserManagement(perMgr.CurrentLoginUser);
            if(!perMgr.CurrentLoginUser.Permission.SET_USER_ADMIN && perMgr.CurrentLoginUser.Permission.SET_USER_SUPER_ADMIN)
            {
                return ShowError("您没有权限执行此操作");
            }
            List<UserAdminAction> actions = perMgr.GetAllAdminActions();
            ViewBag.actions = actions;
            BUser reqUser = perMgr.GetUserInfoWithPermissionInfo(userId);
            ViewBag.reqUser = reqUser;            
            return View(reqUser.Permission);
        }
        [HttpPost]
        public ActionResult AdminPermission(Permissions permission)
        {
            PermissionManagement perMgr = new PermissionManagement(User.Identity.GetUserId<int>());
            try
            {
                int userId = 0;
                int.TryParse(Request["id"], out userId);
                if(userId<=0)
                {
                    return ShowError("参数错误，请不要随意修改URL参数");
                }
                perMgr.GrantUserPermissions(userId,permission);
                return Redirect("/Admin/AdminPermission?id="+userId);
            }
            catch (MiOUException mex)
            {
                return ShowError(mex.Message);
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                return ShowError("系统错误，请联系系统管理员");
            }
        }

        public ActionResult Administrators()
        {
            UserManagement userMgr = new UserManagement(User.Identity.GetUserId<int>());
            List<BAdmin> admins = userMgr.GetAdministrators();
            int pagesize = admins.Count;
            if(pagesize==0)
            {
                pagesize = 30;
            }
            PageItemsResult<BAdmin> result = new PageItemsResult<BAdmin> { CurrentPage = 1, EnablePaging = true, Items = admins, PageSize = pagesize, TotalRecords = admins.Count() };
            DBGrid<BAdmin> grid = new DBGrid<BAdmin>(result);
            return View(grid);
        }
       
        [HttpGet]
        public ActionResult AddAdmin(SearchUserModel searchModel)
        {
            UserManagement userMgr = new UserManagement(User.Identity.GetUserId<int>());
            int page = 1;
            int pageSize = 40;
            if (!string.IsNullOrEmpty(Request["page"]))
            {
                int.TryParse(Request["page"], out page);
            }
            if (!string.IsNullOrEmpty(Request["pageSize"]))
            {
                int.TryParse(Request["pageSize"], out pageSize);
            }
            int total = 0;
            List<BUser> users = userMgr.SearchUsers(page, pageSize,searchModel.Name, searchModel.Nick, searchModel.Email, searchModel.Type != null ? (int)searchModel.Type : 0,
                                                    searchModel.BindingWeChat ? 1 : 0, searchModel.Gender, 0, 0, searchModel.VipLevel != null ? (int)searchModel.VipLevel : 0,
                                                    searchModel.Province != null ? (int)searchModel.Province : 0,
                                                    searchModel.City != null ? (int)searchModel.City : 0,
                                                    searchModel.District != null ? (int)searchModel.District : 0, out total);

            MiOuSearchUserModel model = new MiOuSearchUserModel();
            model.SearchModel = searchModel;
            model.UserGrid = new DBGrid<BUser>(new PageItemsResult<BUser>() { CurrentPage = page, EnablePaging = true, Items = users != null ? users : new List<BUser>(), PageSize = pageSize, TotalRecords = total });
            return View(model);
        }
        [HttpPost]
        public ActionResult AddAdmin()
        {
            MiOuSearchUserModel model = new MiOuSearchUserModel();

            string s = Request["UserId"];
            string[] ids = null;
            if (!string.IsNullOrEmpty(s))
            {
                ids = s.Split(',');

            }
            UserManagement userMgr = new UserManagement(User.Identity.GetUserId<int>());
            try
            {
                if(userMgr.AddUserToAdmin(ids))
                {
                    return Redirect("/Admin/Administrators");
                }
            }
            catch(MiOUException mex)
            {
                return ShowError(mex.Message);
            }
            catch(Exception ex)
            {
                logger.Fatal(ex);
                return ShowError("系统错误，请联系系统管理员");
            }
            return Redirect("/Admin/AddAdmin");
        }

       
        #endregion

        [HttpGet]
        public ActionResult SearchUsers(SearchUserModel searchModel)
        {
            UserManagement userMgr = new UserManagement(User.Identity.GetUserId<int>());
            ViewBag.UserTypes = new SelectList(userMgr.GetUserTypes(), "Id","Name");
            List<BArea> provinces = userMgr.GetAreas(0);
            List<BArea> cities = new List<BArea>();
            List<BArea> districts = new List<BArea>();
            ViewBag.Provinces= new SelectList(provinces, "Id", "Name");           
            ViewBag.Genders= new SelectList(userMgr.GetGenders(), "Id", "Name");
            ViewBag.Vips = new SelectList(userMgr.GetVipLevels(), "Id", "Name");
            if (searchModel.Province!=null && (int)searchModel.Province>0)
            {
                BArea province = (from p in provinces where p.Id==searchModel.Province select p).FirstOrDefault<BArea>();
                if(province!=null)
                {
                    if(province.IsDirect)
                    {
                        cities = new List<BArea>();
                        cities.Add(province);
                        searchModel.City = province.Id;
                        districts = userMgr.GetAreas((int)searchModel.Province);
                    }
                    else
                    {
                        cities = userMgr.GetAreas((int)searchModel.Province);
                        if (searchModel.City == null)
                        {
                            districts = new List<BArea>();
                        }
                        else
                        {
                            districts = userMgr.GetAreas((int)searchModel.City);
                        }                        
                    }
                }
            }
            ViewBag.Cities = new SelectList(cities, "Id", "Name");
            ViewBag.Districts = new SelectList(districts, "Id", "Name");

            long startRegTime = 0;
            long endRegTime = 0;
            if(!string.IsNullOrEmpty(searchModel.RegStartTime))
            {
                startRegTime = DateTimeUtil.ConvertDateTimeToInt(DateTime.Parse(searchModel.RegStartTime));
            }
            if (!string.IsNullOrEmpty(searchModel.RegEndTime))
            {
                endRegTime = DateTimeUtil.ConvertDateTimeToInt(DateTime.Parse(searchModel.RegEndTime));
            }
            int page = 1;
            int pageSize = 40;
            if(!string.IsNullOrEmpty(Request["page"]))
            {
                int.TryParse(Request["page"],out page);
            }
            if (!string.IsNullOrEmpty(Request["pageSize"]))
            {
                int.TryParse(Request["pageSize"], out pageSize);
            }
            int total = 0;
            List<BUser> users = userMgr.SearchUsers(page, pageSize,searchModel.Name, searchModel.Nick,null,searchModel.Type!=null?(int)searchModel.Type:0,
                                                    searchModel.BindingWeChat?1:0,searchModel.Gender,startRegTime,endRegTime,searchModel.VipLevel!=null?(int)searchModel.VipLevel:0,
                                                    searchModel.Province!=null?(int)searchModel.Province:0,
                                                    searchModel.City!=null?(int)searchModel.City:0,
                                                    searchModel.District!=null?(int)searchModel.District:0,out total);

            MiOuSearchUserModel model = new MiOuSearchUserModel();
            model.SearchModel = searchModel;
            model.UserGrid = new DBGrid<BUser>(new PageItemsResult<BUser>() { CurrentPage=page, EnablePaging=true, Items=users!=null?users:new List<BUser>(), PageSize=pageSize, TotalRecords=total });
            return View(model);
        }
        #endregion

        #region common data

        [HttpGet]
        public ActionResult NewVip()
        {
            BVIPLevel newVip = new BVIPLevel();
            return View(newVip);
        }

        [HttpPost]
        public ActionResult NewVip(BVIPLevel vip)
        {
            UserManagement userMgr = null;
            try
            {
                if (ModelState.IsValid)
                {
                    userMgr = new UserManagement(User.Identity.GetUserId<int>());
                    vip.Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                    vip.CreatedBy = userMgr.CurrentLoginUser;
                    vip.Updated = 0;
                    vip.UpdatedBy = null;
                    userMgr.SaveVip(vip);
                    return RedirectToAction("Vips");
                }               
            }
            catch (MiOUException mex)
            {
                logger.Info(mex);
                ViewBag.Message = mex.Message;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                ViewBag.Message = "服务暂时不可用";
            }
            return View(vip);
        }
        public ActionResult UpdateVip(int? id)
        {
            if(id==null)
            {
                return ShowError("参数输入有误");
            }
            UserManagement userMgr = new UserManagement(User.Identity.GetUserId<int>());
            BVIPLevel vip = userMgr.GetVipDetail((int)id);
            return View("NewVip", vip);
        }

        public ActionResult Vips()
        {
            int pageSize = 20;
            int requestPage = 1;
            int.TryParse(Request["page"], out requestPage);
            requestPage = requestPage == 0 ? 1 : requestPage;
            List<BVIPLevel> types = null;
            BaseManager baseMgr = new BaseManager(User.Identity.GetUserId<int>());
            types = baseMgr.GetVipLevels();
            PageItemsResult<BVIPLevel> result = new PageItemsResult<BVIPLevel>() { CurrentPage = 1, Items = types, PageSize = pageSize, TotalRecords = types.Count, EnablePaging = true };
            DBGrid<BVIPLevel> model = new DBGrid<BVIPLevel>(result);
            return View(model);
        }

        public ActionResult UserTypes()
        {
            int pageSize = 20;
            int requestPage = 1;
            int.TryParse(Request["page"], out requestPage);
            requestPage = requestPage == 0 ? 1 : requestPage;
            List<BUserType> types = null;
            BaseManager baseMgr = new BaseManager(User.Identity.GetUserId<int>());
            types = baseMgr.GetUserTypes();
            PageItemsResult<BUserType> result = new PageItemsResult<BUserType>() { CurrentPage = 1, Items = types, PageSize = pageSize, TotalRecords = types.Count, EnablePaging = true };
            DBGrid<BUserType> model = new DBGrid<BUserType>(result);
            return View(model);
        }

        public ActionResult TransferTypes()
        {
            int pageSize = 20;
            int requestPage = 1;
            int.TryParse(Request["page"], out requestPage);
            requestPage = requestPage == 0 ? 1 : requestPage;
            List<BPayType> types = null;
            BaseManager baseMgr = new BaseManager(User.Identity.GetUserId<int>());
            types = baseMgr.GetPayTypes();
            PageItemsResult<BPayType> result = new PageItemsResult<BPayType>() { CurrentPage = 1, Items = types, PageSize = pageSize, TotalRecords = types.Count, EnablePaging = true };
            DBGrid<BPayType> model = new DBGrid<BPayType>(result);
            return View("TransferTypes", model);
        }
        public ActionResult PayCates()
        {
            int pageSize = 20;
            int requestPage = 1;
            int.TryParse(Request["page"], out requestPage);
            requestPage = requestPage == 0 ? 1 : requestPage;
            List<BPayCategory> types = null;
            BaseManager baseMgr = new BaseManager(User.Identity.GetUserId<int>());
            types = baseMgr.GetPayCategories();
            PageItemsResult<BPayCategory> result = new PageItemsResult<BPayCategory>() { CurrentPage = 1, Items = types, PageSize = pageSize, TotalRecords = types.Count, EnablePaging = true };
            DBGrid<BPayCategory> model = new DBGrid<BPayCategory>(result);
            return View("PayCates", model);
        }

        public ActionResult RentTypes()
        {
            int pageSize = 20;
            int requestPage = 1;
            int.TryParse(Request["page"], out requestPage);
            requestPage = requestPage == 0 ? 1 : requestPage;
            List<BObject> types = null;
            BaseManager baseMgr = new BaseManager(User.Identity.GetUserId<int>());
            types = baseMgr.GetRentTypes();
            PageItemsResult<BObject> result = new PageItemsResult<BObject>() { CurrentPage = 1, Items = types, PageSize = pageSize, TotalRecords = types.Count, EnablePaging = true };
            DBGrid<BObject> model = new DBGrid<BObject>(result);
            return View("RentTypes", model);
        }

        public ActionResult MaintanenceTypes()
        {
            int pageSize = 20;
            int requestPage = 1;
            int.TryParse(Request["page"], out requestPage);
            requestPage = requestPage == 0 ? 1 : requestPage;
            List<BMaintenanceType> types = null;
            BaseManager baseMgr = new BaseManager(User.Identity.GetUserId<int>());
            types = baseMgr.GetMaintenanceTypes();
            PageItemsResult<BMaintenanceType> result = new PageItemsResult<BMaintenanceType>() { CurrentPage = 1, Items = types, PageSize = pageSize, TotalRecords = types.Count, EnablePaging = true };
            DBGrid<BMaintenanceType> model = new DBGrid<BMaintenanceType>(result);
            return View("MaintanenceTypes", model);
        }

        public ActionResult DeliveryTypes()
        {
            int pageSize = 20;
            int requestPage = 1;
            int.TryParse(Request["page"], out requestPage);
            requestPage = requestPage == 0 ? 1 : requestPage;
            List<BSelType> types = null;
            BaseManager baseMgr = new BaseManager(User.Identity.GetUserId<int>());
            types = baseMgr.GetDeliveryTypes();
            PageItemsResult<BSelType> result = new PageItemsResult<BSelType>() { CurrentPage = 1, Items = types, PageSize = pageSize, TotalRecords = types.Count, EnablePaging = true };
            DBGrid<BSelType> model = new DBGrid<BSelType>(result);
            return View("DeliveryTypes", model);
        }
        #endregion

        public ActionResult ShowError(string message)
        {
            return RedirectToAction("Error", "Admin", new System.Web.Routing.RouteValueDictionary(new { controller = "Admin", action = "Error", message =message }));
        }

        public ActionResult Error(object message)
        {
            return View("Error",message);
        }
    }
}