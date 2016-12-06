﻿using System;
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

        #region account related
        public ActionResult SetAdminStatus(int? id)
        {
            return View();
        }

        #region admin related functions

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