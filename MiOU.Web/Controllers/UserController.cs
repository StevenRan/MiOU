using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MiOU.Entities.Beans;
using MiOU.BL;
using MiOU.Entities;
namespace MiOU.Web.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Top()
        {
            return View();
        }

        public ActionResult Product()
        {
            int userId = 0;
            int categoryId = 0;
            int childCategoryId = 0;
            int.TryParse(Request["user"],out userId);
            int.TryParse(Request["category"],out categoryId);
            int.TryParse(Request["childCategory"],out childCategoryId);
            UserManagement userMgr = new UserManagement(0);
            ProductManagement pdtMgr = new ProductManagement(0);
            int total = 0;
            int[] userIds = null;
            if(userId>0)
            {
                userIds = new int[] { userId };
            }
            int page = 1;
            int pageSize = 30;
            List<BUser> users = userMgr.SearchUsers(userIds, null, null, null, null, null, null, 1, 1, 0, 0, out total, UserOrderField.RENTOUTTIMES);
            List<BProduct> products = pdtMgr.SearchProducts(null, null, userId, 0, categoryId, childCategoryId, 0, 0, 0, 0, null, pageSize, page, true, out total, ProductOrderField.RENTTIMES);
            BUser user = users[0];
            ViewBag.User = user;
            ViewBag.Products = products;
            return View();
        }

        public ActionResult Owner()
        {
            UserManagement userMgr = new UserManagement(0);
            int total = 0;
            int page = 1;
            int pageSize = 30;
            List<BUser> users = userMgr.SearchUsers(null,null,null,null,null,null,null,page,pageSize,0,0, out total,UserOrderField.RENTOUTTIMES);
            ViewBag.Users = users;
            ViewBag.Total = total;
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View();
        }
    }
}