using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities
{
    public class MiOUConstants
    {
        public const string USER_ID_IS_EMPTY = "用户ID为空";
        public const string USER_NICK_IS_EMPTY = "用户昵称为空";
        public const string USER_NICK_NOT_EXIST = "昵称为{0}的用户不存在";
        public const string USER_ID_NOT_EXIST = "ID为{0}的用户不存在";
        public const string USER_EMAIL_IS_EMPTY = "用户email为空";
        public const string USER_EMAIL_NOT_EXIST = "Email为{0}的用户不存在";
        public const string USER_DISABLE_WEBMASTER = "没有权限禁用网站管理员账户";
        public const string USER_DISABLE_SUPERADMIN = "没有权限禁用超级管理员账户";
        public const string USER_DISABLE_ADMIN = "没有权限禁用管理员账户";
        public const string USER_DISABLE_ACCOUNT = "没有权限禁用用户账户";
        public const string USER_ENABLE_ACCOUNT = "没有权限启用用户账户";

        public const string FILE_NOT_EXIST = "文件{0}不存在";
        public const string FILE_NAME_IS_EMPTY = "文件名称不能为空";
    }
}
