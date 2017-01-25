using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiOU.Web.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MyProducts()
        {
            return View();
        }

        public ActionResult AddProduct()
        {
            return View();
        }
    }
}