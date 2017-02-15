using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MiOU.BL;
using MiOU.Entities.Beans;
using MiOU.Entities.Exceptions;
namespace MiOU.Web.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(int productId)
        {
            ProductManagement pdtMgr = new ProductManagement(0);
            BProduct product = pdtMgr.GetProductDetail(productId);
            return View(product);
        }
    }
}