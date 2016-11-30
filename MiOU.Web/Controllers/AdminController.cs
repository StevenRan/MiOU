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
namespace MiOU.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
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
        public ActionResult SearchUsers(SearchUserModel model)
        {

            return View();
        }
        #endregion

        #region common data
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
    }
}