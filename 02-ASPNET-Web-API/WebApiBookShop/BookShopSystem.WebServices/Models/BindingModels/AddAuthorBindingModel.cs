using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookShopSystem.WebServices.Models.BindingModels
{
    public class AddAuthorBindingModel
    {
        public string FirstName { get; set; }

        [Required(ErrorMessage = "{0} is mandatory!")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

    }
}