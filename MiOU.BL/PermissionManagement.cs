﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using MiOU.DAL;
using MiOU.Entities;
using MiOU.Entities.Beans;
using MiOU.Entities.Exceptions;
namespace MiOU.BL
{
    public class PermissionManagement:BaseManager
    {
        public PermissionManagement(int userId) : base(userId)
        {
        }

        public PermissionManagement(BUser user) : base(user)
        {

        }

        /// <summary>
        /// Sync database user permission actions with the definitions of Permissions object
        /// </summary>
        public void SyncPermissionsWithDB()
        {
            if (logger == null)
            {
                logger = log4net.LogManager.GetLogger(typeof(PermissionManagement));
            }

            MiOUEntities db = null;
            try
            {
                db = new MiOUEntities();
                db.Configuration.AutoDetectChangesEnabled = false;
                List<AdminActionAttribute> cates = new List<AdminActionAttribute>();
                List<Admin_Actions> allActions = (from action in db.Admin_Actions select action).ToList<Admin_Actions>();
                List<Admin_Categories> allCates = (from cate in db.Admin_Categories select cate).ToList<Admin_Categories>();

                Type permission = typeof(Permissions);
                PropertyInfo[] fields = permission.GetProperties();
                if (fields == null || fields.Length <= 0)
                {
                    return;
                }

                foreach (PropertyInfo field in fields)
                {
                    AdminActionAttribute attr = field.GetCustomAttribute<AdminActionAttribute>();
                    if (attr != null)
                    {
                        Admin_Actions action = (from a in allActions where a.Name == field.Name select a).FirstOrDefault<Admin_Actions>();
                        if (action == null)
                        {
                            action = new Admin_Actions();
                            action.Name = field.Name;
                            action.Enabled = true;
                            db.Admin_Actions.Add(action);
                        }

                        action.Category = attr.ID;
                        action.Description = attr.ActionDescription;

                        List<Admin_Categories> categories = (from cate in allCates where cate.Id == attr.ID select cate).ToList<Admin_Categories>();
                        if (categories == null || categories.Count == 0)
                        {
                            Admin_Categories newCate = new Admin_Categories() { Id = attr.ID, Name = attr.CategoryName };
                            db.Admin_Categories.Add(newCate);
                            allCates.Add(newCate);
                        }
                    }
                }

                db.SaveChanges();
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
        }

        public void GrantUserPermissions(int userId, Permissions permission)
        {
            if (userId == 0)
            {
                throw new MiOUException("");
            }
            List<Admin_Actions> actions = new List<Admin_Actions>();
            using (MiOUEntities db = new MiOUEntities())
            {
                List<Admin_Actions> allActions = (from ac in db.Admin_Actions select ac).ToList<Admin_Actions>();
                PropertyInfo[] props = permission.GetType().GetProperties();
                foreach (PropertyInfo prop in props)
                {
                    bool hasPermission = (bool)prop.GetValue(permission);
                    if (hasPermission)
                    {
                        Admin_Actions ac = (from acc in allActions where acc.Name == prop.Name select acc).FirstOrDefault<Admin_Actions>();
                        if (ac != null)
                        {
                            actions.Add(ac);
                        }
                    }
                }
            }

            GrantUserPermissions(userId, actions);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        public bool GrantUserPermissions(int userId, List<Admin_Actions> actions)
        {
            if (!CurrentLoginUser.IsSuperAdmin && !CurrentLoginUser.IsWebMaster)
            {
                if (!CurrentLoginUser.Permission.UPDATE_USER_PERMISSION)
                {
                    throw new MiOUException("没有权限修改管理员权限");
                }
            }

            bool ret = false;
            using (MiOUEntities db = new MiOUEntities())
            {
                Admin_Users au = (from u in db.Admin_Users where u.User_Id == userId select u).FirstOrDefault<Admin_Users>();

                if (au.IsSuperAdmin && !CurrentLoginUser.IsWebMaster)
                {
                    throw new MiOUException("没有权限修改超级管理员权限，只有网站管理员才能修改");
                }

                if (actions != null && actions.Count > 0)
                {
                    db.Database.ExecuteSqlCommand("delete from Admin_Users_Actions where User_Id=" + userId.ToString());
                    foreach (Admin_Actions action in actions)
                    {
                        Admin_Users_Actions uaction = new Admin_Users_Actions() { Action_Id = action.Id, User_Id = userId };
                        db.Admin_Users_Actions.Add(uaction);
                    }
                    db.SaveChanges();
                    ret = true;
                }
            }
            return ret;
        }

        public List<UserAdminAction> GetAllAdminActions()
        {
            List<UserAdminAction> actions = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                actions = (from a in db.Admin_Actions
                           join c in db.Admin_Categories on a.Category equals c.Id into lc
                           from llc in lc.DefaultIfEmpty()
                           orderby a.Category ascending
                           orderby a.Name ascending
                           select new UserAdminAction
                           {
                               Action = a,
                               Category =llc
                           }).ToList<UserAdminAction>();
            }
            return actions;
        }

        /// <summary>
        /// Gets user actions list
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="categoryId"></param>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public List<UserAdminAction> GetPermissionActions(int userId, int categoryId, string categoryName)
        {
            List<UserAdminAction> actions = new List<UserAdminAction>();
            using (MiOUEntities db = new MiOUEntities())
            {
                List<Admin_Categories> allcates = (from cate in db.Admin_Categories orderby cate.Id select cate).ToList<Admin_Categories>();
                var userActions = from ua in db.Admin_Users_Actions
                                  join action in db.Admin_Actions on ua.Action_Id equals action.Id into laction
                                  from uaction in laction.DefaultIfEmpty()
                                  join cate in db.Admin_Categories on uaction.Category equals cate.Id into lcate
                                  from ucate in lcate.DefaultIfEmpty()
                                  select new
                                  {
                                      userId = ua.User_Id,
                                      categoryId = uaction.Id,
                                      categoryName = ucate.Name,
                                      actionId = ua.Action_Id,
                                      actionName = uaction.Name,
                                      actionDesc = uaction.Description
                                  };
                if (userId > 0)
                {
                    userActions = userActions.Where(u => u.userId == userId);
                }

                if (categoryId > 0)
                {
                    userActions = userActions.Where(u => u.categoryId == categoryId);
                }

                if (!string.IsNullOrEmpty(categoryName))
                {
                    userActions = userActions.Where(u => u.categoryName == categoryName);
                }

                foreach (var uaction in userActions)
                {
                    UserAdminAction uaa = new UserAdminAction();
                    uaa.Action = new Admin_Actions() { Id = uaction.actionId, Name = uaction.actionName, Description = uaction.actionDesc, Enabled = true };
                    uaa.Category = new Admin_Categories() { Id = uaction.categoryId, Name = uaction.categoryName };
                    actions.Add(uaa);
                }

            }

            return actions;
        }

        /// <summary>
        /// Gets permission categories
        /// </summary>
        /// <param name="id">Query by category Id</param>
        /// <param name="name">Query by category name</param>
        /// <returns>A list of PermissionCategory</returns>
        public List<PermissionCategory> GetPermissionCategories(int id, string name)
        {
            List<PermissionCategory> categories = new List<PermissionCategory>();

            using (MiOUEntities db = new MiOUEntities())
            {
                var cates = from cate in db.Admin_Categories select new PermissionCategory { CategoryDescription = cate.Description, CategoryId = cate.Id, CategoryName = cate.Name };
                if (id > 0)
                {
                    cates = cates.Where(c => c.CategoryId == id);
                }
                if (!string.IsNullOrEmpty(name))
                {
                    cates = cates.Where(c => c.CategoryName == name);
                }
                categories = cates.ToList<PermissionCategory>();
            }

            return categories;
        }

        /// <summary>
        /// Gets single user permissions object
        /// </summary>
        /// <param name="userId">User Id of user</param>
        /// <returns>Instance of Permissions object</returns>
        public static Permissions GetUserPermissions(int userId)
        {
            Permissions permissions = new Permissions();
            PropertyInfo[] fields = permissions.GetType().GetProperties();
            MiOUEntities db = null;
            try
            {
                db = new MiOUEntities();
                Admin_Users au = (from u in db.Admin_Users where u.User_Id == userId select u).FirstOrDefault<Admin_Users>();
                if (au != null && au.IsSuperAdmin)
                {
                    foreach (PropertyInfo f in fields)
                    {
                        f.SetValue(permissions, true);
                    }
                    return permissions;
                }
                List<Admin_Actions> actions = (from a in db.Admin_Actions select a).ToList<Admin_Actions>();
                List<Admin_Users_Actions> userActions = (from ua in db.Admin_Users_Actions where ua.User_Id == userId select ua).ToList<Admin_Users_Actions>();
                if (userActions != null && userActions.Count > 0)
                {
                    foreach (Admin_Users_Actions ua in userActions)
                    {
                        Admin_Actions action = (from a in actions where a.Id == ua.Action_Id select a).FirstOrDefault<Admin_Actions>();
                        if (action != null)
                        {
                            foreach (PropertyInfo f in fields)
                            {
                                if (f.Name == action.Name || au.IsSuperAdmin)
                                {
                                    f.SetValue(permissions, true);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
            return permissions;
        }
    }
}
