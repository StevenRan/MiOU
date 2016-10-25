using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using MiOU.Entities.Beans;
using MiOU.Entities.Exceptions;
using MiOU.Entities;
using MiOU.DAL;
namespace MiOU.BL
{
    public class BaseManager
    {
        protected static object o = new object();
        protected log4net.ILog logger;
        protected string WebSitePhysicalDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
        public BUser CurrentLoginUser { get; private set; }
        public BaseManager()
        {
            this.InitializeLoggger();
        }
        public BaseManager(BUser user)
        {
            this.CurrentLoginUser = user;
            this.InitializeLoggger();
        }
        public BaseManager(User user)
        {
            if (user != null)
            {
                this.CurrentLoginUser = this.GetUserInfo(user.Id);
            }
            this.InitializeLoggger();
        }
        public BaseManager(int userId)
        {
            this.CurrentLoginUser = this.GetUserInfo(userId);
            this.InitializeLoggger();
        }
        public BaseManager(string email)
        {
            this.CurrentLoginUser = this.GetUserInfo(email);
            this.InitializeLoggger();
        }

        protected virtual void InitializeLoggger()
        {
            if (this.logger == null)
            {
                this.logger = log4net.LogManager.GetLogger(this.GetType().FullName);
            }
        }

        public BUser GetUserInfoByNickName(string nickName)
        {
            if (string.IsNullOrEmpty(nickName))
            {
                throw new MiOU.Entities.Exceptions.MiOUException(MiOUConstants.USER_NICK_IS_EMPTY);
            }
            BUser user = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                //user = new BUser();
                //user.User = (from u in db.User
                //             where u.Id == userId select u).FirstOrDefault<User>();


                var u = from usr in db.User
                        join city in db.Area on usr.City equals city.Id into lcity
                        from llcity in lcity.DefaultIfEmpty()
                        join province in db.Area on usr.Province equals province.Id into lprovince
                        from llprovince in lprovince.DefaultIfEmpty()
                        join district in db.Area on usr.Province equals district.Id into ldistrict
                        from lldistrict in ldistrict.DefaultIfEmpty()
                        where usr.NickName == nickName
                        select new BUser
                        {
                            User = usr,
                            Province = new BArea { Id=llprovince.Id,Name=llprovince.Name },
                            City = new BArea { Id = llcity.Id, Name = llcity.Name },
                            District = new BArea { Id = lldistrict.Id, Name = lldistrict.Name },
                        };
                user = u.FirstOrDefault<BUser>();
                if(user==null)
                {
                    logger.Warn(string.Format(MiOUConstants.USER_NICK_NOT_EXIST, nickName));
                    return user;                    
                }
                Admin_Users au = (from ausr in db.Admin_Users where ausr.User_Id == user.User.Id select ausr).FirstOrDefault<Admin_Users>();
                if (au != null)
                {
                    user.IsSuperAdmin = au.IsSuperAdmin;
                    user.IsWebMaster = au.IsWebMaster;
                    user.IsAdmin = true;
                }
                if (!user.IsSuperAdmin)
                {
                    user.Permission = PermissionManagement.GetUserPermissions(user.User.Id);
                }
                else
                {
                    user.Permission = new Permissions();
                    System.Reflection.PropertyInfo[] fields = typeof(Permissions).GetProperties();
                    foreach (System.Reflection.PropertyInfo field in fields)
                    {
                        field.SetValue(user.Permission, true);
                    }
                }
            }
            return user;
        }

