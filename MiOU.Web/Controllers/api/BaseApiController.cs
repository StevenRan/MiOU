using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web;
using System.Text;
using log4net;

namespace MiOU.Web.Controllers.api
{
    public class BaseApiController : ApiController
    {
        protected HttpContextBase context { get; private set; }
        protected HttpRequestBase request { get; private set; }

        protected ILog logger = null;
        public BaseApiController()
        {
            logger = log4net.LogManager.GetLogger(this.GetType().FullName);
        }

        protected void IniRequest()
        {
            this.context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            this.request = context.Request;
        }
    }
}
