using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace MiOU.Entities.Beans
{
    public class BCategory:BObject
    {
        public int Order { get; set; }
        [Display(Name="父类")]
        public int? ParentId { get; set; }
        public BCategory Parent { get; set; }
        public List<BCategory> ChildRen { get; set; }
        public string IconPhotoMobile { get; set; }
        public string IconPhotoPC { get; set; }
    }
}
