using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace MiOU.Entities.Models
{
    public class MAuditProduct
    {
        public int ProductId { get; set; }
        [Display(Name ="藕品等级")]
        [Required(ErrorMessage = "请选择藕品等级")]
        public int ProductLevel { get; set; }

        [Display(Name = "评估价格")]
        [Required(ErrorMessage = "请输入评估价格")]
        public float EvalutedPrice { get; set; }

        [Display(Name = "评估层色")]
        public float EvalutedPercentage { get; set; }
        public int UserId { get; set; }
        public int Date { get; set; }

        [Display(Name = "审核状态")]
        public int AuditResult { get; set; }
        [Display(Name = "审核备注")]
        public string Message { get; set; }
    }
}
