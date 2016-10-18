using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiOU.DAL;
using MiOU.BL;
using MiOU.Util;
using MiOU.Entities.Beans;
using MiOU.Entities.Exceptions;
using MiOU.Entities;

namespace MiOU.BL.Admin
{
    public class AdminUserManagement:UserManagement
    {
        public AdminUserManagement(int userId) : base(userId)
        {
        }

        public AdminUserManagement(BUser user) : base(user)
        {

        }

        public void RemoveAdminUser(int userId)
        {
            if (!CurrentLoginUser.Permission.SET_USER_ADMIN)
            {
                throw new MiOUException("没有权限执行此操作");
            }
            using (MiOUEntities db = new MiOUEntities())
            {
                User user = (from u in db.User where u.Id == userId select u).FirstOrDefault<User>();
                if (user == null)
                {
                    throw new MiOUException("此用户不存在");
                }
                Admin_Users adminUser = (from au in db.Admin_Users where au.User_Id == userId select au).FirstOrDefault<Admin_Users>();
                if (adminUser == null)
                {
                    throw new MiOUException("此用不是管理员用户");
                }

                adminUser = new Admin_Users() { IsSuperAdmin = false, IsWebMaster = false, User_Id = userId, Description = "" };
                db.Admin_Users.Remove(adminUser);
                db.SaveChanges();
            }
        }

        public void AddAdminUser(int userId)
        {
            if (!CurrentLoginUser.Permission.SET_USER_ADMIN)
            {
                throw new MiOUException("没有权限执行此操作");
            }
            using (MiOUEntities db = new MiOUEntities())
            {
                User user = (from u in db.User where u.Id == userId select u).FirstOrDefault<User>();
                if (user == null)
                {
                    throw new MiOUException("此用户不存在");
                }
                Admin_Users adminUser = (from au in db.Admin_Users where au.User_Id == userId select au).FirstOrDefault<Admin_Users>();
                if (adminUser != null)
                {
                    throw new MiOUException("此用已经是管理员用户，可以对此用户设置对应的权限");
                }

                adminUser = new Admin_Users() { IsSuperAdmin=false, IsWebMaster=false, User_Id=userId, Description="" };
                db.Admin_Users.Add(adminUser);
                db.SaveChanges();
            }
        }

        public void SetUserPermissions(int userId,List<int> adminActions)
        {
            if(!CurrentLoginUser.Permission.UPDATE_USER_PERMISSION)
            {
                throw new MiOUException("没有权限执行此操作");
            }
            if(adminActions==null || adminActions.Count<=0)
            {
                throw new MiOUException("请至少选择一个权限");
            }
            using (MiOUEntities db = new MiOUEntities())
            {
                User user = (from u in db.User where u.Id == userId select u).FirstOrDefault<User>();
                if(user==null)
                {
                    throw new MiOUException("此用户不存在");
                }
                Admin_Users adminUser = (from au in db.Admin_Users where au.User_Id == userId select au).FirstOrDefault<Admin_Users>();
                if(adminUser==null)
                {
                    throw new MiOUException("此用户还不是管理员，请先把此用户设置未管理员");
                }

                //Remove admin actions
                db.Database.ExecuteSqlCommand("delete from Admin_Users_Actions where User_Id="+userId);

                int[] ids = adminActions.ToArray<int>();
                List<Admin_Actions> actions = (from aa in db.Admin_Actions where ids.Contains(aa.Id) select aa).ToList<Admin_Actions>();
                if(actions.Count>0)
                {
                    foreach(Admin_Actions action in actions)
                    {
                        Admin_Users_Actions aua = new Admin_Users_Actions() { User_Id = userId, Action_Id = action.Id };
                        db.Admin_Users_Actions.Add(aua);
                    }
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Set user account status
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="status">0 means active, otherwrise means disabled</param>
        /// <returns></returns>
        public bool SetUserStatus(int userId, int status)
        {
            bool ret = false;
            BUser user = GetUserInfo(userId);
            if (user == null)
            {
                throw new MiOUException(string.Format(MiOUConstants.USER_ID_NOT_EXIST, userId));
            }
            if (user.IsWebMaster)
            {
                throw new MiOUException(MiOUConstants.USER_DISABLE_WEBMASTER);
            }
            if (user.IsSuperAdmin && !CurrentLoginUser.IsWebMaster)
            {
                throw new MiOUException(MiOUConstants.USER_DISABLE_SUPERADMIN);
            }
            if (user.IsAdmin && (!CurrentLoginUser.IsSuperAdmin && !CurrentLoginUser.IsWebMaster))
            {
                throw new MiOUException(MiOUConstants.USER_DISABLE_ADMIN);
            }
            if (status == 1 && (CurrentLoginUser.Permission == null || !CurrentLoginUser.Permission.ENABLE_USER))
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
                User dbUser = (from u in db.User where u.Id == user.User.Id select u).FirstOrDefault<User>();
                if (dbUser == null)
                {
                    throw new MiOUException(string.Format(MiOUConstants.USER_ID_NOT_EXIST, user.User.Id));
                }

                dbUser.Status = status;
                db.SaveChanges();
                ret = true;
            }
            catch (MiOUException mex)
            {
                logger.Error(mex);
                throw mex;
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
    }
}
