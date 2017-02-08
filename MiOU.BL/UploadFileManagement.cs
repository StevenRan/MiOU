using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Drawing;
using MiOU.DAL;
using MiOU.Entities;
using MiOU.Entities.Beans;
using MiOU.Entities.Exceptions;
using MiOU.Util;

namespace MiOU.BL
{
    public class UploadFileManagement:BaseManager
    {
        public UploadFileManagement(int userId) : base(userId)
        {
        }

        public UploadFileManagement(BUser user) : base(user)
        {

        }

        public bool RemoveFile(int id)
        {
            if(id<=0)
            {
                return false;
            }
            bool ret = false;
            MiOUEntities db = null;
            try
            {
                db = new MiOUEntities();
                DAL.File tmpFile = (from f in db.File where f.Id == id select f).FirstOrDefault<DAL.File>();
                if (tmpFile == null)
                {
                    return false;
                }

                string directory = System.AppDomain.CurrentDomain.BaseDirectory;
                string absPath = tmpFile.Path.Replace("/", "\\");
                string fullPath = System.IO.Path.Combine(directory, absPath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    db.File.Remove(tmpFile);
                }
                db.SaveChanges();
                ret = true;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
            return ret;
        }

        public bool RemoveFile(DAL.File file)
        {
            if(file==null || file.Id==0)
            {
                return false;
            }
            return RemoveFile(file.Id);
        }
        public List<BFile> UploadFile(IEnumerable<HttpPostedFileBase> files)
        {
            List<BFile> savedFiles = null;
            string directory = System.AppDomain.CurrentDomain.BaseDirectory;
            string fileName = Guid.NewGuid().ToString();
            string absPath = @"upload\"+DateTime.Now.Year+@"\"+DateTime.Now.Month+@"\"+DateTime.Now.Day;
            if(!Directory.Exists(Path.Combine(directory,absPath)))
            {
                Directory.CreateDirectory(Path.Combine(directory, absPath));
            }
            string fileExt = ".jpg";
            foreach (HttpPostedFileBase file in files) {
                byte[] bytes = FileUtil.StreamToBytes(file.InputStream);
                Bitmap bitmap = ImageUtil.BytesToBitmap(bytes);
                Bitmap tmp = ImageUtil.GetThumbnail(bitmap, 1024, 1000);
                string absPath2 = absPath + "\\" + fileName + fileExt;
                bool ret=FileUtil.SaveBitmap(tmp,Path.Combine(directory,absPath2));
                if(ret)
                {
                    if (savedFiles == null) { savedFiles = new List<BFile>(); }
                    BFile tmpFile = CreateNewFile(CurrentLoginUser.User.UserId,directory, absPath2, fileName + fileExt,fileExt);
                    if(tmpFile!=null)
                    {
                        savedFiles.Add(tmpFile);
                    }
                }
                bitmap.Dispose();
                tmp.Dispose();
            }
            return savedFiles;
        }
        public BFile CreateNewFile(int userId,string directory,string filePath,string name,string ext=null)
        {
            
            BFile bfile = null;
            if (!System.IO.File.Exists(Path.Combine(directory, filePath)))
            {
                throw new MiOUException(string.Format(MiOUConstants.FILE_NOT_EXIST,name!=null?name:""));
            }
            if(string.IsNullOrEmpty(name))
            {
                throw new MiOUException(MiOUConstants.FILE_NAME_IS_EMPTY);
            }
            if(userId==0)
            {
                userId = CurrentLoginUser!=null? CurrentLoginUser.User.UserId:0;
            }
            using (MiOUEntities db = new MiOUEntities())
            {
                MiOU.DAL.File file = new DAL.File() { Created=DateTimeUtil.ConvertDateTimeToInt(DateTime.Now), Enabled=true, Name=name, Path=filePath.Replace("\\","/"), UserId=userId,Ext=ext };
                db.File.Add(file);
                db.SaveChanges();
                bfile = new BFile()
                {
                     Created=file.Created,
                     Ext=file.Ext,
                     Id=file.Id,
                     Name=file.Name,
                     Path=file.Path
                };
            }
        
            return bfile;
        }
    }
}
