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

        /// <summary>
        /// 搜索注册用户
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="nickName"></param>
        /// <param name="email"></param>
        /// <param name="userType"></param>
        /// <param name="openType"></param>
        /// <param name="gendar"></param>
        /// <param name="startRegTime"></param>
        /// <param name="endRegTime"></param>
        /// <param name="vip"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="district"></param>
        /// <returns></returns>
        public List<BUser> SearchUsers(int page,int pageSize,string nickName,string email, int userType,int openType,int gendar,int startRegTime,int endRegTime,int vip,int province,int city,int district)
        {
            List<BUser> bUsers = null;
            MiOUEntities db = null;
            try
            {
                db = new MiOUEntities();
                var tmp = from usr in db.User
                          join pro in db.Area on usr.Province equals pro.Id into lpro
                          from llpro in lpro.DefaultIfEmpty()
                          join cit in db.Area on usr.City equals cit.Id into lcit
                          from llcit in lcit.DefaultIfEmpty()
                          join distri in db.Area on usr.District equals distri.Id into ldistri
                          from lldistri in ldistri.DefaultIfEmpty()
                          join type in db.UserType on usr.UserType equals type.Id into ltype
                          from lltype in ltype.DefaultIfEmpty()
                          join vipl in db.VipLevel on usr.VipLevel equals vipl.Id into lvip
                          from llvip in lvip.DefaultIfEmpty()
                          select new BUser
                          {
                              User=usr,
                              City= llcit!=null? new BArea { Id=llcit.Id, Name=llcit.Name }:null,
                              Province = llpro != null ? new BArea { Id = llpro.Id, Name = llpro.Name } : null,
                              District = lldistri != null ? new BArea { Id = lldistri.Id, Name = lldistri.Name } : null,
                              UserType= new BUserType { Id=lltype.Id,Name=lltype.Name },
                              VIPLevel= llvip!=null? new BVIPLevel { Id=usr.VipLevel, Name= llvip.Name }:null,
                          };

                if(!string.IsNullOrEmpty(nickName))
                {
                    tmp = tmp.Where(u=>u.User.NickName.Contains(nickName));
                }
                if (!string.IsNullOrEmpty(email))
                {
                    tmp = tmp.Where(u => u.User.Email.Contains(email));
                }
                if(userType>0)
                {
                    tmp = tmp.Where(u => u.User.UserType == userType);
                }
                if (openType > 0)
                {
                    tmp = tmp.Where(u => u.User.ExternalUserType == openType);
                }
                if (province > 0)
                {
                    tmp = tmp.Where(u => u.User.Province == province);
                }
                if (city > 0)
                {
                    tmp = tmp.Where(u => u.User.City == city);
                }
                if (district > 0)
                {
                    tmp = tmp.Where(u => u.User.District == district);
                }
                if (startRegTime > 0)
                {
                    tmp = tmp.Where(u => u.User.RegTime >= startRegTime);
                }
                if (endRegTime > 0)
                {
                    tmp = tmp.Where(u => u.User.RegTime <= endRegTime);
                }
                tmp = tmp.Where(u => u.User.Gendar == gendar);

                tmp = tmp.OrderBy(u => u.User.RegTime);
                if (page==0)
                {
                    page = 1;
                }
                bUsers = tmp.Skip((page - 1) * pageSize).Take(pageSize).ToList<BUser>();
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
            return bUsers;
        }

        /// <summary>
        /// 判断昵称是否被用过
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        public bool IsNickExist(string nickName)
        {
            if(!string.IsNullOrEmpty(nickName))
            {
                return null == GetUserInfoByNickName(nickName);
            }
            return true;
        }

        /// <summary>
        /// 判断邮箱是否注册过
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool IsEmailExist(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                return null == GetUserInfo(email);
            }
            return true;
        }

        /// <summary>
        /// 兑换VIP等级
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="vipId"></param>
        /// <returns></returns>
        public bool ExchangeUserVIPLevel(int userId, int vipId)
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

       
    }
}
