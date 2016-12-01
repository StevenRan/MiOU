using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GridMvc;
using GridMvc.DBGrid;
using MiOU.Entities.Beans;
namespace MiOU.Web.Models
{
    public class MiOuSearchUserModel
    {
        public SearchUserModel SearchModel { get; set; }

        public DBGrid<BUser> UserGrid { get; set; }
    }

    public class SearchUserModel
    {
        [Display(Name = "姓名")]
        public string Name { get; set; }
        [Display(Name = "昵称")]
        public string Nick { get; set; }
        [Display(Name = "省份")]
        public int? Province { get; set; }
        [Display(Name = "城市")]
        public int? City { get; set; }
        [Display(Name = "区域")]
        public int? District { get; set; }
        [Display(Name = "性别")]
        public int Gender { get; set; }
        [Display(Name = "VIP级别")]
        public int? VipLevel { get; set; }
        [Display(Name = "类型")]
        public int? Type { get; set; }
        [Display(Name = "绑定微信?")]
        public bool BindingWeChat { get; set; }
        [Display(Name = "开始时间")]
        public string RegStartTime { get; set; }
        [Display(Name = "结束时间")]
        public string RegEndTime { get; set; } 
        [Display(Name = "地址")]
        public string Address { get; set; }
        [Display(Name = "手机")]
        public string Phone { get; set; }
    }

    // Models returned by AccountController actions.
    public class ExternalLoginConfirmationViewModel
    {
        [DataType(DataType.EmailAddress,ErrorMessage ="邮箱格式不正确")]
        [Required(ErrorMessage ="请输入邮箱")]
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "请输入密码")]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "请输入确认密码")]
        [Display(Name = "确认密码")]
        [Compare("Password",ErrorMessage ="两次密码输入不一致")]
        public string Password2 { get; set; }

        [Display(Name = "昵称")]
        public string NickName { get; set; }

        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Required(ErrorMessage = "请选在账户类型")]
        [Display(Name = "类型")]
        [Range(1, Int32.MaxValue,ErrorMessage = "请选择用户类型")]
        public int UserType { get; set; }

        [Display(Name = "性别")]
        public int Gendar { get; set; }

        [Required(ErrorMessage = "请选择省份")]
        [Display(Name = "省份")]
        [Range(1,Int32.MaxValue,ErrorMessage = "请选择省份")]
        public int Province { get; set; }

        [Required(ErrorMessage = "请选择城市")]
        [Display(Name = "城市")]
        [Range(1, Int32.MaxValue,ErrorMessage = "请选择城市")]
        public int City { get; set; }

        [Required(ErrorMessage = "请选择所在区")]
        [Display(Name = "所在区")]
        [Range(1, Int32.MaxValue,ErrorMessage = "请选择所在区")]
        public int District { get; set; }

        public int ExternalUserType { get; set; }
        public string ExternalUserId { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "邮箱")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Hometown")]
        public string Hometown { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
