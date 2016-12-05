using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MiOU.Entities.Beans
{
    public class BVIPLevel:BObject
    {
        [Display(Name = "描述")]
        public string Description { get; set; }

        [Display(Name = "创建者")]
        public BUser CreatedBy { get; set; }

        [Display(Name = "更新者")]
        public BUser UpdatedBy { get; set; }
        public float Start { get; set; }
        public float End { get; set; }
        [Display(Name = "兑换积分")]
        [Required(ErrorMessage ="兑换积分数不能为空")]
        [Range(0,double.MaxValue,ErrorMessage ="兑换VIP等级所需要的积分数量必须大于0")]
        public float CurrencyAmount { get; set; }

        [Display(Name = "押金比例")]
        [Required(ErrorMessage = "押金比例必须填写")]
        [Range(0, 1, ErrorMessage = "押金比例必须在0-1之间")]
        public float YajinPercentage { get; set; }
    }
}
