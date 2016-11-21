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
    public class ProductController : BaseApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ApiMessage GetTopCategoryStatistic()
        {
            this.IniRequest();
            ApiMessage result = new ApiMessage();
            int rentType=0;
            int.TryParse(request["rentType"],out rentType);
            ProductManagement pdm = new ProductManagement(0);
            List<BCategoryStatistic> statistics = pdm.GetTopCategoryStatistic(rentType);
            result.Result = statistics;
            result.Status = ApiCallStatus.OK.ToString();
            result.Message = "成功获取统计数据";
            return result;
        }
    }
}