        public BUser GetUserInfo(int userId)
        {
            if (userId <= 0)
            {
                throw new MiOUException(MiOUConstants.USER_ID_IS_EMPTY);
            }
            BUser user = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                //user = new BUser();
                //user.User = (from u in db.User
                //             where u.Id == userId select u).FirstOrDefault<User>();


                var u = from usr in db.User
                        join city in db.Area on usr.City equals city.Id into lcity
                        from llcity in lcity.DefaultIfEmpty()
                        join province in db.Area on usr.Province equals province.Id into lprovince
                        from llprovince in lprovince.DefaultIfEmpty()
                        join district in db.Area on usr.Province equals district.Id into ldistrict
                        from lldistrict in ldistrict.DefaultIfEmpty()
                        where usr.Id==userId
                        select new BUser
                        {
                            User = usr,
                            Province = new BArea { Id = llprovince.Id, Name = llprovince.Name },
                            City = new BArea { Id = llcity.Id, Name = llcity.Name },
                            District = new BArea { Id = lldistrict.Id, Name = lldistrict.Name },
                        };
                user = u.FirstOrDefault<BUser>();
                if (user == null)
                {
                    logger.Warn(string.Format(MiOUConstants.USER_ID_NOT_EXIST, userId));
                    return null;
                }
                Admin_Users au = (from ausr in db.Admin_Users where ausr.User_Id == userId select ausr).FirstOrDefault<Admin_Users>();
                if (au != null)
                {
                    user.IsSuperAdmin = au.IsSuperAdmin;
                    user.IsWebMaster = au.IsWebMaster;
                    user.IsAdmin = true;
                }
                if (!user.IsSuperAdmin)
                {
                    user.Permission = PermissionManagement.GetUserPermissions(userId);
                }
                else
                {
                    user.Permission = new Permissions();
                    System.Reflection.PropertyInfo[] fields = typeof(Permissions).GetProperties();
                    foreach (System.Reflection.PropertyInfo field in fields)
                    {
                        field.SetValue(user.Permission, true);
                    }
                }
            }
            return user;
        }

        public BUser GetUserInfo(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new MiOUException(MiOUConstants.USER_EMAIL_IS_EMPTY);
            }
            BUser user = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                //user = new BUser();
                //user.User = (from u in db.User where u.Email == email select u).FirstOrDefault<User>();
                var u = from usr in db.User
                        join city in db.Area on usr.City equals city.Id into lcity
                        from llcity in lcity.DefaultIfEmpty()
                        join province in db.Area on usr.Province equals province.Id into lprovince
                        from llprovince in lprovince.DefaultIfEmpty()
                        join district in db.Area on usr.Province equals district.Id into ldistrict
                        from lldistrict in ldistrict.DefaultIfEmpty()
                        where usr.Email==email
                        select new BUser
                        {
                            User = usr,
                            Province = new BArea { Id = llprovince.Id, Name = llprovince.Name },
                            City = new BArea { Id = llcity.Id, Name = llcity.Name },
                            District = new BArea { Id = lldistrict.Id, Name = lldistrict.Name },
                        };
                user = u.FirstOrDefault<BUser>();
                if(user==null)
                {
                    logger.Warn(string.Format(MiOUConstants.USER_EMAIL_NOT_EXIST,email));
                    return null;
                }
                Admin_Users au = (from ausr in db.Admin_Users where ausr.User_Id == user.User.Id select ausr).FirstOrDefault<Admin_Users>();
                if (au != null)
                {
                    user.IsSuperAdmin = au.IsSuperAdmin;
                    user.IsWebMaster = au.IsWebMaster;
                    user.IsAdmin = true;
                }
                if (!user.IsSuperAdmin)
                {
                    user.Permission = PermissionManagement.GetUserPermissions(user.User.Id);
                }
                else
                {
                    user.Permission = new Permissions();
                    System.Reflection.FieldInfo[] fields = typeof(Permissions).GetFields();
                    foreach (System.Reflection.FieldInfo field in fields)
                    {
                        field.SetValue(user.Permission, 1);
                    }
                }
            }
            return user;
        }

        public List<BArea> GetAreas(int parentId)
        {
            List<BArea> areas = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp = from a in db.Area select new BArea { Name= a.Name, Id=a.Id,Level=a.Level };
                if (parentId > 0)
                {
                    tmp = tmp.Where(a => a.Parent.Id == parentId);
                }
                else
                {
                    tmp = tmp.Where(a => a.Level == 1);
                }

                areas = tmp.OrderBy(a => a.Id).ToList<BArea>();
            }
            return areas;
        }

        public List<BUserType> GetUserTypes()
        {
            List<BUserType> types = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                types = (from t in db.UserType orderby t.Id select new BUserType { Id=t.Id, Name=t.Name }).ToList<BUserType>();
            }
            return types;
        }

        public BArea GetAreaByName(string name)
        {
            BArea area = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp = from a in db.Area
                          where a.Name.Contains(name) && a.Level == 1
                          select new BArea
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Level = a.Level,

                          };

                area = tmp.FirstOrDefault<BArea>();
                if(area!=null)
                {
                    List<BArea> childRen = (from a in db.Area where a.Upid== area.Id select new BArea { Id=a.Id,Name=a.Name, Level=a.Level }).ToList<BArea>();
                    area.Chindren = childRen;
                }
            }
            return area;
        }

        public List<BCategory> GetCategories(int parentId)
        {
            if(parentId<0)
            {
                parentId = 0;
            }
            List<BCategory> categories = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp = from c in db.Category where c.ParentId==parentId orderby c.Order select new BCategory { Id = c.Id, Name = c.Name, Order = c.Order != null ? (int)c.Order : 0 };
                categories = tmp.ToList<BCategory>();
            }
            return categories;
        }
        public List<BPriceCategory> GetPriceCategories()
        {
            List<BPriceCategory> categories = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp = from c in db.PriceCategory orderby c.Order select new BPriceCategory { Name=c.Name,Id=c.Id, Order=c.Order };
                categories = tmp.ToList<BPriceCategory>();
            }
                return categories;
        }

        public List<BSelType> GetDeliveryTypes()
        {
            List<BSelType> dTypes = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp = from d in db.DeliveryType
                          join cb in db.User on d.CreatedBy equals cb.Id into lcb
                          from llcb in lcb.DefaultIfEmpty()
                          join ub in db.User on d.UpdatedBy equals ub.Id into lub
                          from llub in lub.DefaultIfEmpty()
                          orderby d.Id ascending
                          select new BSelType
                          {
                              Id=d.Id,
                              Name=d.Name,
                              Created=d.Created,
                              Updated=d.Updated,
                              CreatedBy = new BUser { User = llcb },
                              UpdatedBy = new BUser { User = llub }
                          };

                dTypes = tmp.ToList<BSelType>();
            }
            return dTypes;
        }

        public List<BPayType> GetPayTypes()
        {
            List<BPayType> types = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp = from d in db.PayType
                          select new BPayType
                          {
                              Id=d.Id,
                              Name=d.Name
                          };
                types = tmp.ToList<BPayType>();
            }
            return types;
        }

        public List<BPayCategory> GetPayCategories()
        {
            List<BPayCategory> cates = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp = from d in db.PayCategory
                          select new BPayCategory
                          {
                              Id = d.Id,
                              Name = d.Name
                          };

                cates = tmp.ToList<BPayCategory>();
            }
            return cates;
        }

        public List<BObject> GetRentTypes()
        {
            List<BObject> types = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp = from d in db.RentType
                          select new BObject
                          {
                              Id = d.Id,
                              Name = d.Name
                          };

                types = tmp.ToList<BObject>();
            }
            return types;
        }
        public List<BMaintenanceType> GetMaintenanceTypes()
        {
            List<BMaintenanceType> dTypes = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp = from d in db.MaintenanceType
                          join cb in db.User on d.CreatedBy equals cb.Id into lcb
                          from llcb in lcb.DefaultIfEmpty()
                          join ub in db.User on d.UpdatedBy equals ub.Id into lub
                          from llub in lub.DefaultIfEmpty()
                          orderby d.Id ascending
                          select new BMaintenanceType
                          {
                              Id = d.Id,
                              Name = d.Name,
                              Description=d.Description,
                              CreatedBy = new BUser { User = llcb },
                              UpdatedBy = new BUser { User = llub }
                          };

                dTypes = tmp.ToList<BMaintenanceType>();
            }
            return dTypes;
        }
        public List<BVIPLevel> GetVipLevels()
        {
            List<BVIPLevel> vips = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp = from c in db.VipLevel
                          join cb in db.User on c.CreatedBy equals cb.Id into lcb
                          from llcb in lcb.DefaultIfEmpty()
                          join ub in db.User on c.UpdatedBy equals ub.Id into lub
                          from llub in lub.DefaultIfEmpty()
                          orderby c.Start ascending
                          select new BVIPLevel
                          {
                              Id = c.Id,
                              Name = c.Name,
                              Description = c.Description,
                              Created = c.Created,
                              Updated = c.Updated,
                              CreatedBy = new BUser { User = llcb },
                              UpdatedBy = new BUser { User = llub },
                              CurrencyAmount = c.CurrencyAmount
                          };

                vips = tmp.ToList<BVIPLevel>();

            }
            return vips;
        }
        protected void SyncObjectProperties(object o1, object o2)
        {
            if (o1 == null || o2 == null)
            {
                return;
            }

            if (o1.GetType().ToString() != o2.GetType().ToString())
            {
                return;
            }

            System.Reflection.PropertyInfo[] properties = o1.GetType().GetProperties();
            if (properties == null || properties.Length == 0)
            {
                return;
            }
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                property.SetValue(o1, property.GetValue(o2));
            }
        }
    }
}
