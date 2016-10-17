using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiOU.Entities.Beans;
using MiOU.Entities.Exceptions;
using MiOU.Entities;
using MiOU.DAL;
using MiOU.Util;
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

        public bool SetUserVIPLevel(int userId, int vipId)
        {
            bool ret = false;
            using (MiOUEntities db = new MiOUEntities())
            {
                User dbUser = (from u in db.User where u.Id == userId select u).FirstOrDefault<User>();
                if (dbUser == null)
                {
                    throw new MiOUException(string.Format("编号为{0}的用户不存在", userId));
                }
                if (CurrentLoginUser.IsAdmin)
                {
                    if (!CurrentLoginUser.Permission.SET_USER_LEVEL)
                    {
                        throw new MiOUException("没有权限设置用户的VIP等级");
                    }
                }
                else
                {
                    if (CurrentLoginUser.User.Id != userId)
                    {
                        throw new MiOUException("没有权限执行此操作");
                    }
                }
                VipLevel vip = (from v in db.VipLevel where v.Id == vipId select v).FirstOrDefault<VipLevel>();
                if (vip == null)
                {
                    throw new MiOUException(string.Format("编号为{0}的VIP不存在", vipId));
                }
                if(dbUser.CurrencyAmount<vip.CurrencyAmount)
                {
                    throw new MiOUException("藕币不足，不能兑换");
                }
                int days = 30;
                if (vip.Expired > 0)
                {
                    days = vip.Expired;
                }
                long dateTimeNow = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                List<UserVip> allExistedVips = (from uvip in db.UserVip where uvip.UserId == userId select uvip).ToList<UserVip>();
                List<UserVip> existedVips = (from uvip in allExistedVips where uvip.UserId==userId && uvip.VipLevelId==vipId && (vip.Expired> dateTimeNow || vip.Expired==0) select uvip).ToList<UserVip>();
                if(existedVips.Count>0)
                {
                    throw new MiOUException("已经兑换过此等级VIP,不能再次兑换");
                }
                List<UserVip> l = (from v in existedVips where v.CurrencyAmount > vip.CurrencyAmount select v).ToList<UserVip>();
                if(l.Count>0)
                {
                    throw new MiOUException("只能兑换比当前等级更高的VIP");
                }
                UserVip uVip = new UserVip() { Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now), CurrencyAmount = vip.CurrencyAmount, UserId = userId, VipLevelId = vipId };
                uVip.Expired =0 ;//uVip.Created + (days * 3600 * 24);
                db.UserVip.Add(uVip);
                db.SaveChanges();
                lock(o)
                {
                    dbUser.CurrencyAmount -= vip.CurrencyAmount;
                    db.SaveChanges();
                    UseCurrencyHistory history = new UseCurrencyHistory();
                    history.Amount = vip.CurrencyAmount;
                    history.Category = 1;
                    history.Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                    history.CreatedBy = CurrentLoginUser.User.Id;
                    history.Description = "兑换"+vip.Name+",使用了"+vip.CurrencyAmount+"藕币";
                    history.Type = 1;
                    history.Updated = 0;
                    history.UpdatedBy = 0;
                    db.UseCurrencyHistory.Add(history);
                    db.SaveChanges();
                    ret = true;
                }
            }
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
