using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MiOU.BL;
using MiOU.Entities.Beans;
using MiOU.Entities.Models;
using MiOU.Entities.Exceptions;
namespace MiOU.Web.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult List()
        {
            int category = 0;
            int childCategory = 0;
            int deliverType = 0;
            int rentType = 0;
            int level = 0;
            string keywork = Request["keyword"];
            int.TryParse(Request["category"], out category);
            int.TryParse(Request["childCategory"], out childCategory);
            int.TryParse(Request["deliverType"], out deliverType);
            int.TryParse(Request["rentType"], out rentType);
            int.TryParse(Request["level"], out level);

            ProductManagement pdtMgr = new ProductManagement(0);
            List<BCategory> categories=pdtMgr.GetCategories(0, true);
            ViewBag.Categories = categories;
            ViewBag.DeliveryTypes = pdtMgr.GetDeliveryTypes();
            ViewBag.RentTypes = pdtMgr.GetRentTypes();
            ViewBag.Levels = pdtMgr.GetProductLevels(0,0,0,null);
            MSearchProduct model = new MSearchProduct() { Category = category, ChildCategory = childCategory, DeliverType = deliverType, RentType = rentType, Keyword = keywork };

            return View(model);
        }

        public ActionResult Detail(int productId)
        {
            ProductManagement pdtMgr = new ProductManagement(0);
            BProduct product = pdtMgr.GetProductDetail(productId);
            return View(product);
        }
    }
}