using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

        [Required(ErrorMessage = "联系人不能为空")]
        [Display(Name = "联系人")]
        public string Contact { get; set; }

        [Required(ErrorMessage = "电话不能为空")]
        [Display(Name = "电话")]
        public string Phone { get; set; }

        [Display(Name = "地址")]
        public string Address { get; set; }

        [Required(ErrorMessage = "藕品描述不能为空")]
        [Display(Name = "描述")]
        public string Description { get; set; }

       
        [Display(Name = "照片")]
        public string Photos { get; set; }

        public HttpPostedFileBase FilesInput { get; set; }

        [Display(Name = "照片")]
        [Required(ErrorMessage = "照片不能为空")]
        public string PhotoIds { get; set; }

        [Required(ErrorMessage = "租赁类别不能为空")]
        [Display(Name = "租赁")]
        public int RentType { get; set; }

        [Required(ErrorMessage = "交付类型不能为空")]
        [Display(Name = "交付")]
        public int DeliveryType { get; set; }  
             
        [Display(Name = "库存")]
        [Range(1,Int32.MaxValue,ErrorMessage ="库存必须大于或等于1")]
        public int Repertory { get; set; }

        //后台数据库里存入的数据必须是此数值除以100后的数据
        [Display(Name = "层色")]
        [Range(1,100,ErrorMessage ="层色只能在10 - 100之间")]
        public float Percentage { get; set; }

        [Display(Name = "原价")]
        public float Price { get; set; }

        [Display(Name = "租赁形式")]
        public string PriceCotegories { get; set; }

        [Display(Name = "托管方式")]
        public int ManageType { get; set; }
    }
}
