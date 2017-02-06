using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            if (!string.IsNullOrEmpty(Request["type"]))
            {
                int.TryParse(Request["type"], out iType);
            }
            if (iType == 0)
            {
                iType = 1;
            }
            ProductManagement pdt = new ProductManagement(0);
            List<BObject> rentTypes = pdt.GetRentTypes();
            ViewBag.rTypes = rentTypes;
            ViewBag.selType = iType;
            return View();
        }

        public ActionResult AddProduct()
        {
            return View();
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