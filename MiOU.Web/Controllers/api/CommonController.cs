using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MiOU.Entities;
using MiOU.BL;
using MiOU.Entities.Beans;

namespace MiOU.Web.Controllers.api
{
    public class CommonController :  BaseApiController
    {
        BaseManager manager = new BaseManager();
        public CommonController()
        {

        }
        [HttpPost,HttpGet]
        public ApiMessage GetAreaByParent()
        {
            ApiMessage message = new ApiMessage();
            this.IniRequest();
            int parentId = 0;
            int.TryParse(request["pId"],out parentId);
            List<BArea> ares = manager.GetAreas(parentId);
            message.Status = "OK";
            message.Message = "";
            message.Result = ares;
            return message;
        }
    }
}
