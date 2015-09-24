using BookShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopService.Models
{
    public class CategoryDetailedViewModel
    {
        public CategoryDetailedViewModel(Category category)
        {
            this.Id = category.Id;
            this.Name = category.Name;
            //this.Books = category.Books.Select(b => b.Title).ToList();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        //public virtual List<string> Books { get; set; }
    }
}
