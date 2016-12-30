using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Models
{
    public class MProduct
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "类别不能为空")]
        [Display(Name = "类别")]
        public int CategoryId { get; set; }
        [Display(Name = "子类别")]
        public int ChildCategoryId { get; set; }
        [Required(ErrorMessage = "名称不能为空")]
        [Display(Name = "名称")]
        public string Name { get; set; }
        [Display(Name = "描述")]
        public string Description { get; set; }
        [Required(ErrorMessage = "照片不能为空")]
        [Display(Name = "照片")]
        public string Photos { get; set; }
        [Required(ErrorMessage = "租赁类别不能为空")]
        [Display(Name = "租赁")]
        public int RentType { get; set; }
        [Required(ErrorMessage = "交付类型不能为空")]
        [Display(Name = "交付")]
        public int DeliveryType { get; set; }       
        [Display(Name = "库存")]
        public int Repertory { get; set; }
        [Display(Name = "层色")]
        [Range(0.1,1,ErrorMessage ="层色只能在0.1 - 1之间")]
        public float Percentage { get; set; }
        [Display(Name = "价格")]
        public int Price { get; set; }
        public string PriceCotegories { get; set; }
    }
}
