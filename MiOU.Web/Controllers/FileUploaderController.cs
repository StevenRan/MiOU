using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiOU.Entities;
using MiOU.BL;
using MiOU.Entities.Beans;
using MiOU.Entities.Exceptions;
using Microsoft.AspNet.Identity;

namespace MiOU.Web.Controllers
{
    [Authorize]
    public class FileUploaderController : Controller
    {
        log4net.ILog logger = null;

        public FileUploaderController()
        {
            logger = log4net.LogManager.GetLogger(this.GetType().FullName);
        }

        [HttpPost]
        public JsonResult Upload(IEnumerable<HttpPostedFileBase> FilesInput)
        {
            ApiMessage message = new ApiMessage() {  Status = "OK",Message="Files successfully uploaded."};
            if(FilesInput!=null && FilesInput.Count()>0)
            {
                UploadFileManagement fileMger = new UploadFileManagement(User.Identity.GetUserId<int>());
                message.Result= fileMger.UploadFile(FilesInput);
            }
            return Json(message);
        }

        [Authorize]
        public JsonResult UploadAcator(IEnumerable<HttpPostedFileBase> FilesInput)
        {
            ApiMessage message = new ApiMessage() { Status = "OK", Message = "Files successfully uploaded." };
            if (FilesInput != null && FilesInput.Count() > 0)
            {
                UploadFileManagement fileMger = new UploadFileManagement(User.Identity.GetUserId<int>());
                UserManagement userMgr = new UserManagement(fileMger.CurrentLoginUser);
                try
                {
                    List<BFile> files = fileMger.UploadFile(FilesInput);
                    if (files != null && files.Count > 0)
                    {
                        BFile file = fileMger.UploadFile(FilesInput)[0];
                        bool result = userMgr.SaveAvator(fileMger.CurrentLoginUser.User.UserId,file);
                        if (result)
                        {
                            message.Status = "OK";
                            message.Message = "头像上传成功";
                        }
                        else
                        {
                            message.Status = "ERROR";
                            message.Message = "头像修改失败";
                        }
                    }
                    else
                    {
                        message.Status = "ERROR";
                        message.Message = "头像上传失败";
                    }
                }
                catch(MiOUException mex)
                {
                    logger.Warn(mex);
                    message.Status = "ERROR";
                    message.Message = mex.Message;
                }
                catch(Exception ex)
                {
                    logger.Fatal(ex);
                    message.Status = "ERROR";
                    message.Message = "服务暂时不可用，请稍后再试";
                }
               
            }
            return Json(message);
        }

        [HttpPost]
        public JsonResult Delete()
        {
            ApiMessage message = new ApiMessage();
            
            int id;
            int.TryParse(Request["id"], out id);
            if(id<=0)
            {
                message.Status = "ERROR";
                message.Message = "文件ID不正确";
            }
            else
            {

            }
            return Json(message);
        }
    }
}