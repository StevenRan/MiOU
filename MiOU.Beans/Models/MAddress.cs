using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MiOU.Entities.Models
{
    public class MAddress
    {
        [Display(Name = "编号")]
        public int Id { get; set; }

        [Required(ErrorMessage ="请选择省份")]
        [Display(Name ="省份")]
        public int Province { get; set; }

        [Required(ErrorMessage = "请选择城市")]
        [Display(Name = "城市")]
        public int City { get; set; }

        [Required(ErrorMessage = "请选择行政区")]
        [Display(Name = "区域")]
        public int District { get; set; }

        public int User { get; set; }

        [Required(ErrorMessage = "请填写联系电话")]
        [Display(Name = "电话")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "请填写联系人")]
        [Display(Name = "联系人")]
        public string Contact { get; set; }

        [Required(ErrorMessage = "请填写小区名字")]
        [Display(Name = "小区")]
        public string Apartment { get; set; }

        [Required(ErrorMessage = "请填写附近商圈，如人民广场，外滩...")]
        [Display(Name = "靠近")]
        public string NearBy { get; set; }

        [Display(Name = "地址")]
        public string Address { get; set; }

        [Display(Name = "设为默认")]
        public bool Default { get; set; }
    }
}
