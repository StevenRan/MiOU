using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MiOU.Entities.Models
{
    public class MResetPassword
    {
        [System.ComponentModel.DataAnnotations.Required]
        [EmailAddress]
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Required]       
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "密码至少6位", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("Password", ErrorMessage = "两次密码输入不一致")]
        public string ConfirmPassword { get; set; }

        //generte from server and show in the form
        public string Code { get; set; }

        [Compare("Code", ErrorMessage = "验证码输入不正确")]
        [Display(Name = "验证码")]
        public string Code2 { get; set; }
    }
}
