using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MiOU.Entities.Beans
{
    public class BObject
    {
        [Required(ErrorMessage ="名称不能为空")]
        [Display(Name = "名称")]
        public string Name { get; set; }

        [Display(Name = "编号")]
        public int Id { get; set; }

        [Display(Name = "创建时间")]
        public long Created { get; set; }

        [Display(Name = "更新时间")]
        public long Updated { get; set; }
    }
}
