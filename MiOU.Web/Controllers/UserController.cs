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
        public ActionResult Top()
        {
            return View();
        }

        public ActionResult Users()
        {
            return View();
        }
    }
}