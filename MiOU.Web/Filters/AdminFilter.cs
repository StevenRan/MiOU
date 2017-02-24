using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using MiOU.BL;
namespace MiOU.Web.Filters
{
    public class AdminFilter : System.Web.Mvc.ActionFilterAttribute
    {
        public string Message { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            string username = HttpContext.Current.User.Identity.Name;
            BaseManager baseMgr = new BaseManager(username);
            if(!baseMgr.CurrentLoginUser.IsAdmin)
            {
                HttpContext.Current.Response.Redirect("/Account/LoginError?message=" + this.Message);
            }
        }
    }
}