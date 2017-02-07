using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MiOU.Entities.Models;
using MiOU.BL;
using MiOU.Entities.Beans;

namespace MiOU.Web.Controllers
{
    [Authorize]
    public class MyController : Controller
    {
        log4net.ILog logger = null;

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
    }
}