using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Models
{
    public class Purchase
    {

        public int Id { get; set; }

        public decimal Price { get; set; }

        public DateTime DateOfPurchase { get; set; }

        public bool IsRecalled { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public int BookId { get; set; }

        public virtual Book Book { get; set; }
    }
}
