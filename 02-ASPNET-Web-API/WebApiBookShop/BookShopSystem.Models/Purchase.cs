using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopSystem.Models
{
    public class Purchase
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [Display(Name = "Book")]
        public int BookId { get; set; }

        public DateTime? DateOfPurchase { get; set; }

        public virtual Book Book { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [Display(Name = "Is-racalled")]
        public bool IsRecalled { get; set; }
    }
}
