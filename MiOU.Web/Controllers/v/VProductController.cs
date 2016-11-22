using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiOU.Web.Controllers.v
{
    public class VProductController : Controller
    {
        // GET: VProduct
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetProductsByCategory(object categoryId, object rentType)
        {
            int cid = 0;
            int type = 0;
            if(categoryId!=null)
            {
                int.TryParse(categoryId.ToString(),out cid);
            }
            if (rentType != null)
            {
                int.TryParse(rentType.ToString(), out type);
            }
            return View();
        }
    }
}