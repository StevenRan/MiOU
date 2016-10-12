using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiOU.Entities.Beans;
using MiOU.Entities.Exceptions;
using MiOU.Entities;
using MiOU.DAL;
namespace MiOU.BL
{
    public class UserManagement:BaseManager
    {
        public UserManagement(int userId) : base(userId)
        {
        }

        public UserManagement(BUser user) : base(user)
        {

        }

        public bool IsNickExist(string nickName)
        {
            if(!string.IsNullOrEmpty(nickName))
            {
                return null == GetUserInfoByNickName(nickName);
            }
            return true;
        }

        public bool IsEmailExist(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                return null == GetUserInfo(email);
            }
            return true;
        }

        public bool SetUserVIPLevel(int userId, BVIPLevel level)
        {
            bool ret = false;

            return ret;
        }

        /// <summary>
        /// Set user account status
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="status">0 means active, otherwrise means disabled</param>
        /// <returns></returns>
        public bool SetUserStatus(int userId,int status)
        {
            bool ret = false;
            BUser user = GetUserInfo(userId);
            if(user==null)
            {
                throw new MiOUException(string.Format(MiOUConstants.USER_ID_NOT_EXIST,userId));
            }
            if(user.IsWebMaster)
            {
                throw new MiOUException(MiOUConstants.USER_DISABLE_WEBMASTER);
            }
            if(user.IsSuperAdmin && !CurrentLoginUser.IsWebMaster)
            {
                throw new MiOUException(MiOUConstants.USER_DISABLE_SUPERADMIN);
            }
            if(user.IsAdmin && (!CurrentLoginUser.IsSuperAdmin && !CurrentLoginUser.IsWebMaster))
            {
                throw new MiOUException(MiOUConstants.USER_DISABLE_ADMIN);
            }
            if(status==1 && (CurrentLoginUser.Permission==null || !CurrentLoginUser.Permission.ENABLE_USER))
            {
                throw new MiOUException(MiOUConstants.USER_ENABLE_ACCOUNT);
            }
            if (status == 0 && (CurrentLoginUser.Permission == null || !CurrentLoginUser.Permission.DISABLE_USER))
            {
                throw new MiOUException(MiOUConstants.USER_DISABLE_ACCOUNT);
            }
            MiOUEntities db = null;
            try
            {
                db = new MiOUEntities();
                User dbUser = (from u in db.User where u.Id==user.User.Id select u).FirstOrDefault<User>();               
                if(dbUser==null)
                {
                    throw new MiOUException(string.Format(MiOUConstants.USER_ID_NOT_EXIST, user.User.Id));
                }                

                dbUser.Status = status;
                db.SaveChanges();
                ret = true;
            }
            catch(MiOUException mex)
            {
                logger.Error(mex);
                throw mex;
            }
            catch(Exception ex)
            {
                logger.Fatal(ex);
            }
            finally
            {
                if(db!=null)
                {
                    db.Dispose();
                }
            }
            return ret;
        }
    }
}
