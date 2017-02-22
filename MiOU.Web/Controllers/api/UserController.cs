using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MiOU.BL;
using MiOU.Entities;
using MiOU.Entities.Beans;
using MiOU.Entities.Exceptions;
using MiOU.Entities.Models;
namespace MiOU.Web.Controllers.api
{
    [Authorize]
    public class UserController : BaseApiController
    {
        [AcceptVerbs("POST")]
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

        [AcceptVerbs("POST")]
        public ApiMessage SaveAddress()
        {
            this.IniRequest();
            ApiMessage result = new ApiMessage();
            int addressId = 0;
            int provinceId = 0;
            int cityId = 0;
            int districtId = 0;
            string phone = request["Phone"];
            string contact = request["Contact"];
            string apartment = request["Apartment"];
            string nearBy = request["NearBy"];
            string address = request["Address"];
            int.TryParse(request["AddressId"], out addressId);
            int.TryParse(request["Province"], out provinceId);
            int.TryParse(request["City"], out cityId);
            int.TryParse(request["District"], out districtId);
            if (addressId < 0)
            {
                result.Message = "POST的参数有误";
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
                return result;
            }

            if (provinceId <= 0) {
                result.Message = "省份不能为空";
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
                return result;
            }
            if (cityId <= 0)
            {
                result.Message = "城市不能为空";
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
                return result;
            }
            if (districtId <= 0)
            {
                result.Message = "区不能为空";
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
                return result;
            }
            if (string.IsNullOrEmpty(contact))
            {
                result.Message = "联系方式不能为空";
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
                return result;
            }
            if (string.IsNullOrEmpty(phone))
            {
                result.Message = "电话不能为空";
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
                return result;
            }
            if (string.IsNullOrEmpty(apartment))
            {
                result.Message = "小区社区不能为空";
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
                return result;
            }
            if (string.IsNullOrEmpty(nearBy))
            {
                result.Message = "靠近不能为空";
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
                return result;
            }
            UserManagement userMgr = new UserManagement(User.Identity.Name);
            MAddress addressModel = new MAddress();
            addressModel.Id = addressId;
            addressModel.Address = address;
            addressModel.Apartment = apartment;
            addressModel.City = cityId;
            addressModel.Contact = contact;
            addressModel.Default = false;
            addressModel.District = districtId;
            addressModel.NearBy = nearBy;
            addressModel.Phone = phone;
            addressModel.Province = provinceId;
            addressModel.User = userMgr.CurrentLoginUser.User.UserId;
            try
            {
                userMgr.SaveAddress(addressModel);
                result.Result = userMgr.GetAddress(addressModel.Id);
                result.Status = ApiCallStatus.OK.ToString();
                result.Message = "藕品地点创建成功";
            }
            catch (MiOUException mex)
            {
                logger.Warn(mex);
                result.Message = mex.Message;
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                result.Message = "服务不可用，请稍后在试";
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
            }
            return result;
        }

        [AcceptVerbs("POST")]
        public ApiMessage DeleteAvactor()
        {
            this.IniRequest();
            ApiMessage result = new ApiMessage();
            int avactorId = 0;
            int.TryParse(request["avactorId"], out avactorId);
            if (avactorId <= 0)
            {
                result.Message = "POST的参数有误";
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
            }
            UserManagement userMgr = new UserManagement(User.Identity.Name);
            try
            {
                userMgr.DeleteAvator(avactorId);
                result.Result = null;
                result.Status = ApiCallStatus.OK.ToString();
                result.Message = "历史头像删除成功";
            }
            catch (MiOUException mex)
            {
                logger.Warn(mex);
                result.Message = mex.Message;
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                result.Message = "服务不可用，请稍后在试";
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
            }
            return result;
        }

        [AcceptVerbs("POST")]
        public ApiMessage SetAvactor()
        {
            this.IniRequest();
            ApiMessage result = new ApiMessage();
            int avactorId = 0;
            int.TryParse(request["avactorId"], out avactorId);
            if (avactorId <= 0)
            {
                result.Message = "POST的参数有误";
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
            }
            UserManagement userMgr = new UserManagement(User.Identity.Name);
            try
            {
                userMgr.SetAvaror(avactorId, userMgr.CurrentLoginUser.User.UserId);
                result.Result = null;
                result.Status = ApiCallStatus.OK.ToString();
                result.Message = "头像设置成功";
            }
            catch (MiOUException mex)
            {
                logger.Warn(mex);
                result.Message = mex.Message;
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                result.Message = "服务不可用，请稍后在试";
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
            }
            return result;
        }

        [AcceptVerbs("POST")]
        public ApiMessage SetDefaultAddress()
        {
            this.IniRequest();
            ApiMessage result = new ApiMessage();
            int addressId = 0;
            int.TryParse(request["addressId"], out addressId);
            if (addressId <= 0)
            {
                result.Message = "POST的参数有误";
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
            }
            UserManagement userMgr = new UserManagement(User.Identity.Name);
            try
            {
                userMgr.SetDefaultAddress(addressId);
                result.Result = null;
                result.Status = ApiCallStatus.OK.ToString();
                result.Message = "默认藕品地点设置成功";
            }
            catch (MiOUException mex)
            {
                logger.Warn(mex);
                result.Message = mex.Message;
                result.Result = null;
                result.Status = ApiCallStatus.ERROR.ToString();
            }
            catch (Exception ex)
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
