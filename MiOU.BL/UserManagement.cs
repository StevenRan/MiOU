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

        public bool AddUserToAdmin(string[] ids)
        {
            bool ret = false;
            if(ids==null || ids.Length<=0)
            {
                throw new MiOUException("请选择用户，然后保存为管理员");
            }
            if(!CurrentLoginUser.Permission.SET_USER_ADMIN)
            {
                throw new MiOUException("没有权限添加管理员");
            }
            using (MiOUEntities db = new MiOUEntities())
            {
                List<Admin_Users> eAdmins = (from au in db.Admin_Users orderby au.User_Id ascending select au).ToList<Admin_Users>();
                foreach(string id in ids)
                {
                    int uid = 0;
                    int.TryParse(id,out uid);
                    Admin_Users tmp = (from t in db.Admin_Users where t.User_Id==uid select t).FirstOrDefault<Admin_Users>();
                    if(tmp!=null)
                    {
                        continue;
                    }

                    tmp = new Admin_Users() { User_Id=uid, Created=DateTimeUtil.ConvertDateTimeToInt(DateTime.Now), CreayedBy=CurrentLoginUser.User.UserId, Description="管理员", IsSuperAdmin=false, IsWebMaster=false,Updated=0,UpdatedBy=0 };
                    db.Admin_Users.Add(tmp);
                }
                db.SaveChanges();
                ret = true;
            }
            return ret;
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
        public List<BUser> SearchUsers(int page,int pageSize,string name,string nickName,string email, int userType,int openType,int gendar,long startRegTime, long endRegTime,int vip,int province,int city,int district,out int total)
        {
            List<BUser> bUsers = null;
            MiOUEntities db = null;
            total = 0;
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
                              UserType= lltype!=null? new BUserType { Id=lltype.Id,Name=lltype.Name }:null,
                              VIPLevel= llvip!=null? new BVIPLevel { Id=usr.VipLevel, Name= llvip.Name }:null,
                              Gendar= (usr.Gendar==3? new BObject { Id=3,Name ="保密"}:usr.Gendar==1?new BObject { Id=usr.Gendar,Name="男" }: usr.Gendar == 2 ? new BObject { Id = usr.Gendar, Name = "女" }:null)
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
                if(gendar>0)
                {
                    tmp = tmp.Where(u => u.User.Gendar == gendar);
                }                

                tmp = tmp.OrderBy(u => u.User.RegTime);
                if (page==0)
                {
                    page = 1;
                }
                total = tmp.Count();
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

        public List<BAdmin> GetAdministrators()
        {
            if(!CurrentLoginUser.IsWebMaster && !CurrentLoginUser.IsSuperAdmin)
            {
                throw new MiOUException("您没有权限查看管理员列表");
            }

            List<BAdmin> admins = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp = from u in db.Admin_Users
                          join ud in db.User on u.User_Id equals ud.UserId into lud
                          from llud in lud.DefaultIfEmpty()
                          join cd in db.User on u.CreayedBy equals cd.UserId into lcd
                          from llcd in lcd.DefaultIfEmpty()
                          join uud in db.User on u.UpdatedBy equals uud.UserId into luud
                          from lluud in luud.DefaultIfEmpty()
                          orderby u.Created ascending
                          select new BAdmin
                          {
                              User= llud,
                              Created= u.Created,
                              CreatedBy= new BUser { User=llcd },
                              UpdatedBy = new BUser { User = lluud },
                              IsWebMaster= u.IsWebMaster,
                              IsSuperAdmin=u.IsSuperAdmin,
                              Enabled=u.Enabled
                          };

               
                admins = tmp.ToList<BAdmin>();
            }
            return admins;
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
                User dbUser = (from u in db.User where u.UserId == userId select u).FirstOrDefault<User>();
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
                    if (CurrentLoginUser.User.UserId != userId)
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
                    history.CreatedBy = CurrentLoginUser.User.UserId;
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BVIPLevel GetVipDetail(int id)
        {
            BVIPLevel vip = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                vip = (from v in db.VipLevel
                       join cu in db.User on v.CreatedBy equals cu.UserId into lcu
                       from llcu in lcu.DefaultIfEmpty()
                       join uu in db.User on v.UpdatedBy equals uu.UserId into luu
                       from luuu in luu.DefaultIfEmpty()
                       where v.Id == id
                       select new BVIPLevel
                       {
                           Id = v.Id,
                           Name = v.Name,
                           CurrencyAmount = v.CurrencyAmount,
                           Created = v.Created,
                           CreatedBy = new BUser { User = llcu },
                           Updated = v.Updated,
                           UpdatedBy = new BUser { User = luuu },
                           Description = v.Description,
                           YajinPercentage=v.YajinPercentage
                       }).FirstOrDefault<BVIPLevel>();
            }
            return vip;
        }

        public void SaveVip(BVIPLevel vip)
        {
            if(vip==null)
            {
                logger.Error("vip instance cannot be null.");
                throw new MiOUException("数据错误，不能执行此操作");
            }
            VipLevel dbVip = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                if (vip.Id > 0)
                {

                    dbVip = (from v in db.VipLevel where v.Id == vip.Id select v).FirstOrDefault<VipLevel>();
                    if (dbVip == null)
                    {
                        logger.Error(string.Format("编号为{0}的VIP不存在", vip.Id));
                        throw new MiOUException("数据错误，不能执行此操作");
                    }

                    dbVip.Name = vip.Name;
                    dbVip.Description = vip.Description;
                    dbVip.Updated = vip.Updated > 0 ? vip.Updated : DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                    if (vip.UpdatedBy != null && vip.UpdatedBy.User != null)
                    {
                        dbVip.UpdatedBy = vip.UpdatedBy.User.UserId;
                    }
                    else
                    {
                        dbVip.UpdatedBy = CurrentLoginUser.User.UserId;
                    }
                    dbVip.CurrencyAmount = vip.CurrencyAmount;
                    dbVip.YajinPercentage = vip.YajinPercentage;
                }
                else
                {
                    dbVip = new VipLevel()
                    {
                        Name = vip.Name,
                        Description = vip.Description,
                        Created = vip.Created > 0 ? vip.Created : DateTimeUtil.ConvertDateTimeToInt(DateTime.Now),
                        CreatedBy = (vip.CreatedBy != null && vip.CreatedBy.User != null) ? vip.CreatedBy.User.UserId : CurrentLoginUser.User.UserId,
                        Updated = 0,
                        UpdatedBy = 0,
                        YajinPercentage=vip.YajinPercentage,
                        CurrencyAmount = vip.CurrencyAmount
                    };
                    db.VipLevel.Add(dbVip);                   
                }

                VipLevel tmp = (from v in db.VipLevel where v.CurrencyAmount == dbVip.CurrencyAmount select v).FirstOrDefault<VipLevel>();
                if (tmp != null)
                {
                    throw new MiOUException(string.Format("兑换积分为{0}的VIP等级已经存在，请重新输入不一样的积分和名称",dbVip.CurrencyAmount));
                }
                db.SaveChanges();
            }
        }
    }
}
