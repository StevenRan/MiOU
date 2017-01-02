using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiOU.Entities;
using MiOU.BL;
using MiOU.Entities.Beans;
namespace MiOU.Web.Controllers
{
    public class FileUploaderController : Controller
    {
        [HttpPost]
        public JsonResult Upload(IEnumerable<HttpPostedFileBase> FilesInput)
        {
            ApiMessage message = new ApiMessage() {  Status = "OK",Message="Files successfully uploaded."};
            if(FilesInput!=null && FilesInput.Count()>0)
            {
                UploadFileManagement fileMger = new UploadFileManagement(0);
                message.Result= fileMger.UploadFile(FilesInput);
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