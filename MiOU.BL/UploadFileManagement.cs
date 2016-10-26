using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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

        public BFile CreateNewFile(int userId,string directory,string filePath,string name,string ext=null)
        {
            
            BFile bfile = null;
            if (!System.IO.File.Exists(Path.Combine(directory,filePath)))
            {
                throw new MiOUException(string.Format(MiOUConstants.FILE_NOT_EXIST,name!=null?name:""));
            }
            if(string.IsNullOrEmpty(name))
            {
                throw new MiOUException(MiOUConstants.FILE_NAME_IS_EMPTY);
            }
            if(userId==0)
            {
                userId = CurrentLoginUser.User.UserId;
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
