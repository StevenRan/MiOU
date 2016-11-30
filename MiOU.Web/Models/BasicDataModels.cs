using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace MiOU.Web.Models
{
    public class UserTypeModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "名称不能为空")]
        [Display(Name = "名称")]
        public string Name { get; set; }
       
        [Display(Name = "描述")]
        public string Desc { get; set; }
    }
}