using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MiOU.Entities.Beans
{
    public class AdminActionAttribute : System.Attribute
    {
        public int ID { get; set; }
        public string CategoryName { get; set; }
        public string ActionDescription { get; set; }
    }

    public class Permissions
    {
        [Display(Name = "新建用户")]
        [AdminActionAttribute(ID = 1, CategoryName = "用户管理", ActionDescription = "新建用户")]
        public bool CREATE_USER { get; set; }

        [Display(Name = "删除用户")]
        [AdminActionAttribute(ID = 1, CategoryName = "用户管理", ActionDescription = "删除用户")]
        public bool DELETE_USER { get; set; }

        [Display(Name = "禁用用户")]
        [AdminActionAttribute(ID = 1, CategoryName = "用户管理", ActionDescription = "禁用用户")]
        public bool DISABLE_USER { get; set; }

        [Display(Name = "启用用户")]
        [AdminActionAttribute(ID = 1, CategoryName = "用户管理", ActionDescription = "启用用户")]
        public bool ENABLE_USER { get; set; }

        [Display(Name = "设置用户级别")]
        [AdminActionAttribute(ID = 1, CategoryName = "用户管理", ActionDescription = "设置用户级别")]
        public bool SET_USER_LEVEL { get; set; }

        [Display(Name = "更新用户")]
        [AdminActionAttribute(ID = 1, CategoryName = "用户管理", ActionDescription = "更新用户")]
        public bool UPDATE_USER { get; set; }

        [Display(Name = "更新用户密码")]
        [AdminActionAttribute(ID = 1, CategoryName = "用户管理", ActionDescription = "更新用户密码")]
        public bool UPDATE_USER_PASSWORD { get; set; }

        [Display(Name = "查询用户")]
        [AdminActionAttribute(ID = 1, CategoryName = "用户管理", ActionDescription = "查询用户")]
        public bool SEARCH_USER { get; set; }

        [Display(Name = "修改用户权限")]
        [AdminActionAttribute(ID = 1, CategoryName = "用户管理", ActionDescription = "修改用户权限")]
        public bool UPDATE_USER_PERMISSION { get; set; }

        [Display(Name = "设置超级管理员")]
        [AdminActionAttribute(ID = 1, CategoryName = "用户管理", ActionDescription = "设置超级管理员")]
        public bool SET_USER_SUPER_ADMIN { get; set; }

       



        [Display(Name = "查看产品")]
        [AdminActionAttribute(ID = 2, CategoryName = "产品管理", ActionDescription = "查看资源")]
        public bool VIEW_PRODUCT { get; set; }

        [Display(Name = "修改产品")]
        [AdminActionAttribute(ID = 2, CategoryName = "产品管理", ActionDescription = "修改产品")]
        public bool EDIT_PRODUCT { get; set; }

        [Display(Name = "审核产品")]
        [AdminActionAttribute(ID = 2, CategoryName = "产品管理", ActionDescription = "审核产品")]
        public bool SHENHE_PRODUCT { get; set; }

        [Display(Name = "禁用产品")]
        [AdminActionAttribute(ID = 2, CategoryName = "产品管理", ActionDescription = "禁用产品")]
        public bool DISABLE_PRODUCT { get; set; }

        [Display(Name = "启用产品")]
        [AdminActionAttribute(ID = 2, CategoryName = "产品管理", ActionDescription = "启用产品")]
        public bool ENABLE_PRODUCT { get; set; }

        [Display(Name = "删除产品图片")]
        [AdminActionAttribute(ID = 2, CategoryName = "产品管理", ActionDescription = "删除产品图片")]
        public bool DELETE_PRODUCT_IMAGES { get; set; }


        [Display(Name = "查看订单")]
        [AdminActionAttribute(ID = 3, CategoryName = "订单管理", ActionDescription = "查看订单")]
        public bool VIEW_ORDER { get; set; }

        [Display(Name = "取消订单")]
        [AdminActionAttribute(ID = 3, CategoryName = "订单管理", ActionDescription = "取消订单")]
        public bool CANCEL_ORDER { get; set; }

        [Display(Name = "修改订单")]
        [AdminActionAttribute(ID = 3, CategoryName = "订单管理", ActionDescription = "修改订单")]
        public bool EDIT_ORDER { get; set; }



        [Display(Name = "查看支付记录")]
        [AdminActionAttribute(ID = 4, CategoryName = "支付管理", ActionDescription = "查看支付记录")]
        public bool VIEW_PAYMENT_HISTORY { get; set; }
    }
}
