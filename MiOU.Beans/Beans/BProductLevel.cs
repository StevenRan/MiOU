using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Foolproof;
namespace MiOU.Entities.Beans
{
    public class BProductLevel:BObject
    {
        [Required(ErrorMessage = "起始价格不能为空")]
        [Display(Name = "起始价格")]
        [Range(0,double.MaxValue,ErrorMessage ="起始价格必须是数字")]
        [LessThan("EndPrice", ErrorMessage = "起始价格必须小于终止价格")]
        public float StartPrice { get; set; }

        [Required(ErrorMessage = "终止价格不能为空")]
        [Display(Name = "终止价格")]
        [Range(0, double.MaxValue, ErrorMessage = "终止价格必须是数字")]
        [GreaterThan("StartPrice",ErrorMessage ="终止价格必须大于起始价格")]
        public float EndPrice { get; set; }

        [Display(Name = "等级描述")]
        public string Description { get; set; }
        public BUser CreatedBy { get; set; }
        public BUser UpdatedBy { get; set; }
        public List<BVIPLevel> RentableVips { get; set; }

        [Display(Name = "可借用VIP")]
        public string RentableVipLevels { get; set; }
    }
}
