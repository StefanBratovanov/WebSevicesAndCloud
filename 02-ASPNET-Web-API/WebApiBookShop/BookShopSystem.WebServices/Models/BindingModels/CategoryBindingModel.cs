using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace BookShopSystem.WebServices.Controllers
{
    public class CategoryBindingModel
    {
        [Required(ErrorMessage = "{0} is mandatory!")]
        [Display(Name = "Name")]
        [MinLength(2)]
        public string Name { get; set; }
    }
}
