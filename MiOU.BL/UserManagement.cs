using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiOU.Entities.Beans;
using MiOU.Entities.Models;
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

        public UserManagement(string name) : base(name)
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

        public List<BAddress> GetAddresses(int userId=0,int addressId=0)
        {
            List<BAddress> addresses = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp =
                    from a in db.AddressBook
                    join aprovince in db.Area on a.Province equals aprovince.Id into lap
                    from llap in lap.DefaultIfEmpty()
                    join acity in db.Area on a.City equals acity.Id into lac
                    from llac in lac.DefaultIfEmpty()
                    join adis in db.Area on a.District equals adis.Id into lad
                    from llad in lad.DefaultIfEmpty()
                    join user in db.User on a.UserId equals user.UserId into luser
                    from lluser in luser.DefaultIfEmpty()
                    orderby a.Default
                    orderby a.Created descending
                    select new BAddress
                    {
                        Address = a.Address,
                        Id = a.Id,
                        Apartment = a.Apartment,
                        City = new BArea { Id = a.City, Name = llac.Name, IsDirect = llac.IsDirect },
                        Province = new BArea { Id = a.Province, Name = llap.Name, IsDirect = llap.IsDirect },
                        District = new BArea { Id = a.District, Name = llad.Name, IsDirect = llad.IsDirect },
                        Contact = a.Contact,
                        Phone = a.Phone,
                        Created = a.Created,
                        Updated = a.Updated,
                        NearBy = a.NearBy,
                        IsDefault = a.Default,
                        User = new BUser { User = lluser }
                    };
                  
                if(addressId>0)
                {
                    tmp = tmp.Where(a => a.Id == addressId);
                }
                if (userId > 0)
                {
                    tmp = tmp.Where(a => a.User.User.UserId == userId);
                }
                addresses = tmp.ToList<BAddress>();
            }
            return addresses;
        }

        public void SetDefaultAddress(int id)
        {
            using (MiOUEntities db = new MiOUEntities())
            {                
                AddressBook newDefaultAddress = (from ab in db.AddressBook where ab.Id==id select ab).FirstOrDefault<AddressBook>();
                if(newDefaultAddress==null)
                {
                    throw new MiOUException("您要设为默认藕品的地点信息不存在");
                }
                if(newDefaultAddress.UserId!=CurrentLoginUser.User.UserId)
                {
                    throw new MiOUException("您要设置的藕品地点不属于您，无法设置");
                }
                AddressBook defaultAddress = (from ab in db.AddressBook where ab.Default == true select ab).FirstOrDefault<AddressBook>();
                if(defaultAddress!=null)
                {
                    if(defaultAddress.UserId!=CurrentLoginUser.User.UserId)
                    {
                        throw new MiOUException("当前的默认藕品地点不属于您，无法对其进行更改，请确保操作自己的藕品地点");
                    }

                    defaultAddress.Default = false;
                }
                newDefaultAddress.Default = true;
                db.SaveChanges();
            }
        }

        public MAddress GetAddressModel(int addressId)
        {
            BAddress address = GetAddress(addressId);
            MAddress model = new MAddress();
            if(address!=null)
            {
                model.Id = address.Id;
                model.Address = address.Address;
                model.Apartment = address.Apartment;
                model.City = address.City.Id;
                model.Province = address.Province.Id;
                model.District = address.District.Id;
                model.NearBy = address.NearBy;
                model.Phone = address.Phone;
                model.User = address.User.User.UserId;
                model.Contact = address.Contact;
            }
            return model;
        }

        public void DeleteAddress(int addressId)
        {
            using (MiOUEntities db = new MiOUEntities())
            {
                AddressBook address = (from a in db.AddressBook where a.Id==addressId select a).FirstOrDefault<AddressBook>();
                if(address==null)
                {
                    throw new MiOUException("此藕品地点不存在");
                }
                if(address.UserId!=CurrentLoginUser.User.UserId)
                {
                    throw new MiOUException("请不要尝试删除别人的藕品地点");
                }

                db.AddressBook.Remove(address);
                db.SaveChanges();
            }
        }
        public BAddress GetAddress(int addressId)
        {
            BAddress address = null;
            List<BAddress> tmps = GetAddresses(0, addressId);
            if(tmps==null || tmps.Count==0)
            {
                throw new MiOUException("当前藕品地点不存在");
            }
            address = tmps[0];
            if(address.User.User.UserId!=CurrentLoginUser.User.UserId)
            {
                throw new MiOUException("不能获取他人的藕品地点信息");
            }
            return address;
        }
        public bool SaveAddress(MAddress address)
        {
            bool ret = false;
            if(address==null)
            {
                throw new MiOUException("藕品地点数据不正确");
            }
            if(string.IsNullOrEmpty(address.Contact))
            {
                throw new MiOUException("联系人不能为空");
            }
            if (string.IsNullOrEmpty(address.Phone))
            {
                throw new MiOUException("联系电话不能为空");
            }
            if (string.IsNullOrEmpty(address.Apartment))
            {
                throw new MiOUException("小区不能为空");
            }
            if (string.IsNullOrEmpty(address.NearBy))
            {
                throw new MiOUException("靠近的商圈不能为空，至少填一个标志性建筑");
            }
            if (address.Province==0)
            {
                throw new MiOUException("省份不能为空");
            }
            if (address.City == 0)
            {
                throw new MiOUException("城市不能为空");
            }
            if (address.District == 0)
            {
                throw new MiOUException("行政区不能为空");
            }
            using (MiOUEntities db = new MiOUEntities())
            {
                AddressBook dbAddress = null;
                if (address.Id == 0)
                {
                    dbAddress = new AddressBook();
                    dbAddress.Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                    dbAddress.UserId = address.User>0?address.User: CurrentLoginUser.User.UserId;
                    db.AddressBook.Add(dbAddress);
                }
                else
                {
                    dbAddress = (from ab in db.AddressBook where ab.Id == address.Id select ab).FirstOrDefault<AddressBook>();
                    if(dbAddress==null)
                    {
                        throw new MiOUException("您要编辑的藕品地点不存在");
                    }
                    if(dbAddress.UserId!=CurrentLoginUser.User.UserId)
                    {
                        throw new MiOUException("当前编辑的藕品地点不属于您，请不要尝试修改别人的数据");
                    }
                    dbAddress.Updated = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                }                
                
                dbAddress.NearBy = address.NearBy;
                dbAddress.Phone = address.Phone;
                dbAddress.Province = address.Province;
                dbAddress.City = address.City;
                dbAddress.Contact = address.Contact;
                dbAddress.District = address.District;
                dbAddress.Phone = address.Phone;
                dbAddress.Address = address.Address;
                dbAddress.Default = address.Default;
                dbAddress.Apartment = address.Apartment;                
                db.SaveChanges();
                ret = true;
            }
            return ret;
        }

        public List<BUserAvator> GetAvtors(int userId)
        {
            List<BUserAvator> acs = null;
            if (userId <= 0 && CurrentLoginUser != null)
            {
                userId = CurrentLoginUser.User.UserId;
            }

            if (userId <= 0)
            {
                throw new MiOUException("输入数据不正确");
            }
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp = from a in db.UserAvator
                          join img in db.File on a.FileId equals img.Id into limg
                          from llimg in limg.DefaultIfEmpty()
                          join usr in db.User on a.UpdatedBy equals usr.UserId into lupdatedby
                          from llupdatedbt in lupdatedby.DefaultIfEmpty()
                          join owner in db.User on a.UserId equals owner.UserId into lowner
                          from llowner in lowner.DefaultIfEmpty()
                          orderby a.Enabled
                          select new BUserAvator
                          {
                              Created = a.Created,
                              Enabled = a.Enabled,
                              Id = a.Id,
                              Image = new BFile { Created = llimg.Created, Path = llimg.Path, UserId = llimg.UserId },
                              Updated = a.Updated,
                              UpdatedBy = new BUser { User = llupdatedbt },
                              Owner=new BUser { User=llowner}
                          };
                if (userId > 0)
                {
                    tmp = tmp.Where(a => a.Owner.User.UserId == userId);
                }
                acs = tmp.ToList<BUserAvator>();
            }
            return acs;
        }
        public bool SaveAvator(int userId, BFile file)
        {
            bool ret = false;
            using (MiOUEntities db = new MiOUEntities())
            {
                if (userId != CurrentLoginUser.User.UserId)
                {
                    throw new MiOUException("请不要尝试修改别人的头像");
                }

                UserAvator old = (from o in db.UserAvator where o.UserId==userId & o.Enabled==true select o).FirstOrDefault<UserAvator>();
                UserAvator newAvator = new UserAvator() { Created=DateTimeUtil.ConvertDateTimeToInt(DateTime.Now), Enabled=true, FileId=file.Id, Updated=0, UpdatedBy=0, UserId=userId};
                db.UserAvator.Add(newAvator);
                db.SaveChanges();
                if(newAvator.Id>0 && old!=null)
                {
                    old.Enabled = false;
                    old.Updated = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                    old.UpdatedBy = CurrentLoginUser.User.UserId;
                    db.SaveChanges();
                    ret = true;
                }
            }
            return ret;
        }
        public bool SetAvaror(int avarorId,int userId)
        {
            bool ret = false;
            using (MiOUEntities db = new MiOUEntities())
            {
                if (userId != CurrentLoginUser.User.UserId)
                {
                    throw new MiOUException("请不要尝试修改别人的头像");
                }

                UserAvator old = (from o in db.UserAvator where o.UserId == userId & o.Enabled == true select o).FirstOrDefault<UserAvator>();
                UserAvator newOne = (from o in db.UserAvator where o.UserId == userId & o.Id==avarorId select o).FirstOrDefault<UserAvator>();               
                if(newOne!=null)
                {
                    newOne.Enabled = true;
                    db.SaveChanges();
                    old.Enabled = false;
                    db.SaveChanges();
                }
                else
                {
                    throw new MiOUException("要设置的头像数据不存在");
                }
            }
            return ret;
        }

        public bool DeleteAvator(int avatorId)
        {
            bool ret = false;
            using (MiOUEntities db = new MiOUEntities())
            {
                UserAvator current = (from o in db.UserAvator where o.Id==avatorId select o).FirstOrDefault<UserAvator>();
                if(current==null)
                {
                    throw new MiOUException("要删除的头像数据不存在");
                }
                if(current.UserId!=CurrentLoginUser.User.UserId)
                {
                    throw new MiOUException("请不要尝试删除别人的历史头像数据");
                }
                if(current.Enabled)
                {
                    throw new MiOUException("无法删除正在使用中的头像，请先上传新头像或者重新设置历史头像为当前头像，然后在删除此头像");
                }
                File file = (from f in db.File where f.Id== current.FileId select f).FirstOrDefault<File>();
                db.UserAvator.Remove(current);
                db.SaveChanges();
                if(file!=null)
                {
                    UploadFileManagement fileMgr = new UploadFileManagement(CurrentLoginUser);
                    fileMgr.RemoveFile(file);
                    ret = true;
                }
            }
            return ret;
        }
    }
}
