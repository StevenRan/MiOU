﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using MiOU.BL;
using MiOU.Entities.Beans;
using MiOU.Entities.Exceptions;
using MiOU.Entities;
using MiOU.Util;

namespace MiOU.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            ProductManagement pdtMgr = new ProductManagement(User.Identity.GetUserId<int>());
            UserManagement userMgr = new UserManagement(User.Identity.GetUserId<int>());
            List<BCategory> categories = pdtMgr.GetHomeProdustListByCategory(0);
            ViewBag.Categories = categories;
            ViewBag.HotOwner = userMgr.GetTopUsers(5);
            return View();
        }

        public ActionResult NotFound()
        {            
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}