﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiOU.Entities.Beans;
using MiOU.DAL;
namespace MiOU.BL
{
    public class BaseManager
    {
        protected log4net.ILog logger;
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

        public BUser GetUserInfo(int userId)
        {
            if (userId <= 0)
            {
                return null;
            }
            BUser user = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                user = new BUser();
                user.User = (from u in db.User where u.Id == userId select u).FirstOrDefault<User>();

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
                return null;
            }
            BUser user = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                user = new BUser();
                user.User = (from u in db.User where u.Email == email select u).FirstOrDefault<User>();

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

        public List<Area> GetAreas(int parentId)
        {
            List<Area> areas = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp = from a in db.Area select a;
                if (parentId > 0)
                {
                    tmp = tmp.Where(a => a.Upid == parentId);
                }
                else
                {
                    tmp = tmp.Where(a => a.Level == 1);
                }

                areas = tmp.OrderBy(a => a.Id).ToList<Area>();
            }
            return areas;
        }

        public List<UserType> GetUserTypes()
        {
            List<UserType> types = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                types = (from t in db.UserType orderby t.Id select t).ToList<UserType>();
            }
            return types;
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
