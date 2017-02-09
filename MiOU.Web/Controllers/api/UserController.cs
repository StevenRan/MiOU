﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MiOU.BL;
using MiOU.Entities;
using MiOU.Entities.Beans;
using MiOU.Entities.Exceptions;
namespace MiOU.Web.Controllers.api
{
    
    public class UserController : BaseApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ApiMessage DeleteAddress()
        {
            this.IniRequest();
            ApiMessage result = new ApiMessage();
            int addressId = 0;
            int.TryParse(request["addressId"], out addressId);
            if(addressId<=0)
            {
                result.Message = "POST的参数有误";
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
            }
            UserManagement userMgr = new UserManagement(User.Identity.Name);
            try
            {
                userMgr.DeleteAddress(addressId);
                result.Result = null;
                result.Status = ApiCallStatus.OK.ToString();
                result.Message = "藕品地点删除成功";
            }
            catch(MiOUException mex)
            {
                logger.Warn(mex);
                result.Message = mex.Message;
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
            }
            catch(Exception ex)
            {
                logger.Fatal(ex);
                result.Message = "服务不可用，请稍后在试";
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
            }
            return result;
        }
    }
}