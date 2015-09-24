using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopService.Models
{
    public class RoleBindingModel
    {
        [Required]
        [MinLength(6)]
        public string Username { get; set; }
    }
}
