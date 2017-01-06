using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace MiOU.Entities.Models
{
    public class MSearchProduct
    {
        [Display(Name ="审核状态")]
        public int? Status { get; set; }
        [Display(Name = "关键字")]
        public string Keyword { get; set; }
        [Display(Name = "类别")]
        public int? Category { get; set; }
        [Display(Name = "子类别")]
        public int? ChildCategory { get; set; }
        [Display(Name = "省份")]
        public int? Province { get; set; }
        [Display(Name = "城市")]
        public int? City { get; set; }
        [Display(Name = "区域")]
        public int? District { get; set; }
        [Display(Name = "租赁类型")]
        public int? RentType { get; set; }
        [Display(Name = "交付类型")]
        public int? DeliverType { get; set; }
        [Display(Name = "等级")]
        public int? ProductLevel { get; set; }
        [Display(Name = "层色")]
        public int? Percentage { get; set; }
    }
}
